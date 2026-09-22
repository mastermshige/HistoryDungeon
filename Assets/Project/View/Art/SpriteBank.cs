using System.Collections.Generic;
using UnityEngine;

namespace HistoryDungeon.View
{
    /// <summary>
    /// 画面で使うドット絵をまとめて持つ設定ファイル。絵の差し替えは、Inspector で対応する枠にスプライトをドラッグするだけで済む。
    /// 個別のオブジェクトにはスプライトを持たせない。
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteBank", menuName = "HistoryDungeon/Sprite Bank")]
    public sealed class SpriteBank : ScriptableObject
    {
        [Header("教室")]
        [SerializeField] private Sprite _classroomFloor;
        [SerializeField] private Sprite _teacher;

        [Header("ダンジョン")]
        [SerializeField] private Sprite _dungeonFloor;
        [SerializeField] private Sprite _dungeonWall;
        [SerializeField] private Sprite _dungeonBanner;
        [Tooltip("問題を出すキャラ。問題の数より少なくても、順番に使い回される")]
        [SerializeField] private Sprite[] _dungeonCharacters;

        public Sprite ClassroomFloor => Require(_classroomFloor, nameof(_classroomFloor));
        public Sprite Teacher => Require(_teacher, nameof(_teacher));
        public Sprite DungeonFloor => Require(_dungeonFloor, nameof(_dungeonFloor));
        public Sprite DungeonWall => Require(_dungeonWall, nameof(_dungeonWall));
        public Sprite DungeonBanner => Require(_dungeonBanner, nameof(_dungeonBanner));

        public IReadOnlyList<Sprite> DungeonCharacters
        {
            get
            {
                if (_dungeonCharacters == null || _dungeonCharacters.Length == 0)
                    throw new System.InvalidOperationException("SpriteBank の _dungeonCharacters が1枚も設定されていません。");

                return _dungeonCharacters;
            }
        }

        private Sprite Require(Sprite sprite, string fieldName)
        {
            if (sprite == null)
                throw new System.InvalidOperationException($"SpriteBank の {fieldName} が設定されていません。");

            return sprite;
        }
    }
}
