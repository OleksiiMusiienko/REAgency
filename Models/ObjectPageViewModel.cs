using Org.BouncyCastle.Utilities;

namespace REAgency.Models
{
    public class ObjectPageViewModel
    {
        public IEnumerable<ObjectsViewModel> Objects { get; }
        public PageViewModel PageViewModel { get; }
        public string typeOfAction { get; set; }
        public ObjectPageViewModel(IEnumerable<ObjectsViewModel> objects, PageViewModel viewModel)
        {
            Objects = objects;
            PageViewModel = viewModel;
            
        }
    }
}
