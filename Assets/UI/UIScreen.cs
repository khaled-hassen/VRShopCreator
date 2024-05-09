using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] protected TMP_FontAsset font;
        public event Action OnCloseWindow;

        public void OnCloseWindowClick() => OnCloseWindow?.Invoke();
    }
}