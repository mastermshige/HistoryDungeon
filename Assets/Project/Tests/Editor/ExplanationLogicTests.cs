using System;
using System.Collections.Generic;
using HistoryDungeon.Logic;
using NUnit.Framework;

namespace HistoryDungeon.Tests
{
    public class ExplanationLogicTests
    {
        private readonly List<int> _pages = new List<int>();
        private int _finishedCount;

        private ExplanationLogic Create(int pageCount)
        {
            var logic = new ExplanationLogic(pageCount);
            logic.PageChanged += _pages.Add;
            logic.Finished += OnFinished;
            return logic;
        }

        private void OnFinished()
        {
            _finishedCount++;
        }

        [SetUp]
        public void SetUp()
        {
            _pages.Clear();
            _finishedCount = 0;
        }

        [Test]
        public void ページ数が0以下なら例外()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplanationLogic(0));
        }

        [Test]
        public void 送るたびにページが進む()
        {
            var logic = Create(3);

            logic.Advance();
            logic.Advance();

            CollectionAssert.AreEqual(new[] { 1, 2 }, _pages);
            Assert.IsTrue(logic.IsLastPage);
            Assert.AreEqual(0, _finishedCount);
        }

        [Test]
        public void 最後のページで送ると終了が通知される()
        {
            var logic = Create(2);

            logic.Advance();
            logic.Advance();

            Assert.IsTrue(logic.IsFinished);
            Assert.AreEqual(1, _finishedCount);
        }

        [Test]
        public void ページが1つだけなら最初の送りで終了する()
        {
            var logic = Create(1);

            logic.Advance();

            Assert.IsTrue(logic.IsFinished);
            Assert.AreEqual(0, _pages.Count);
        }

        [Test]
        public void 終了後に送ると例外()
        {
            var logic = Create(1);
            logic.Advance();

            Assert.Throws<InvalidOperationException>(logic.Advance);
        }
    }
}
