using ApplicationForRunners.SharedClasses;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        MapPreparedData WorkoutData = new MapPreparedData();
        Plugin.Geolocator.Abstractions.Position  NewWorkoutPoint=null;

        public GLFlag MyGeoFlag { get; } = new GLFlag();
        public List<Plugin.Geolocator.Abstractions.Position> RouteCoordinates { get; set; }

        List<TypeOfConventer> WorkValue { get; set; } = new List<TypeOfConventer>() {TypeOfConventer.Timer,TypeOfConventer.Speed, TypeOfConventer.Distance, TypeOfConventer.MediumSpeed};
        List<Label> WorkField { get; set; } 

        AutoResetEvent autoEvent=null;
        Timer stateTimer = null;
        DateTime startWorkout;
        DateTime lastInterval= new DateTime(0);

        public MapPage()
        {
            InitializeComponent();
            RouteCoordinates = new List<Plugin.Geolocator.Abstractions.Position>();

            WorkField = new List<Label>
            {
                WorkField1,
                WorkField2,
                WorkField3,
                WorkField4
            };

            MessagingCenter.Subscribe<CounterFieldPage, TypeOfConventer>(this, "ChangeWorkField", OnTapGestureChose);
            //MessagingCenter.Unsubscribe<CounterFieldPage, string>(this, "ChangeWorkField");
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainMap.CheckGeoPermissions(true);
            MainMap.InBackgroundListeningLocation(false);
        }


        protected override void OnDisappearing()
        {
            MainMap.InBackgroundListeningLocation(true);

            base.OnDisappearing();           
        }


        private async void OnMyLocation(object sender, EventArgs e) {

            try
            {
                var myLocation = await Task.Run(() => AppGeolocator.Current.GetPositionAsync(TimeSpan.FromMilliseconds(500), null, false).Result);
                MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(myLocation.Latitude, myLocation.Longitude), MainMap.VisibleRegion.Radius));
            }
            catch { };
        }
        

        private void OnChooseMapType(object sender, EventArgs e)
        {
            switch ((int)MainMap.MapType) {
                case 0:
                    MainMap.MapType = MapType.Hybrid;
                    break;
                case 2:
                    MainMap.MapType = MapType.Satellite; 
                    break;
                case 1:
                    MainMap.MapType = MapType.Street;
                    break;
            }
        }



        private void OnTapGestureChose(CounterFieldPage sender, TypeOfConventer arg)
        {
            int i = 0;
            foreach (Label label in WorkField) {

                if(label.Id == sender.CallObjId){
                    WorkValue[i] = arg;
                }

                i++;
            }

            if (NewWorkoutPoint != null)
            {
                RefreshTimerWorkFieldAndObj(WorkoutData.Time);
                RefreshWorkFieldAndObj(NewWorkoutPoint);
            }
            else
                ResetCounter();
        }


        private async void OnTapGestureRecognizer(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CounterFieldPage( ((Label)sender).Id));
        }



        private async void OnUsingMap(object sender, EventArgs e)
        {
            UsingMap.IsEnabled = false;
            //Turn on
            if (MainMap.LocFlag == CommonMap.LocationFlag.Stop)
            {                     
                await MainMap.StartListeningLocation();

                if ( MainMap.LocFlag==CommonMap.LocationFlag.Start) {

                    UsingMap.IsVisible = false;
                    UsingMapStop.IsVisible = false;
                    UsingMapPause.IsVisible = true;

                    await StartListeningLocation();

                    autoEvent = new AutoResetEvent(false);
                    stateTimer = new Timer(UpdateTimerWorkFieldAndObj, autoEvent, 0, 1000);
                }    
                
            }
            UsingMap.IsEnabled = true;
        }


        private async void OnUsingMapPause(object sender, EventArgs e)
        {
            UsingMapPause.IsEnabled = false;

            if (MainMap.LocFlag == CommonMap.LocationFlag.Start)
            {
                await MainMap.StopListeningLocation(false, false);

                if (MainMap.LocFlag == CommonMap.LocationFlag.Stop)
                {
                    stateTimer.Dispose();
                    autoEvent.Dispose();

                    lastInterval = new DateTime(DateTime.Now.Ticks - startWorkout.Ticks + lastInterval.Ticks);
                    startWorkout = new DateTime(0);

                    await StopListeningLocation(false, false);

                    UsingMapPause.IsVisible = false;
                    UsingMap.IsVisible = true;
                    UsingMapStop.IsVisible = true;

                }
            }
            UsingMapPause.IsEnabled = true;
        }

        private async void OnUsingMapStop(object sender, EventArgs e)
        {
            UsingMapStop.IsEnabled = false;
            UsingMap.IsEnabled = false;

            if (MainMap.LocFlag == CommonMap.LocationFlag.Stop )
            {
                object mapPolyline = MainMap.platformRoad;
                await MainMap.StopListeningLocation(true, true);

                //You cant change order, this element send data 
                await Navigation.PushAsync(new MapResultPage(this.RouteCoordinates, WorkoutData, mapPolyline));
                await StopListeningLocation(true, true);

                UsingMap.IsVisible = true;
                UsingMapStop.IsVisible = false;
                }
            
            UsingMapStop.IsEnabled = true;
            UsingMap.IsEnabled = true;
        }


        public async Task StartListeningLocation()
        {
            await AppGeolocator.StartListeningAsync(MyGeoFlag,1000);//minDistance will take priority  

            AppGeolocator.Current.PositionChanged += PositionChanged;
            AppGeolocator.Current.PositionError += PositionError;


            try
            {   //Starting Position
                var positionStart = await Task.Run(() => AppGeolocator.Current.GetPositionAsync(TimeSpan.FromMilliseconds(50), null, false).Result);
                PositionChanged(this, new PositionEventArgs(positionStart));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }

        }

        public async Task StopListeningLocation(bool stopGeolocator, bool ifDataReset)
        {         
            AppGeolocator.Current.PositionChanged -= PositionChanged;
            AppGeolocator.Current.PositionError -= PositionError;

            await AppGeolocator.StopListeningAsync(MyGeoFlag, stopGeolocator);


            if (ifDataReset == true){
                RouteCoordinates.Clear();
                WorkoutData.Reset();
                NewWorkoutPoint = null;

                NumberOfPoint = 0;
                lastInterval = new DateTime(0);
                startWorkout = new DateTime(0);

                ResetCounter();
            }
        }

        int NumberOfPoint { get; set; } = 0;

        public void PositionChanged(object sender, PositionEventArgs arg)
        {
            var conventer = new WorkoutConventer();
            //start 
            if (RouteCoordinates.Count == 0)
            {
                //WorkoutData.NumberOfPoint++;
                WorkoutData.StartTime = new DateTime(DateTime.Now.Ticks); //new DateTime(arg.Position.Timestamp.LocalDateTime.Ticks);
                startWorkout = new DateTime(DateTime.Now.Ticks);
                UpdateWorkFieldAndObj(null, arg.Position);
                RouteCoordinates.Add(new Plugin.Geolocator.Abstractions.Position(arg.Position));
            }
            else if (startWorkout.Ticks == 0)
                {   //after pause, new value
                    if (arg.Position.Timestamp.Ticks != RouteCoordinates.Last().Timestamp.Ticks)
                    {
                        startWorkout = new DateTime(DateTime.Now.Ticks);
                        //WorkoutData.NumberOfPoint++;
                        //UpdateWorkFieldAndObj(RouteCoordinates.Last(), arg.Position);
                        UpdateWorkFieldAndObj(null, arg.Position);
                        RouteCoordinates.Add(new Plugin.Geolocator.Abstractions.Position(arg.Position));
                    }
                    else
                    {//after pause, if value is same
                        startWorkout = new DateTime(DateTime.Now.Ticks);
                    }//new value                           
                 }else if (arg.Position.Timestamp.Ticks != RouteCoordinates.Last().Timestamp.Ticks)
                    {
                        NumberOfPoint++;
                        UpdateWorkFieldAndObj(RouteCoordinates.Last(), arg.Position);
                        RouteCoordinates.Add(new Plugin.Geolocator.Abstractions.Position(arg.Position));
                    }
            
        }

        public void PositionError(object sender, PositionErrorEventArgs e)
        {        
            Debug.WriteLine(e.Error + "\n");
            //Handle event here for errors
        }


        void UpdateWorkFieldAndObj(Plugin.Geolocator.Abstractions.Position oldValue, Plugin.Geolocator.Abstractions.Position newValue) {

            var conventer = new WorkoutConventer();
            var surveyTime = (DateTime.Now.Ticks - startWorkout.Ticks + lastInterval.Ticks) / 10000000;//measurement approximate accuracy//seconds

            WorkoutData = conventer.ValueObject(new MapPreparedData(WorkoutData), oldValue, newValue, NumberOfPoint, surveyTime);
            NewWorkoutPoint = newValue;
            RefreshWorkFieldAndObj(newValue);
        }

        void RefreshWorkFieldAndObj(Plugin.Geolocator.Abstractions.Position newValue) {

            MapPreparedData copyWorkoutData = new MapPreparedData(WorkoutData);
            for (int i = 0; i < WorkField.Count; i++)
            {
                if (WorkValue[i] != TypeOfConventer.Timer)
                {
                    var newVal = WorkoutConventer.ValueTypeIn(WorkValue[i], copyWorkoutData, newValue).ToString("0.00");
                    newVal=newVal.Replace(",", ".");
                    var useLabel = WorkField[i];

                    Device.BeginInvokeOnMainThread(() => {
                        useLabel.Text = newVal;
                    });
                }
            }
        }



        void UpdateTimerWorkFieldAndObj(Object state)
        {
            DateTime actualTime= new DateTime(DateTime.Now.Ticks - startWorkout.Ticks + lastInterval.Ticks);
            WorkoutData.Time = actualTime;

            RefreshTimerWorkFieldAndObj(actualTime);
        }

        void RefreshTimerWorkFieldAndObj(DateTime actualTime) {

            int i = 0;
            for (i=0; i < WorkField.Count; i++)
            {
                if (WorkValue[i] == TypeOfConventer.Timer) {

                    var timerNewVal = actualTime.ToString("H:mm:ss");
                    var useLabel = WorkField[i];

                    Device.BeginInvokeOnMainThread(() => {
                        useLabel.Text = timerNewVal;
                    });
                }
            }
        }

        void ResetCounter() {

            for (int i = 0; i < WorkField.Capacity; i++)
            {
                if (WorkValue[i] == TypeOfConventer.Timer)
                    WorkField[i].Text = "0:00:00";
                else
                    WorkField[i].Text = "0.00";
            }
        }
    }
}


