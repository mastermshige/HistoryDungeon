using System;
using HistoryDungeon.Logic;
using NUnit.Framework;

namespace HistoryDungeon.Tests
{
    public class QuizQuestionTests
    {
        [Test]
        public void 正しい内容なら作成できる()
        {
            var question = TestData.Question(1);

            Assert.AreEqual("正解1", question.CorrectAnswer);
            Assert.AreEqual(3, question.WrongAnswers.Count);
        }

        [Test]
        public void 誤りの選択肢が3つでなければ例外()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizQuestion("先生", "問題", "正解", new[] { "誤A", "誤B" }, "解説"));
        }

        [Test]
        public void 誤りの選択肢に正解と同じものがあれば例外()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizQuestion("先生", "問題", "正解", new[] { "正解", "誤B", "誤C" }, "解説"));
        }

        [Test]
        public void 誤りの選択肢が重複していれば例外()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizQuestion("先生", "問題", "正解", new[] { "誤A", "誤A", "誤C" }, "解説"));
        }

        [Test]
        public void 問題文が空なら例外()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuizQuestion("先生", " ", "正解", new[] { "誤A", "誤B", "誤C" }, "解説"));
        }
    }
}
