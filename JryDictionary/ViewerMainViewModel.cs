using Jasily.ComponentModel;

namespace JryDictionary
{
    public class ViewerMainViewModel : MainViewModel
    {
        #region Overrides of MainViewModel

        public override MainViewModelType ViewModelType => MainViewModelType.Viewer;
        
        public override string WindowTitle => this.Searched ? $"jry dictionary ({this.Things.Count}{(this.HasNext ? "+" : "")})" : "jry dictionary";

        #endregion
    }
}