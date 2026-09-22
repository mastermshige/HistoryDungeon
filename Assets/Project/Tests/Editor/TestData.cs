using System.Collections.Generic;
using HistoryDungeon.Logic;

namespace HistoryDungeon.Tests
{
    /// <summary>テスト用のダミーデータ生成（実際の問題内容とは無関係）。</summary>
    internal static class TestData
    {
        public static QuizQuestion Question(int number)
        {
            return new QuizQuestion(
                $"キャラ{number}",
                $"問題{number}",
                $"正解{number}",
                new[] { $"誤A{number}", $"誤B{number}", $"誤C{number}" },
                $"解説{number}");
        }

        public static List<QuizQuestion> Questions(int count)
        {
            var list = new List<QuizQuestion>();
            for (var i = 1; i <= count; i++)
                list.Add(Question(i));
            return list;
        }

        public static Era Era(string name, int questionCount)
        {
            return new Era(
                name,
                new[] { $"{name}の解説1", $"{name}の解説2" },
                new[] { new TimelineEntry("紀元前", $"{name}の出来事") },
                Questions(questionCount));
        }
    }
}
