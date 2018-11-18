using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterMDPage : ContentPage
    {
        public ListView ListView;
        public MDMenuItem firstPage;

        public MasterMDPage()
        {
            InitializeComponent();

            BindingContext = new MainMDPageMasterViewModel();
            ListView = MenuItemsListView;

            firstPage= ((MainMDPageMasterViewModel)BindingContext).MenuItems.First();
        }

        class MainMDPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MDMenuItem> MenuItems { get; set; }

            public MainMDPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MDMenuItem>(new[]
                {
                    new MDMenuItem { Id = 0, Title = "Workout" ,TargetType=typeof(MapPage),OlwaysActive=true},
                    new MDMenuItem { Id = 1, Title = "History",TargetType=typeof(WorkoutListPage) },
                    new MDMenuItem { Id = 2, Title = "Options",TargetType=typeof(OptionsPage) },     
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}