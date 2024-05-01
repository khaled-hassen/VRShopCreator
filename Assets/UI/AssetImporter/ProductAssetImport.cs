using UnityEngine;
using UnityEngine.UI;

namespace UI.AssetImporter
{
    public class ProductAssetImport : UIScreen
    {
        [SerializeField] private GameObject specRowPrefab;
        [SerializeField] private GameObject contentContainer;
        private string _filePath;

        public void LoadUI(string filePath) => _filePath = filePath;

        public void OnNewSpecClicked()
        {
            var specRow = Instantiate(
                specRowPrefab,
                contentContainer.transform.position,
                contentContainer.transform.rotation,
                contentContainer.transform
            );

            var removeBtn = specRow.transform.Find("RemoveBtn").GetComponent<Button>();
            removeBtn.onClick.AddListener(() => Destroy(specRow));
        }
    }
}