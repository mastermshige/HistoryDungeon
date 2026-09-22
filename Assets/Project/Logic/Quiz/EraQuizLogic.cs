using System;
using System.Collections.Generic;

namespace HistoryDungeon.Logic
{
    /// <summary>出題された問題の内容（View に見せる選択肢の並びを含む）。</summary>
    public sealed class QuestionPresentation
    {
        public int QuestionIndex { get; }
        public string CharacterName { get; }
        public string Prompt { get; }
        public IReadOnlyList<string> Choices { get; }

        public QuestionPresentation(int questionIndex, string characterName, string prompt, IReadOnlyList<string> choices)
        {
            QuestionIndex = questionIndex;
            CharacterName = characterName;
            Prompt = prompt;
            Choices = choices;
        }
    }

    /// <summary>1回の解答の判定結果。</summary>
    public sealed class AnswerResult
    {
        public int QuestionIndex { get; }
        public int ChosenIndex { get; }
        public bool IsCorrect { get; }
        public int CorrectIndex { get; }
        public string CorrectAnswer { get; }
        public string Explanation { get; }

        public AnswerResult(int questionIndex, int chosenIndex, bool isCorrect, int correctIndex, string correctAnswer, string explanation)
        {
            QuestionIndex = questionIndex;
            ChosenIndex = chosenIndex;
            IsCorrect = isCorrect;
            CorrectIndex = correctIndex;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
        }
    }

    /// <summary>
    /// 1つの時代のダンジョン出題を進める。問題は番号（インデックス）で扱い、View のことは知らない。
    /// 不正解でも同じ問題に留まり、最終的に全問に正解すると時代クリアになる。
    /// </summary>
    public sealed class EraQuizLogic
    {
        public const int ChoiceCount = QuizQuestion.WrongAnswerCount + 1;

        private readonly IReadOnlyList<QuizQuestion> _questions;
        private readonly Func<int, int> _nextRandom;
        private string[] _currentChoices;
        private int _correctIndex;

        public int QuestionCount => _questions.Count;
        public int CurrentQuestionIndex { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsCleared { get; private set; }

        public event Action<QuestionPresentation> QuestionPresented;
        public event Action<AnswerResult> AnswerJudged;
        public event Action EraCleared;

        /// <param name="nextRandom">0以上 max 未満の整数を返す関数（テストで差し替えるため外から渡す）。</param>
        public EraQuizLogic(IReadOnlyList<QuizQuestion> questions, Func<int, int> nextRandom)
        {
            if (questions == null || questions.Count == 0)
                throw new ArgumentException("問題が1問もありません。", nameof(questions));
            if (nextRandom == null)
                throw new ArgumentNullException(nameof(nextRandom));

            _questions = questions;
            _nextRandom = nextRandom;
        }

        public QuizQuestion GetQuestion(int questionIndex)
        {
            if (questionIndex < 0 || questionIndex >= _questions.Count)
                throw new ArgumentOutOfRangeException(nameof(questionIndex));

            return _questions[questionIndex];
        }

        public void Start()
        {
            if (IsStarted)
                throw new InvalidOperationException("すでに開始しています。");

            IsStarted = true;
            Present(0);
        }

        public void Answer(int choiceIndex)
        {
            if (!IsStarted || IsCleared)
                throw new InvalidOperationException("出題中ではありません。");
            if (choiceIndex < 0 || choiceIndex >= ChoiceCount)
                throw new ArgumentOutOfRangeException(nameof(choiceIndex));

            var question = _questions[CurrentQuestionIndex];
            var isCorrect = choiceIndex == _correctIndex;
            AnswerJudged?.Invoke(new AnswerResult(
                CurrentQuestionIndex, choiceIndex, isCorrect, _correctIndex, question.CorrectAnswer, question.Explanation));

            if (!isCorrect)
                return;

            if (CurrentQuestionIndex + 1 < _questions.Count)
            {
                Present(CurrentQuestionIndex + 1);
                return;
            }

            IsCleared = true;
            EraCleared?.Invoke();
        }

        private void Present(int questionIndex)
        {
            CurrentQuestionIndex = questionIndex;
            var question = _questions[questionIndex];

            var choices = new List<string>(ChoiceCount) { question.CorrectAnswer };
            choices.AddRange(question.WrongAnswers);
            Shuffle(choices);

            _currentChoices = choices.ToArray();
            _correctIndex = choices.IndexOf(question.CorrectAnswer);
            QuestionPresented?.Invoke(new QuestionPresentation(
                questionIndex, question.CharacterName, question.Prompt, _currentChoices));
        }

        private void Shuffle(List<string> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = _nextRandom(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
