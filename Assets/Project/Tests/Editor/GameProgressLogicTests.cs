using System;
using System.Collections.Generic;
using HistoryDungeon.Logic;
using NUnit.Framework;

namespace HistoryDungeon.Tests
{
    public class GameProgressLogicTests
    {
        private readonly List<GamePhase> _phases = new List<GamePhase>();

        private GameProgressLogic Create(int eraCount)
        {
            var eras = new List<Era>();
            for (var i = 1; i <= eraCount; i++)
                eras.Add(TestData.Era($"時代{i}", 2));

            var logic = new GameProgressLogic(eras);
            logic.PhaseChanged += _phases.Add;
            return logic;
        }

        [SetUp]
        public void SetUp()
        {
            _phases.Clear();
        }

        [Test]
        public void 時代が0なら例外()
        {
            Assert.Throws<ArgumentException>(() => new GameProgressLogic(new List<Era>()));
        }

        [Test]
        public void 最初はタイトル()
        {
            var logic = Create(2);

            Assert.AreEqual(GamePhase.Title, logic.Phase);
        }

        [Test]
        public void 開始すると最初の時代の教室になる()
        {
            var logic = Create(2);

            logic.StartGame();

            Assert.AreEqual(GamePhase.Classroom, logic.Phase);
            Assert.AreEqual(0, logic.CurrentEraIndex);
            Assert.AreEqual("時代1", logic.CurrentEra.Name);
        }

        [Test]
        public void 教室からダンジョンへ進む()
        {
            var logic = Create(2);
            logic.StartGame();

            logic.FinishClassroom();

            Assert.AreEqual(GamePhase.Dungeon, logic.Phase);
            Assert.AreEqual(0, logic.CurrentEraIndex);
        }

        [Test]
        public void ダンジョンを終えると次の時代の教室へ戻る()
        {
            var logic = Create(2);
            logic.StartGame();
            logic.FinishClassroom();

            logic.FinishDungeon();

            Assert.AreEqual(GamePhase.Classroom, logic.Phase);
            Assert.AreEqual(1, logic.CurrentEraIndex);
            Assert.AreEqual("時代2", logic.CurrentEra.Name);
        }

        [Test]
        public void 最終時代のダンジョンを終えるとエンディングになる()
        {
            var logic = Create(2);
            logic.StartGame();
            logic.FinishClassroom();
            logic.FinishDungeon();
            logic.FinishClassroom();

            logic.FinishDungeon();

            Assert.AreEqual(GamePhase.Ending, logic.Phase);
        }

        [Test]
        public void エンディングからタイトルへ戻ると最初からやり直せる()
        {
            var logic = Create(1);
            logic.StartGame();
            logic.FinishClassroom();
            logic.FinishDungeon();

            logic.ReturnToTitle();
            logic.StartGame();

            Assert.AreEqual(GamePhase.Classroom, logic.Phase);
            Assert.AreEqual(0, logic.CurrentEraIndex);
        }

        [Test]
        public void 状態の変化が順に通知される()
        {
            var logic = Create(1);

            logic.StartGame();
            logic.FinishClassroom();
            logic.FinishDungeon();
            logic.ReturnToTitle();

            CollectionAssert.AreEqual(
                new[] { GamePhase.Classroom, GamePhase.Dungeon, GamePhase.Ending, GamePhase.Title },
                _phases);
        }

        [Test]
        public void 順番を飛ばした操作は例外()
        {
            var logic = Create(2);

            Assert.Throws<InvalidOperationException>(logic.FinishClassroom);
            Assert.Throws<InvalidOperationException>(logic.FinishDungeon);
            Assert.Throws<InvalidOperationException>(logic.ReturnToTitle);

            logic.StartGame();
            Assert.Throws<InvalidOperationException>(logic.StartGame);
        }

        private GameProgressLogic CreateAtEndingWithMistake()
        {
            var logic = Create(1);
            logic.StartGame();
            logic.FinishClassroom();
            logic.Mistakes.Record(logic.CurrentEra.Questions[0]);
            logic.FinishDungeon();
            return logic;
        }

        [Test]
        public void 間違いがあればエンディングから復習へ進める()
        {
            var logic = CreateAtEndingWithMistake();

            logic.StartReview();

            Assert.AreEqual(GamePhase.Review, logic.Phase);
            Assert.AreEqual(1, logic.Mistakes.Count);
        }

        [Test]
        public void 復習を終えると記録が空になりエンディングへ戻る()
        {
            var logic = CreateAtEndingWithMistake();
            logic.StartReview();

            logic.FinishReview();

            Assert.AreEqual(GamePhase.Ending, logic.Phase);
            Assert.AreEqual(0, logic.Mistakes.Count);
            Assert.IsTrue(logic.HasReviewed);
        }

        [Test]
        public void 復習済みの印はタイトルへ戻ると消える()
        {
            var logic = CreateAtEndingWithMistake();
            logic.StartReview();
            logic.FinishReview();

            logic.ReturnToTitle();

            Assert.IsFalse(logic.HasReviewed);
        }

        [Test]
        public void 間違いが無ければ復習は始められない()
        {
            var logic = Create(1);
            logic.StartGame();
            logic.FinishClassroom();
            logic.FinishDungeon();

            Assert.Throws<InvalidOperationException>(logic.StartReview);
        }

        [Test]
        public void 復習はエンディング以外では始められず終えられない()
        {
            var logic = Create(1);

            Assert.Throws<InvalidOperationException>(logic.StartReview);
            Assert.Throws<InvalidOperationException>(logic.FinishReview);
        }

        [Test]
        public void タイトルへ戻ると間違いの記録が空になる()
        {
            var logic = CreateAtEndingWithMistake();

            logic.ReturnToTitle();

            Assert.AreEqual(0, logic.Mistakes.Count);
        }

        [Test]
        public void 新しく始めると前回の間違いは残らない()
        {
            var logic = CreateAtEndingWithMistake();
            logic.ReturnToTitle();
            logic.Mistakes.Record(TestData.Question(9));

            logic.StartGame();

            Assert.AreEqual(0, logic.Mistakes.Count);
        }
    }
}
