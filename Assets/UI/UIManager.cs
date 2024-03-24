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
        [SerializeField] private int gapBetweenScreens = 30;

        private UIScreen _assetImportScreen;
        private UIScreen _mainMenuScreen;
        private Vector3 _velocity = Vector3.zero;

        private void Awake()
        {
            menuInputAction.action.performed += OnMenuButtonPressed;
        }

        private void Update()
        {
            if (_mainMenuScreen is null && _assetImportScreen is null) return;
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
            if (_mainMenuScreen)
            {
                CloseWindowScreen(ref _mainMenuScreen);
            }
            else
            {
                OpenFileExplorer();
                CenterUI();
            }
        }

        private void OpenFileExplorer()
        {
            var screen = Instantiate(fileExplorerPrefab, transform.position, transform.rotation, transform);
            screen.LoadUI();
            _mainMenuScreen = screen;
            RearrangeScreens();

            // listen to events
            screen.OnCloseWindow += () => CloseWindowScreen(ref _mainMenuScreen);
            screen.OnImportAsset += OpenImportAssetMenu;
        }

        private void CloseWindowScreen(ref UIScreen screen)
        {
            Destroy(screen.gameObject);
            screen = null;
            RearrangeScreens();

            interactionManager.EnableTeleportation();
            interactionManager.EnableContinuousMovement();
        }

        private void OpenImportAssetMenu(string filePath)
        {
            if (filePath is null || _assetImportScreen is not null) return;

            var screen = Instantiate(productAssetImportPrefab, transform.position, transform.rotation, transform);
            screen.LoadUI(filePath);
            _assetImportScreen = screen;
            RearrangeScreens();

            // listen to events
            screen.OnCloseWindow += () => CloseWindowScreen(ref _assetImportScreen);
        }

        private void RearrangeScreens()
        {
            if (_mainMenuScreen != null)
                _mainMenuScreen.transform.position = CalculateNewPosition(_mainMenuScreen, _assetImportScreen, Vector3.left);

            if (_assetImportScreen != null)
                _assetImportScreen.transform.position = CalculateNewPosition(_assetImportScreen, _mainMenuScreen, Vector3.right);
        }

        private Vector3 CalculateNewPosition(UIScreen currentScreen, UIScreen otherScreen, Vector3 direction)
        {
            if (otherScreen is null) return transform.position;

            var rectangle = currentScreen.GetComponent<RectTransform>();
            var halfWidth = rectangle.rect.width / 2;
            var xTranslate = (halfWidth + gapBetweenScreens) * rectangle.localScale.x;
            return rectangle.position + direction * xTranslate;
        }
    }
}