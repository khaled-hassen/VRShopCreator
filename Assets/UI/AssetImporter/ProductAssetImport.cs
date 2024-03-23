using UnityEngine;
using UnityEngine.UI;

namespace UI.AssetImporter
{
    public class ProductAssetImport : UIScreen
    {
        [SerializeField] private GameObject specRowPrefab;
        [SerializeField] private GameObject contentContainer;

        public override void LoadUI()
        {
        }

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