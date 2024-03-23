using Player.Controllers.Scripts;
using UI.AssetImporter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float cameraDistance = 1.1f;
        [SerializeField] private float centerTime = 0.3f;
        [SerializeField] private float uiYPosition = 1.2f;
        [SerializeField] private float windowTiltAngle = 30f;
        [SerializeField] private FileExplorer.FileExplorer fileExplorerPrefab;
        [SerializeField] private ProductAssetImport productAssetImportPrefab;
        [SerializeField] private InputActionProperty menuInputAction;
        [SerializeField] private XRInteractionManager interactionManager;

        private bool _isMenuOpen;
        private UIScreen _menuScreen;
        private Vector3 _velocity = Vector3.zero;

        private void Awake()
        {
            menuInputAction.action.performed += OnMenuButtonPressed;
        }

        private void Update()
        {
            if (!_isMenuOpen || _menuScreen is null) return;
            CenterUI();
        }

        private void CenterUI()
        {
            var targetPosition = CalculateTargetPosition();
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, centerTime);

            transform.rotation = CalculateTargetRotation();
        }

        private Vector3 CalculateTargetPosition()
        {
            var cameraTransform = mainCamera.transform;
            var newPosition = cameraTransform.position + cameraTransform.forward * cameraDistance;
            newPosition.y = uiYPosition;
            return newPosition;
        }

        private Quaternion CalculateTargetRotation()
        {
            var lookDir = mainCamera.transform.position - transform.position;
            lookDir.y = 0f;
            var targetRotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            targetRotation *= Quaternion.Euler(windowTiltAngle, 180, 0);
            return targetRotation;
        }

        private void OnMenuButtonPressed(InputAction.CallbackContext context)
        {
            if (_isMenuOpen) CloseMenu();
            else OpenMenu();
        }

        private void OpenMenu()
        {
            var screen = Instantiate(fileExplorerPrefab, transform.position, transform.rotation, transform);
            screen.LoadUI();
            screen.OnCloseWindow += CloseMenu;
            _menuScreen = screen;
            _isMenuOpen = true;
            CenterUI();
        }

        private void CloseMenu()
        {
            if (_menuScreen) Destroy(_menuScreen.gameObject);
            _isMenuOpen = false;
            _menuScreen = null;
            interactionManager.EnableTeleportation();
            interactionManager.EnableContinuousMovement();
        }
    }
}