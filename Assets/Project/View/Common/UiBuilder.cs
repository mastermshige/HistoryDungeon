using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace HistoryDungeon.View
{
    /// <summary>仮素材の画面（色つきの四角と文字）を組み立てる共通部品。座標は 1920x1080 の中心を原点とする。</summary>
    internal sealed class UiBuilder
    {
        private const float ReferenceWidth = 1920f;
        private const float ReferenceHeight = 1080f;
        private const float ReferencePixelsPerUnit = 100f; // Canvas の Reference Pixels Per Unit の既定値

        private readonly TMP_FontAsset _font;

        public UiBuilder(TMP_FontAsset font)
        {
            _font = font != null ? font : throw new ArgumentNullException(nameof(font));
        }

        public static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null)
                return;

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        public RectTransform CreateCanvas(Transform parent, string objectName, int sortingOrder)
        {
            var canvasObject = new GameObject(objectName);
            canvasObject.transform.SetParent(parent, false);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            return (RectTransform)canvasObject.transform;
        }

        public RectTransform CreatePanel(string objectName, RectTransform parent, Color color)
        {
            var panel = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return (RectTransform)panel.transform;
        }

        public TextMeshProUGUI CreateText(
            string objectName, RectTransform parent, float size, Color color, TextAlignmentOptions alignment)
        {
            var textObject = new GameObject(objectName, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.font = _font;
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = FontStyles.Bold;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.raycastTarget = false;
            return text;
        }

        public Button CreateButton(
            string objectName, RectTransform parent, Color color, string label, float fontSize,
            Vector2 position, Vector2 size, out TextMeshProUGUI labelText)
        {
            var rect = CreatePanel(objectName, parent, color);
            Place(rect, position, size);

            var image = rect.GetComponent<Image>();
            image.raycastTarget = true;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            labelText = CreateText("Label", rect, fontSize, Color.white, TextAlignmentOptions.Center);
            Stretch(labelText.rectTransform, 10f);
            labelText.text = label;
            return button;
        }

        /// <summary>ドット絵を1枚、くっきりしたまま表示する。</summary>
        public RectTransform CreateSprite(string objectName, RectTransform parent, Sprite sprite)
        {
            var rect = CreatePanel(objectName, parent, Color.white);
            var image = rect.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            return rect;
        }

        /// <summary>1枚のドット絵を敷き詰める。tileSize は画面上で1マスを何ピクセルで見せるか（1920x1080 基準）。</summary>
        public void TileSprite(RectTransform panel, Sprite sprite, float tileSize)
        {
            var image = panel.GetComponent<Image>();
            image.color = Color.white;
            image.sprite = sprite;
            image.type = Image.Type.Tiled;
            image.pixelsPerUnitMultiplier = sprite.rect.width * ReferencePixelsPerUnit / (sprite.pixelsPerUnit * tileSize);
        }

        public static void Place(RectTransform rect, Vector2 anchoredPosition, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        public static void Stretch(RectTransform rect, float margin = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(margin, margin);
            rect.offsetMax = new Vector2(-margin, -margin);
        }
    }
}
