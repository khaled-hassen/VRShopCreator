using JetBrains.Annotations;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "New UIWindow", menuName = "UI/Window")]
    public class UIWindowData : ScriptableObject
    {
        public UIScreen screen;
        [CanBeNull] public UIScreen previous;
        [CanBeNull] public UIScreen next;
    }
}