using System;
using System.Collections.Generic;

namespace HistoryDungeon.Logic
{
    /// <summary>1回のプレイ中に間違えた問題の記録。同じ問題を何度間違えても1件として数え、出会った順に並べる。</summary>
    public sealed class MistakeLog
    {
        private readonly List<QuizQuestion> _questions = new List<QuizQuestion>();

        public int Count => _questions.Count;
        public IReadOnlyList<QuizQuestion> Questions => _questions;

        public void Record(QuizQuestion question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (!_questions.Contains(question))
                _questions.Add(question);
        }

        public void Clear()
        {
            _questions.Clear();
        }
    }
}
