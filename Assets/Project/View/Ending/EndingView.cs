using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HistoryDungeon.View
{
    /// <summary>
    /// エンディング画面。ダンジョンを背景に、金色の題名とみんなでお祝いする先生・キャラの行列を見せる。
    /// 「復習する」「タイトルへ」が押されたことを知らせるだけで、その後どうするかは判断しない。
    /// </summary>
    public sealed class EndingView : MonoBehaviour
    {
        private const float CastY = -370f;
        private const float CastSize = 180f;
        private const float ShadeAlpha = 0.5f;

        private static readonly Color TitleColor = new Color(1f, 0.85f, 0.35f);
        private static readonly Color ButtonColor = new Color(0.30f, 0.45f, 0.75f);
        private static readonly Color ReviewButtonColor = new Color(0.80f, 0.45f, 0.20f);

        private RectTransform _root;
        private CastRow _cast;
        private TextMeshProUGUI _title;
        private TextMeshProUGUI _body;
        private Button _returnButton;
        private Button _reviewButton;
        private TextMeshProUGUI _reviewLabel;

        public event Action ReturnRequested;
        public event Action ReviewRequested;

        public void Initialize(TMP_FontAsset font, SpriteBank sprites)
        {
            if (_root != null)
                throw new InvalidOperationException("すでに初期化されています。");

            var builder = new UiBuilder(font);
            UiBuilder.EnsureEventSystem();
            _root = builder.CreateCanvas(transform, "EndingCanvas", 10);

            DungeonBackdrop.Build(builder, _root, sprites, ShadeAlpha);
            _cast = new CastRow(builder, _root, sprites, CastY, CastSize);

            _title = builder.CreateText("Title", _root, 130f, TitleColor, TextAlignmentOptions.Center);
            UiBuilder.Place(_title.rectTransform, new Vector2(0f, 270f), new Vector2(1800f, 220f));

            _body = builder.CreateText("Body", _root, 52f, Color.white, TextAlignmentOptions.Center);
            UiBuilder.Place(_body.rectTransform, new Vector2(0f, 105f), new Vector2(1700f, 200f));

            _reviewButton = builder.CreateButton(
                "ReviewButton", _root, ReviewButtonColor, "", 48f, new Vector2(0f, -55f), new Vector2(1000f, 110f), out _reviewLabel);
            _reviewButton.onClick.AddListener(OnReviewClicked);

            _returnButton = builder.CreateButton(
                "ReturnButton", _root, ButtonColor, "タイトルへ", 56f, new Vector2(0f, -185f), new Vector2(520f, 110f), out _);
            _returnButton.onClick.AddListener(OnReturnClicked);
        }

        /// <param name="reviewButtonLabel">復習ボタンの文字。null のときは復習ボタンを隠す。</param>
        public void Show(string title, string body, string reviewButtonLabel)
        {
            _title.text = title;
            _body.text = body;
            _reviewButton.gameObject.SetActive(reviewButtonLabel != null);
            if (reviewButtonLabel != null)
                _reviewLabel.text = reviewButtonLabel;
            _root.gameObject.SetActive(true);
            _cast.StartBobbing();
        }

        public void Hide()
        {
            _cast?.StopBobbing();
            _root.gameObject.SetActive(false);
        }

        private void OnReturnClicked()
        {
            ReturnRequested?.Invoke();
        }

        private void OnReviewClicked()
        {
            ReviewRequested?.Invoke();
        }

        private void OnDestroy()
        {
            _cast?.StopBobbing();

            if (_returnButton != null)
                _returnButton.onClick.RemoveListener(OnReturnClicked);
            if (_reviewButton != null)
                _reviewButton.onClick.RemoveListener(OnReviewClicked);
        }
    }
}
