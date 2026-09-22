using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HistoryDungeon.View
{
    /// <summary>先生を中央に、左右3体ずつキャラが並ぶ行列。上下にゆらして見せる。タイトル・エンディングで共通。</summary>
    internal sealed class CastRow
    {
        private const float Spacing = 240f;
        private const float BobHeight = 18f;
        private const float BobSeconds = 0.55f;

        // 先生の左右に並べるキャラの番号（SpriteBank の並び順）。種類が偏らないよう人物とモンスターを混ぜる
        private static readonly int[] LeftCast = { 15, 11, 2 };
        private static readonly int[] RightCast = { 7, 10, 14 };

        private readonly List<RectTransform> _members = new List<RectTransform>();
        private readonly float _y;

        public CastRow(UiBuilder builder, RectTransform parent, SpriteBank sprites, float y, float size)
        {
            _y = y;
            var characters = sprites.DungeonCharacters;

            Add(builder, parent, sprites.Teacher, 0f, size);
            for (var i = 0; i < LeftCast.Length; i++)
            {
                var step = (i + 1) * Spacing;
                Add(builder, parent, characters[LeftCast[i] % characters.Count], -step, size);
                Add(builder, parent, characters[RightCast[i] % characters.Count], step, size);
            }
        }

        public void StartBobbing()
        {
            StopBobbing();

            for (var i = 0; i < _members.Count; i++)
            {
                _members[i].DOLocalMoveY(_y + BobHeight, BobSeconds)
                    .SetDelay(i * 0.1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void StopBobbing()
        {
            foreach (var member in _members)
            {
                member.DOKill();
                var position = member.localPosition;
                member.localPosition = new Vector3(position.x, _y, position.z);
            }
        }

        private void Add(UiBuilder builder, RectTransform parent, Sprite sprite, float x, float size)
        {
            var member = builder.CreateSprite("Cast", parent, sprite);
            UiBuilder.Place(member, new Vector2(x, _y), new Vector2(size, size));
            _members.Add(member);
        }
    }
}
