using System;
using System.Collections.Generic;
using System.Linq;
using HistoryDungeon.Logic;
using NUnit.Framework;

namespace HistoryDungeon.Tests
{
    public class EraQuizLogicTests
    {
        private readonly List<QuestionPresentation> _presented = new List<QuestionPresentation>();
        private readonly List<AnswerResult> _judged = new List<AnswerResult>();
        private int _clearedCount;

        private EraQuizLogic Create(int questionCount, Func<int, int> nextRandom = null)
        {
            var logic = new EraQuizLogic(TestData.Questions(questionCount), nextRandom ?? (max => 0));
            logic.QuestionPresented += _presented.Add;
            logic.AnswerJudged += _judged.Add;
            logic.EraCleared += OnEraCleared;
            return logic;
        }

        private void OnEraCleared()
        {
            _clearedCount++;
        }

        private int CorrectIndexOf(QuestionPresentation presentation)
        {
            return presentation.Choices.ToList().IndexOf($"正解{presentation.QuestionIndex + 1}");
        }

        [SetUp]
        public void SetUp()
        {
            _presented.Clear();
            _judged.Clear();
            _clearedCount = 0;
        }

        [Test]
        public void 問題が0問なら例外()
        {
            Assert.Throws<ArgumentException>(() => new EraQuizLogic(new List<QuizQuestion>(), max => 0));
        }

        [Test]
        public void 開始すると最初の問題が4択で出題される()
        {
            var logic = Create(2);

            logic.Start();

            Assert.AreEqual(1, _presented.Count);
            Assert.AreEqual(0, _presented[0].QuestionIndex);
            Assert.AreEqual("キャラ1", _presented[0].CharacterName);
            Assert.AreEqual(EraQuizLogic.ChoiceCount, _presented[0].Choices.Count);
            Assert.AreEqual(4, _presented[0].Choices.Distinct().Count());
            Assert.Contains("正解1", _presented[0].Choices.ToList());
        }

        [Test]
        public void 二重に開始すると例外()
        {
            var logic = Create(1);
            logic.Start();

            Assert.Throws<InvalidOperationException>(logic.Start);
        }

        [Test]
        public void 選択肢の並びは乱数で変わる()
        {
            var indexes = new HashSet<int>();
            for (var seed = 0; seed < 20; seed++)
            {
                var random = new Random(seed);
                var logic = new EraQuizLogic(TestData.Questions(1), max => random.Next(max));
                QuestionPresentation presentation = null;
                logic.QuestionPresented += p => presentation = p;
                logic.Start();
                indexes.Add(presentation.Choices.ToList().IndexOf("正解1"));
            }

            Assert.Greater(indexes.Count, 1, "正解の位置が毎回同じになっている");
        }

        [Test]
        public void 正解すると次の問題へ進む()
        {
            var logic = Create(2);
            logic.Start();

            logic.Answer(CorrectIndexOf(_presented[0]));

            Assert.AreEqual(1, _judged.Count);
            Assert.IsTrue(_judged[0].IsCorrect);
            Assert.AreEqual(2, _presented.Count);
            Assert.AreEqual(1, _presented[1].QuestionIndex);
            Assert.AreEqual(1, logic.CurrentQuestionIndex);
            Assert.AreEqual(0, _clearedCount);
        }

        [Test]
        public void 不正解なら同じ問題に留まり正解と解説を通知する()
        {
            var logic = Create(2);
            logic.Start();
            var wrongIndex = (CorrectIndexOf(_presented[0]) + 1) % EraQuizLogic.ChoiceCount;

            logic.Answer(wrongIndex);

            Assert.AreEqual(1, _judged.Count);
            Assert.IsFalse(_judged[0].IsCorrect);
            Assert.AreEqual(CorrectIndexOf(_presented[0]), _judged[0].CorrectIndex);
            Assert.AreEqual("正解1", _judged[0].CorrectAnswer);
            Assert.AreEqual("解説1", _judged[0].Explanation);
            Assert.AreEqual(1, _presented.Count, "不正解では次の問題が出てはいけない");
            Assert.AreEqual(0, logic.CurrentQuestionIndex);
        }

        [Test]
        public void 不正解のあと再挑戦して正解すれば進める()
        {
            var logic = Create(2);
            logic.Start();
            var correct = CorrectIndexOf(_presented[0]);

            logic.Answer((correct + 1) % EraQuizLogic.ChoiceCount);
            logic.Answer(correct);

            Assert.AreEqual(2, _judged.Count);
            Assert.IsFalse(_judged[0].IsCorrect);
            Assert.IsTrue(_judged[1].IsCorrect);
            Assert.AreEqual(1, logic.CurrentQuestionIndex);
        }

        [Test]
        public void 最後の問題に正解すると時代クリアが通知される()
        {
            var logic = Create(2);
            logic.Start();
            logic.Answer(CorrectIndexOf(_presented[0]));
            logic.Answer(CorrectIndexOf(_presented[1]));

            Assert.IsTrue(logic.IsCleared);
            Assert.AreEqual(1, _clearedCount);
            Assert.AreEqual(2, _presented.Count, "クリア後に新しい問題が出てはいけない");
        }

        [Test]
        public void 一度も間違えなくても間違えてからでも全問正解ならクリアできる()
        {
            var logic = Create(3);
            logic.Start();

            for (var i = 0; i < 3; i++)
            {
                var correct = CorrectIndexOf(_presented[i]);
                if (i == 1)
                    logic.Answer((correct + 2) % EraQuizLogic.ChoiceCount);
                logic.Answer(correct);
            }

            Assert.IsTrue(logic.IsCleared);
            Assert.AreEqual(1, _clearedCount);
            Assert.AreEqual(4, _judged.Count);
        }

        [Test]
        public void 範囲外の選択肢番号は例外()
        {
            var logic = Create(1);
            logic.Start();

            Assert.Throws<ArgumentOutOfRangeException>(() => logic.Answer(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => logic.Answer(EraQuizLogic.ChoiceCount));
        }

        [Test]
        public void 開始前とクリア後の解答は例外()
        {
            var logic = Create(1);
            Assert.Throws<InvalidOperationException>(() => logic.Answer(0));

            logic.Start();
            logic.Answer(CorrectIndexOf(_presented[0]));
            Assert.Throws<InvalidOperationException>(() => logic.Answer(0));
        }

        [Test]
        public void 問題番号から問題を取り出せる()
        {
            var logic = Create(3);

            Assert.AreEqual("問題2", logic.GetQuestion(1).Prompt);
            Assert.Throws<ArgumentOutOfRangeException>(() => logic.GetQuestion(3));
            Assert.Throws<ArgumentOutOfRangeException>(() => logic.GetQuestion(-1));
        }
    }
}
