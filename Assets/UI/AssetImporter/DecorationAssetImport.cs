using StoreAsset;

namespace UI.AssetImporter
{
    public class DecorationAssertImport : AssetImporter
    {
        public void AddAsset()
        {
            ImportAsset();
        }

        protected override void AddDataToAsset(StoreAssetData assetData)
        {
        }
    }
}