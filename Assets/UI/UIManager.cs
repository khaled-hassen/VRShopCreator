using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Player.Controllers.Scripts;
using StoreAsset;
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
        [SerializeField] private float uiYPosition = 1.4f;
        [SerializeField] private float windowTiltAngle = 20f;
        [SerializeField] private FileExplorer.FileExplorer fileExplorerPrefab;
        [SerializeField] private AssetsExplorer.AssetsExplorer assetsExplorerPrefab;
        [SerializeField] private ProductAssetImport productAssetImportPrefab;
        [SerializeField] private DecorationAssertImport decorationAssetImportPrefab;
        [SerializeField] private InputActionProperty menuInputAction;
        [SerializeField] private XRInteractionManager interactionManager;
        [SerializeField] private int gapBetweenScreens = 30;
        private readonly List<UIScreen> _activeScreens = new();

        private UIScreen _assetImportScreen;
        private UIScreen _assetsExplorerScreen;
        private UIScreen _decorationImportScreen;
        private UIScreen _fileExplorerScreen;
        private Vector3 _velocity = Vector3.zero;

        private void Awake()
        {
            menuInputAction.action.performed += OnMenuButtonPressed;
        }

        private void Update()
        {
            if (_activeScreens.Count == 0) return;
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
            if (_assetsExplorerScreen)
            {
                CloseWindowScreen(ref _assetsExplorerScreen);
            }
            else
            {
                OpenAssetsExplorer();
                CenterUI();
            }
        }

        private void RearrangeScreens()
        {
            if (_activeScreens.Count == 0) return;

            var previousPosition = Vector3.zero;
            var totalWidth = 0f;
            RectTransform prevRect = null;

            foreach (var screen in _activeScreens)
            {
                var xTranslate = 0f;

                var screenRect = screen.GetComponent<RectTransform>();
                if (prevRect is not null)
                {
                    var localScaleX = prevRect.localScale.x;
                    var prevHalfWidth = prevRect.rect.width / 2;
                    var halfWidth = screenRect.rect.width / 2;
                    xTranslate = (halfWidth + prevHalfWidth + gapBetweenScreens) * localScaleX;
                }

                screen.transform.localPosition = previousPosition + Vector3.right * xTranslate;
                previousPosition = screen.transform.localPosition;
                prevRect = screenRect;
                totalWidth += prevRect.rect.width + gapBetweenScreens;
            }

            totalWidth -= gapBetweenScreens + _activeScreens.First().GetComponent<RectTransform>().rect.width;

            foreach (var screen in _activeScreens) screen.transform.localPosition += Vector3.left * screen.transform.localScale.x * (totalWidth / 2);
        }

        private void OpenAssetsExplorer()
        {
            var screen = Instantiate(assetsExplorerPrefab, transform.position, transform.rotation, transform);
            screen.LoadUI();
            _assetsExplorerScreen = screen;
            _activeScreens.Insert(0, _assetsExplorerScreen);
            RearrangeScreens();

            // listen to events
            screen.OnCloseWindow += () => CloseWindowScreen(ref _assetsExplorerScreen);
            screen.OnOpenAddNewAssetScreen += OpenFileExplorer;
            screen.OnEditItem += OpenEditScreen;
        }

        private void OpenFileExplorer()
        {
            var screen = Instantiate(fileExplorerPrefab, transform.position, transform.rotation, transform);
            screen.LoadUI();
            _fileExplorerScreen = screen;
            _activeScreens.Insert(_assetsExplorerScreen is null ? 0 : 1, screen);
            RearrangeScreens();

            // listen to events
            screen.OnCloseWindow += () => CloseWindowScreen(ref _fileExplorerScreen);
            screen.OnImportAsset += path => OpenImportProductAssetMenu(path);
        }

        private void CloseWindowScreen(ref UIScreen screen)
        {
            DestroyScreen(ref screen);
            RearrangeScreens();

            interactionManager.EnableTeleportation();
            interactionManager.EnableContinuousMovement();
        }

        private void DestroyScreen(ref UIScreen screen)
        {
            Destroy(screen.gameObject);
            _activeScreens.Remove(screen);
            screen = null;
        }

        private void OpenImportProductAssetMenu(string filePath, [CanBeNull] StoreAssetData assetData = null)
        {
            if (filePath is null && assetData is null) return;

            if (_assetImportScreen is not null) DestroyScreen(ref _assetImportScreen);
            if (_decorationImportScreen is not null) DestroyScreen(ref _decorationImportScreen);

            var screen = Instantiate(productAssetImportPrefab, transform.position, transform.rotation, transform);
            if (assetData is null) screen.LoadUI(filePath);
            else screen.LoadUI(assetData);
            _assetImportScreen = screen;
            _activeScreens.Add(screen);
            RearrangeScreens();

            // listen to events
            screen.OnCloseWindow += () => CloseWindowScreen(ref _assetImportScreen);
            screen.OnTabClicked += path => OpenImportDecorationAssetMenu(path);
            screen.OnAssetImported += () => (_assetsExplorerScreen as AssetsExplorer.AssetsExplorer)?.UpdateUI();
        }

        private void OpenImportDecorationAssetMenu(string filePath, [CanBeNull] StoreAssetData assetData = null)
        {
            if ((filePath is null && assetData is null) || _decorationImportScreen is not null) return;
            if (_assetImportScreen is not null) DestroyScreen(ref _assetImportScreen);

            var screen = Instantiate(decorationAssetImportPrefab, transform.position, transform.rotation, transform);
            if (assetData is null) screen.LoadUI(filePath);
            else screen.LoadUI(assetData);
            _decorationImportScreen = screen;
            _activeScreens.Add(screen);
            RearrangeScreens();

            // listen to events
            screen.OnCloseWindow += () => CloseWindowScreen(ref _decorationImportScreen);
            screen.OnTabClicked += path => OpenImportProductAssetMenu(path);
            screen.OnAssetImported += () => (_assetsExplorerScreen as AssetsExplorer.AssetsExplorer)?.UpdateUI();
        }

        private void OpenEditScreen(StoreAssetData data)
        {
            if (data.assetSpecs.Count == 0) OpenImportDecorationAssetMenu(null, data);
            else OpenImportProductAssetMenu(null, data);
        }
    }
}