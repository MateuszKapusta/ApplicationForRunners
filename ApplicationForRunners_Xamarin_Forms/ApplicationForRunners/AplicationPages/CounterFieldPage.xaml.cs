using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CounterFieldPage : ContentPage
    {
        public ObservableCollection<TypeOfConventer> Items { get; set; }
        public Guid CallObjId { get; private set; }

        public CounterFieldPage(Guid id)
        {
            InitializeComponent();

            CallObjId = id;

            Items = new ObservableCollection<TypeOfConventer>();

            foreach (TypeOfConventer val in WorkoutConventer.NrOfType()) 
                Items.Add(val);

          
            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {           
            if (e.SelectedItem == null)
                return;

            MessagingCenter.Send<CounterFieldPage, TypeOfConventer>(this, "ChangeWorkField", (TypeOfConventer)e.SelectedItem);
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
