using ApplicationForRunners.DataObjects;
using ApplicationForRunners.ItemManager;
using Plugin.Toasts;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserOptionsPgae : ContentPage
	{
        UserItemManager userManager = DBConnection.MainConnection.UserItem;


		public UserOptionsPgae ()
		{
			InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var userList= await userManager.GetItemsAsync();
            birthdayText.Date=userList.First().Birth;

            if (userList.First().Gender.Equals("F"))
                genderSwitch.IsToggled = true;
            else
                genderSwitch.IsToggled = false;

            weightText.Text = Convert.ToString( userList.First().Weight);
            heightText.Text = Convert.ToString(userList.First().Height);
        }


        // if error, bad entry data
        private async void OnSave(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoadingScreenPage());

            var userList = await userManager.GetItemsAsync();
            UserItem newUser = userList.First();

            newUser.Birth = birthdayText.Date;

            if (!string.IsNullOrEmpty(weightText.Text))
                newUser.Weight = Convert.ToDouble(weightText.Text);

            if (!string.IsNullOrEmpty(heightText.Text))
                newUser.Height = Convert.ToDouble(heightText.Text);

            if (genderSwitch.IsToggled == true)
                newUser.Gender = "F";

            try
            {
                await userManager.SaveItemAsync(newUser, true);
            }
            catch (Exception ex)
            {
                NotificationOptions options = new NotificationOptions();
                IToastNotificator notificator = DependencyService.Get<IToastNotificator>();
                var message = string.Format("{0}", ex.Message);
                options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Error);
                options.Description = message;
                await notificator.Notify(options);
            }

            await Navigation.PopModalAsync();
            await Navigation.PopAsync();
        }
    }
}