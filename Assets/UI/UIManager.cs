using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float hiddenThresholdAngle = 20f;
        [SerializeField] private float cameraDistance = 1.5f;
        [SerializeField] private float centerDistanceMargin = 1f;
        [SerializeField] private float centerTime = 0.3f;
        [SerializeField] private float uiYPosition = 2f;
        [SerializeField] private UIScreen fileExplorerPrefab;
        [SerializeField] private InputActionProperty menuInputAction;

        private bool _isCentered = true;
        private bool _isMenuOpen;
        private UIScreen _menuScreen;
        private Vector3 _velocity = Vector3.zero;

        private void Awake() => menuInputAction.action.performed += OnMenuButtonPressed;

        private void Update()
        {
            if (!_isMenuOpen || _menuScreen is null) return;
            CenterUI();
        }

        private void CenterUI()
        {
            Vector3 targetPosition = CalculateNewPosition();
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, centerTime);

            var lookAtPos = new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z);
            transform.LookAt(lookAtPos);


            // if (!CheckIfUIIsVisible()) _isCentered = false;
            // if (_isCentered) return;
            //
            // Vector3 newPosition = CalculateNewPosition();
            // float distance = Vector3.Distance(transform.position, newPosition);
            // if (distance > centerDistanceMargin)
            // {
            //     transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _velocity, centerTime);
            // }
            // else _isCentered = true;
        }

        private Vector3 CalculateNewPosition()
        {
            Transform cameraTransform = mainCamera.transform;
            Vector3 newPosition = cameraTransform.position + cameraTransform.forward * cameraDistance;
            newPosition.y = uiYPosition;
            return newPosition;
        }

        private bool CheckIfUIIsVisible()
        {
            Vector3 cameraForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            Vector3 uiForwardVector = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            float angle = Vector3.Angle(cameraForwardVector, uiForwardVector);
            return angle < hiddenThresholdAngle;
        }

        private void OnMenuButtonPressed(InputAction.CallbackContext context)
        {
            if (_isMenuOpen) CloseMenu();
            else OpenMenu();
        }

        private void OpenMenu()
        {
            UIScreen screen = Instantiate(fileExplorerPrefab, Vector3.zero, Quaternion.identity, transform);
            Vector3 localPosition = screen.transform.localPosition;
            localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            screen.transform.localPosition = localPosition;
            screen.LoadUI();
            _menuScreen = screen;
            _isMenuOpen = true;
            CenterUI();
        }

        private void CloseMenu()
        {
            if (_menuScreen) Destroy(_menuScreen.gameObject);
            _isMenuOpen = false;
            _menuScreen = null;
        }
    }
}