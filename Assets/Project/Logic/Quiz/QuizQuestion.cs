using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoryDungeon.Logic
{
    /// <summary>ダンジョンのキャラが出す4択問題1問分のデータ。</summary>
    public sealed class QuizQuestion
    {
        public const int WrongAnswerCount = 3;

        public string CharacterName { get; }
        public string Prompt { get; }
        public string CorrectAnswer { get; }
        public IReadOnlyList<string> WrongAnswers { get; }
        public string Explanation { get; }

        public QuizQuestion(
            string characterName,
            string prompt,
            string correctAnswer,
            IReadOnlyList<string> wrongAnswers,
            string explanation)
        {
            if (string.IsNullOrWhiteSpace(characterName))
                throw new ArgumentException("キャラ名が空です。", nameof(characterName));
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("問題文が空です。", nameof(prompt));
            if (string.IsNullOrWhiteSpace(correctAnswer))
                throw new ArgumentException("正解が空です。", nameof(correctAnswer));
            if (string.IsNullOrWhiteSpace(explanation))
                throw new ArgumentException("解説が空です。", nameof(explanation));
            if (wrongAnswers == null || wrongAnswers.Count != WrongAnswerCount)
                throw new ArgumentException($"誤りの選択肢は{WrongAnswerCount}つ必要です。", nameof(wrongAnswers));
            if (wrongAnswers.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("空の選択肢があります。", nameof(wrongAnswers));
            if (wrongAnswers.Contains(correctAnswer) || wrongAnswers.Distinct().Count() != wrongAnswers.Count)
                throw new ArgumentException("選択肢に重複があります。", nameof(wrongAnswers));

            CharacterName = characterName;
            Prompt = prompt;
            CorrectAnswer = correctAnswer;
            WrongAnswers = wrongAnswers.ToArray();
            Explanation = explanation;
        }
    }
}
