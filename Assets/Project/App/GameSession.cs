using HistoryDungeon.Logic;
using UnityEngine;

namespace HistoryDungeon.App
{
    /// <summary>シーンをまたいで引き継ぐゲーム全体の進行状況と、共通の名前。</summary>
    public static class GameSession
    {
        public const string GameName = "歴史年号ダンジョン";
        public const string ClassroomSceneName = "Classroom";
        public const string DungeonSceneName = "Dungeon";

        private static GameProgressLogic _progress;

        public static GameProgressLogic Progress => _progress ??= new GameProgressLogic(EraCatalog.All());

        // Enter Play Mode の設定によっては static が前回の再生から残るため、再生開始のたびに作り直す。
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlay()
        {
            _progress = null;
        }
    }
}
