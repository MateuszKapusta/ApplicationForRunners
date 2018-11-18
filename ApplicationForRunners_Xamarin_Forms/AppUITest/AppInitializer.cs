using Xamarin.UITest;

namespace AppUITest
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android 
                    .ApkFile("..\\ApplicationForRunners_Xamarin_Forms\\Droid\\bin\\Debug\\com.xamarin.sample.ApplicationForRunners.apk")
                    .StartApp();
            }

            return ConfigureApp.iOS.AppBundle("..\\ApplicationForRunners_Xamarin_Forms\\iOS\\bin\\iPhoneSimulator\\Debug\\ApplicationForRunnersiOS.app").StartApp();
        }
    }
}