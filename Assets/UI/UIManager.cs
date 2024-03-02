using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<UIWindowData> windows;
        private UIWindowData _currentWindow;


        // Start is called before the first frame update
        private void Start()
        {
            // TODO just for testing
            UIScreen screen = Instantiate(windows[0].screen, new Vector3(-0.102f, 1.095f, 0.897f), Quaternion.identity);
            screen.LoadUI();
        }
    }
}