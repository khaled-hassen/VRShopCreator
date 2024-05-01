namespace UI.AssetImporter
{
    public abstract class AssetImporter : UIScreen
    {
        public delegate void OnTabClickedCallback(string filePath);

        protected string _filePath;
        public event OnTabClickedCallback OnTabClicked;

        public void OnTabClickedEvent() => OnTabClicked?.Invoke(_filePath);

        public virtual void LoadUI(string filePath) => _filePath = filePath;
    }
}