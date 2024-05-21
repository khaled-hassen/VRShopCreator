using System;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Keyboard : MonoBehaviour
    {
        private const float Distance = 0.8f;
        private const float VerticalOffset = -0.4f;
        private TMP_InputField _inputField;
        private bool _isKeyboardOpen;
        private Transform _positionSource;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onSelect.AddListener(_ => OpenKeyboard());
            if (Camera.main != null) _positionSource = Camera.main.transform;
        }

        private void Update()
        {
            if (!_isKeyboardOpen) return;
            var direction = _positionSource.forward;
            direction.y = 0;
            direction.Normalize();
            var targetPosition = _positionSource.position + direction * Distance + Vector3.up * VerticalOffset;
            NonNativeKeyboard.Instance.RepositionKeyboard(targetPosition);
        }

        private void OpenKeyboard()
        {
            NonNativeKeyboard.Instance.Close();

            NonNativeKeyboard.Instance.InputField = _inputField;
            NonNativeKeyboard.Instance.PresentKeyboard(_inputField.text);

            SetCursorIndicatorAlpha(1);
            NonNativeKeyboard.Instance.OnClosed += CloseKeyboard;

            _isKeyboardOpen = true;
        }

        private void CloseKeyboard(object _, EventArgs __)
        {
            SetCursorIndicatorAlpha(0);
            NonNativeKeyboard.Instance.OnClosed -= CloseKeyboard;
            _isKeyboardOpen = false;
        }

        public void SetCursorIndicatorAlpha(float alpha)
        {
            _inputField.customCaretColor = true;
            var color = _inputField.caretColor;
            color.a = alpha;
            _inputField.caretColor = color;
        }
    }
}