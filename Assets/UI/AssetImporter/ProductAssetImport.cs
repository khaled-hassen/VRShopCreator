using UnityEngine;
using UnityEngine.UI;

namespace UI.AssetImporter
{
    public class ProductAssetImport : AssetImporter
    {
        [SerializeField] protected GameObject contentContainer;
        [SerializeField] protected GameObject specRowPrefab;

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