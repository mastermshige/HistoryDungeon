using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoryDungeon.Logic
{
    /// <summary>年表の1項目（年号と出来事）。</summary>
    public sealed class TimelineEntry
    {
        public string Year { get; }
        public string Event { get; }

        public TimelineEntry(string year, string @event)
        {
            if (string.IsNullOrWhiteSpace(year))
                throw new ArgumentException("年号が空です。", nameof(year));
            if (string.IsNullOrWhiteSpace(@event))
                throw new ArgumentException("出来事が空です。", nameof(@event));

            Year = year;
            Event = @event;
        }
    }

    /// <summary>1つの時代のデータ（解説・年表・ダンジョンの問題）。</summary>
    public sealed class Era
    {
        public string Name { get; }
        public IReadOnlyList<string> ExplanationPages { get; }
        public IReadOnlyList<TimelineEntry> Timeline { get; }
        public IReadOnlyList<QuizQuestion> Questions { get; }

        public Era(
            string name,
            IReadOnlyList<string> explanationPages,
            IReadOnlyList<TimelineEntry> timeline,
            IReadOnlyList<QuizQuestion> questions)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("時代名が空です。", nameof(name));
            if (explanationPages == null || explanationPages.Count == 0)
                throw new ArgumentException("解説ページがありません。", nameof(explanationPages));
            if (timeline == null)
                throw new ArgumentNullException(nameof(timeline));
            if (questions == null || questions.Count == 0)
                throw new ArgumentException("問題がありません。", nameof(questions));

            Name = name;
            ExplanationPages = explanationPages.ToArray();
            Timeline = timeline.ToArray();
            Questions = questions.ToArray();
        }
    }
}
