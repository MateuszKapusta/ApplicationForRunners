using Plugin.Toasts.UWP;
using Xamarin.Forms;


namespace ApplicationForRunners.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
      
        public MainPage()
        {
            this.InitializeComponent();
            //init Maps
            Xamarin.FormsMaps.Init("AUTHORIZATION_TOKEN");

            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init();

            Constants.ApplicationActualPermissions = new MainPermissions();

            LoadApplication(new ApplicationForRunners.App());
        }
    }
}