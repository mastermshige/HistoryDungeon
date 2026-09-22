using UnityEngine;

namespace HistoryDungeon.View
{
    /// <summary>ダンジョンの背景（床・暗がり・石壁・旗）を組み立てる。ダンジョン・タイトル・エンディングで共通。</summary>
    internal static class DungeonBackdrop
    {
        private const float TileSize = 96f;
        private const float WallY = 490f;
        private static readonly float[] BannerPositions = { -720f, 0f, 720f };
        private static readonly Color BaseColor = new Color(0.10f, 0.08f, 0.16f);

        /// <param name="shadeAlpha">床を暗くする度合い（0〜1）。文字を載せる画面ほど大きくする。</param>
        public static void Build(UiBuilder builder, RectTransform parent, SpriteBank sprites, float shadeAlpha)
        {
            var floor = builder.CreatePanel("Floor", parent, BaseColor);
            UiBuilder.Stretch(floor);
            builder.TileSprite(floor, sprites.DungeonFloor, TileSize);

            UiBuilder.Stretch(builder.CreatePanel("Shade", parent, new Color(0.05f, 0.03f, 0.10f, shadeAlpha)));

            var wall = builder.CreatePanel("Wall", parent, BaseColor);
            UiBuilder.Place(wall, new Vector2(0f, WallY), new Vector2(1920f, TileSize));
            builder.TileSprite(wall, sprites.DungeonWall, TileSize);

            foreach (var x in BannerPositions)
            {
                var banner = builder.CreateSprite("Banner", parent, sprites.DungeonBanner);
                UiBuilder.Place(banner, new Vector2(x, WallY), new Vector2(TileSize, TileSize));
            }
        }
    }
}
