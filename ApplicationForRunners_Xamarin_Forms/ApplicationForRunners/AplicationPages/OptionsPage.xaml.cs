using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OptionsPage : ContentPage
	{
		public OptionsPage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Constants.AppUnitsType.Equals("KM"))
                unitsSwitch.IsToggled =false;
            else
                unitsSwitch.IsToggled = true;
        }


        public async void OnLogout(object sender, EventArgs e) {

            await Navigation.PushModalAsync(new LoadingScreenPage());

            if (await DBConnection.MainConnection.LogoutUser() == true) {
                await Navigation.PopModalAsync();
                App.Current.MainPage = new AplicationPages.StartPage(); 
            }else
                await Navigation.PopModalAsync();

        }

        private void OnToggledUnits(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var senderObj = (Switch)sender;

            if (senderObj.IsToggled)
                Constants.AppUnitsType = "MI";
            else
                Constants.AppUnitsType = "KM";

        }

        private async void OnChangeUserOpt(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserOptionsPgae());
        }
    }
}