using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;

namespace AppUITest
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void BasicAppTest()
        {
            for (int i = 0; i < 10; i++) {
                try{
                    app.Tap(c => c.Button("Facebook"));
                }
                catch{
                    Thread.Sleep(50);
                    continue;
                }
                break;
            }

            int startButton = 22;
            app.Tap(c => c.Button("NoResourceEntry-"+ startButton));
            Thread.Sleep(12000);        
            app.Tap(c => c.Button("NoResourceEntry-"+ (startButton+1)));
            Thread.Sleep(3000);
            app.Tap(c => c.Button("NoResourceEntry-"+ startButton));
            app.Tap(c => c.Button("NoResourceEntry-"+ (startButton+1)));
            app.Tap(c => c.Button("NoResourceEntry-"+ (startButton+2)));
            app.Tap(c => c.Text("Add Path"));
            //app.Tap(c => c.Text("Delete"));
            //app.Tap(c => c.Text("Yes"));
            app.Tap(c => c.Text("Add"));
            app.Tap(c => c.Text("Workout"));
            app.Tap(c => c.Text("History"));
            Thread.Sleep(3000);
            app.Tap(c => c.Text("History"));
            app.Tap(c => c.Text("Options"));
            try
            {
                app.Tap(c => c.Marked("NoResourceEntry-57"));
            }
            catch { }   
            app.Tap(c => c.Text("Options"));
            app.Tap(c => c.Text("Workout"));
            app.Tap(c => c.Marked("0.00"));
            app.Tap(c => c.Text("Tempo"));
        }


    }
}

