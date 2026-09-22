using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HistoryDungeon.View
{
    /// <summary>4択の1つ分のボタン。押されたことを番号つきで知らせるだけで、正誤は判断しない。</summary>
    public sealed class ChoiceButtonView : MonoBehaviour
    {
        private Button _button;
        private TextMeshProUGUI _label;
        private int _index;

        public event Action<int> Clicked;

        public void Initialize(int index, Button button, TextMeshProUGUI label)
        {
            _index = index;
            _button = button;
            _label = label;
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void SetText(string text)
        {
            _label.text = text;
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(_index);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
