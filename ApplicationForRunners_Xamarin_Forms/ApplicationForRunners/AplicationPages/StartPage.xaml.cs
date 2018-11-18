using ApplicationForRunners.DataObjects;
using ApplicationForRunners.ItemManager;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.Connectivity;
using Plugin.Toasts;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage: CarouselPage
    {
        IToastNotificator notificator = DependencyService.Get<IToastNotificator>();
        NotificationOptions options = new NotificationOptions();

        public StartPage()
        {
            InitializeComponent();
            Children.Clear();
            Children.Add(LoginPageXaml1);

            this.CurrentPageChanged += DeleteLastPage;             
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
                     
            if (Children.Count() > 1)
            {             
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:                
                    case Device.Android:
                        CurrentPage = Children[Children.Count() - 2];
                        break;
                    case Device.UWP://Windows:
                        Children.Remove(Children.Last());
                        break;
                }

                return true;
            }
            else
                return false;        
        }

        private  void DeleteLastPage(object sender, EventArgs e)
        {          
            //remove all page from end to start, break when find currend page
            foreach (ContentPage element in this.Children.Reverse()) {

                if (CurrentPage != null && !CurrentPage.Equals(element))
                    Children.Remove(element);
                else
                    break;
            }                      
        }

        private void StartMainFunction()
        {
            App.Current.MainPage = new MainMDPage();      
        }


        public async void OnLoginFacebook(object sender, EventArgs e) {

            await LoginNextStep(MobileServiceAuthenticationProvider.Facebook);
        }
        public async void OnLoginGoogle(object sender, EventArgs e) {

            await LoginNextStep(MobileServiceAuthenticationProvider.Google);
        }
        public async void OnLoginTwitter(object sender, EventArgs e) {

            await LoginNextStep(MobileServiceAuthenticationProvider.Twitter);
        }

        public async void OnLoginMicrosoft(object sender, EventArgs e) {

            await LoginNextStep(MobileServiceAuthenticationProvider.MicrosoftAccount);

        }

        async Task LoginNextStep(MobileServiceAuthenticationProvider provider)
        {
            switch (Device.RuntimePlatform){
                case Device.Android:
                case Device.UWP:
                    await Navigation.PushModalAsync(new LoadingScreenPage());
                    break;
            }

            if (await DBConnection.MainConnection.LoginUser(provider) == true) {

                switch (Device.RuntimePlatform){
                    case Device.iOS:
                        await Navigation.PushModalAsync(new LoadingScreenPage());
                        break;
                }

                if (await DBConnection.MainConnection.AcountExist() == true)
                {
                    await Navigation.PopModalAsync();
                    StartMainFunction();
                }
                else
                {
                    options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Info);
                    options.Description = "Account don't exist";
                    await notificator.Notify(options);

                    await DBConnection.MainConnection.LogoutUser();
                    await Navigation.PopModalAsync();
                }

            }else
                await Navigation.PopModalAsync();
        }



        public async void GoRegistration1(object sender, EventArgs e)
        {
            Children.Add(RegistrationPageXaml1);
            
            await Task.Delay(10);
            this.CurrentPage = Children.Last();
        }


        public async void OnRegistrationFacebook(object sender, EventArgs e)
        {
            await RegistrationNextStep(MobileServiceAuthenticationProvider.Facebook);
        }
        public async void OnRegistrationGoogle(object sender, EventArgs e)
        {
            await RegistrationNextStep(MobileServiceAuthenticationProvider.Google);
        }
        public async void OnRegistrationTwitter(object sender, EventArgs e)
        {
            await RegistrationNextStep(MobileServiceAuthenticationProvider.Twitter);
        }

        public async void OnRegistrationMicrosoft(object sender, EventArgs e)
        {
            await RegistrationNextStep(MobileServiceAuthenticationProvider.MicrosoftAccount);
        }


        async Task RegistrationNextStep(MobileServiceAuthenticationProvider  provider) {

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                case Device.UWP:
                    await Navigation.PushModalAsync(new LoadingScreenPage());
                    break;
            }

            if (await DBConnection.MainConnection.LoginUser(provider, true) == true) {

                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        await Navigation.PushModalAsync(new LoadingScreenPage());
                        break;
                }

                if (await DBConnection.MainConnection.AcountExist() == false)
                {
                    await Navigation.PopModalAsync();
                    Children.Add(RegistrationPageXaml2);
                    await Task.Delay(10);
                    this.CurrentPage = Children.Last();

                }
                else {
                    options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Info);
                    options.Description = "Account already exist";
                    await notificator.Notify(options);
                    await DBConnection.MainConnection.LogoutUser();
                    await Navigation.PopModalAsync();
                }
            }else
                await Navigation.PopModalAsync();

        }

        public async void OnRegistration(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)            
                return;

            await Navigation.PushModalAsync(new LoadingScreenPage());

            UserItemManager userManager = DBConnection.MainConnection.UserItem;
            UserItem newUser = new UserItem();

            if (unitsSwitch.IsToggled == false) 
                Constants.AppUnitsType = "KM";
             else
                Constants.AppUnitsType = "MI";

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
            catch (Exception ex) {
                await DBConnection.MainConnection.LogoutUser();
                await Navigation.PopModalAsync();
                OnBackButtonPressed();
                var message = string.Format("{0}", ex.Message);
                options = Constants.NotificationStyle.Options(Constants.NotificationStyle.Which.Error);
                options.Description = message;
                await notificator.Notify(options);

                return;
            }

            await DBConnection.MainConnection.ConfirmUser();
            await Navigation.PopModalAsync();
            StartMainFunction();
        }
    }
}