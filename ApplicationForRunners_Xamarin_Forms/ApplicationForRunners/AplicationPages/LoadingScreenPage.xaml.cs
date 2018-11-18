using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingScreenPage : ContentPage
    {
        public LoadingScreenPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
            //return base.OnBackButtonPressed();
        }
    }
}