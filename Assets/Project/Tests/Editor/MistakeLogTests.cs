using System;
using HistoryDungeon.Logic;
using NUnit.Framework;

namespace HistoryDungeon.Tests
{
    public class MistakeLogTests
    {
        [Test]
        public void 最初は空()
        {
            var log = new MistakeLog();

            Assert.AreEqual(0, log.Count);
        }

        [Test]
        public void 間違えた問題が出会った順に並ぶ()
        {
            var log = new MistakeLog();
            var q1 = TestData.Question(1);
            var q2 = TestData.Question(2);

            log.Record(q2);
            log.Record(q1);

            CollectionAssert.AreEqual(new[] { q2, q1 }, log.Questions);
        }

        [Test]
        public void 同じ問題を何度間違えても1件()
        {
            var log = new MistakeLog();
            var q = TestData.Question(1);

            log.Record(q);
            log.Record(q);

            Assert.AreEqual(1, log.Count);
        }

        [Test]
        public void nullを記録すると例外()
        {
            Assert.Throws<ArgumentNullException>(() => new MistakeLog().Record(null));
        }

        [Test]
        public void Clearで空になる()
        {
            var log = new MistakeLog();
            log.Record(TestData.Question(1));

            log.Clear();

            Assert.AreEqual(0, log.Count);
        }
    }
}
