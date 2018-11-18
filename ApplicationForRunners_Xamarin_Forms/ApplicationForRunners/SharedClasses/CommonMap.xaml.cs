using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace ApplicationForRunners.SharedClasses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommonMap : Map
    {

        public GLFlag MyGeoFlag { get; } =new  GLFlag();

        public enum LocationFlag { Stop, Start, InBackground};
        public LocationFlag LocFlag { get; private set; }

        readonly public  int positionCount;
        readonly public int refreshRoud;
        public List<Plugin.Geolocator.Abstractions.Position> RouteCoordinates { get; set; }
        public List<Plugin.Geolocator.Abstractions.Position> LastCoordinates { get; set; }
        public List<Plugin.Geolocator.Abstractions.Position> EntirePathListTmp { get; set; }
        public object EntirePathObjTmp = null;
        public object platformRoad = new object();

        public static readonly BindableProperty DrawPointProperty =
        BindableProperty.Create(nameof(DrawPoint), typeof(Plugin.Geolocator.Abstractions.Position), typeof(CommonMap), new Plugin.Geolocator.Abstractions.Position(), BindingMode.TwoWay);

        public Plugin.Geolocator.Abstractions.Position DrawPoint{
            get { return (Plugin.Geolocator.Abstractions.Position)GetValue(DrawPointProperty); }
            set { SetValue(DrawPointProperty, value); }
        }

        public static readonly BindableProperty LastPFlagProperty =
        BindableProperty.Create(nameof(LastPFlag), typeof(Plugin.Geolocator.Abstractions.Position), typeof(CommonMap), new Plugin.Geolocator.Abstractions.Position(), BindingMode.TwoWay);

        public Plugin.Geolocator.Abstractions.Position LastPFlag{
            get { return (Plugin.Geolocator.Abstractions.Position)GetValue(LastPFlagProperty); }
            set { SetValue(LastPFlagProperty, value); }
        }


        public CommonMap(){

            InitializeComponent();

            if (Constants.ApplicationActualPermissions == null)
                throw new Exception("Must set platform ApplicationActualPermissions before calling ApplicationActualPermissions.");
            else
            AskForPermissions();

            positionCount = 3;
            refreshRoud = 100;
            LastCoordinates = new List<Plugin.Geolocator.Abstractions.Position>();
            RouteCoordinates = new List<Plugin.Geolocator.Abstractions.Position>();
        }

        public void DrawRoadAtStart(List<Plugin.Geolocator.Abstractions.Position> pointsList)
        {
            if (pointsList != null && pointsList.Count > 1) {
                EntirePathListTmp = pointsList;
            }                
        }

        public void DrawRoadAtStart(object pointsObj)
        {
            if (pointsObj != null) {
                EntirePathObjTmp = pointsObj;
            }
        }

        public void DeleteRoadAtStart() {

            EntirePathObjTmp = null;
            EntirePathListTmp = null;
        }
        
        
        public async Task StopListeningLocation(bool stopGeolocator,bool ifDataReset)
        {
            LocFlag = LocationFlag.Stop;

            LastPFlag = new Plugin.Geolocator.Abstractions.Position();

            if (ifDataReset == true) {

                platformRoad = new object();//is delete

                RouteCoordinates.Clear();
                LastCoordinates.Clear();
                DrawPoint = null;
                LastPFlag = null;              
            }

            AppGeolocator.Current.PositionChanged -= PositionChanged;
            AppGeolocator.Current.PositionError -= PositionError;

            await AppGeolocator.StopListeningAsync(MyGeoFlag, stopGeolocator);
        }

        public async Task StartListeningLocation(){

            if (CheckGeoPermissions(true) == false) {
                AskForPermissions();
                return;
            }

            await AppGeolocator.StartListeningAsync(MyGeoFlag,1000);//minDistance will take priority  
            LocFlag = LocationFlag.Start;
            AppGeolocator.Current.PositionChanged += PositionChanged;
            AppGeolocator.Current.PositionError += PositionError;

            try
            {   //Starting Position
                var positionStart = await Task.Run(() =>  AppGeolocator.Current.GetPositionAsync(TimeSpan.FromMilliseconds(50), null, false).Result);
                PositionChanged(this, new Plugin.Geolocator.Abstractions.PositionEventArgs(positionStart));
            }
            catch (Exception ex) {
                Debug.WriteLine("Unable to get location: " + ex);
            }               
        }

        public void InBackgroundListeningLocation(bool option) {

            if (option == true) {
                if (LocFlag == LocationFlag.Start)
                    LocFlag = LocationFlag.InBackground;

            }else
                if (LocFlag == LocationFlag.InBackground)
                    LocFlag = LocationFlag.Start;                             
        }


        public void PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs arg)
        {

            if ((LocFlag == LocationFlag.Start || LocFlag == LocationFlag.InBackground) 
                && (RouteCoordinates.Count == 0 || arg.Position.Timestamp.Ticks != RouteCoordinates.Last().Timestamp.Ticks)) //first element can be null, so dont use it 
            {
             
                RouteCoordinates.Add(new Plugin.Geolocator.Abstractions.Position(arg.Position));
                LastCoordinates.Add(new Plugin.Geolocator.Abstractions.Position(arg.Position));
                DrawPoint = new Plugin.Geolocator.Abstractions.Position(arg.Position);


                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                    case Device.Android:
                        if(VisibleRegion!=null)
                        this.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(arg.Position.Latitude, arg.Position.Longitude), VisibleRegion.Radius));
                        break;
                    case Device.UWP:
                        //Implementation in native map

                        break;
                }
            }
        }

        public void  PositionError(object sender, Plugin.Geolocator.Abstractions.PositionErrorEventArgs e)
        {
            //await StopListeningLocation(true,false);
            Debug.WriteLine(e.Error+"\n");
            //Handle event here for errors
            //await StartListeningLocation();
        }

        public async void SetGeolocator()
        {
            var position = await AppGeolocator.Current.GetLastKnownLocationAsync();
            if (position != null)
                if (VisibleRegion != null)
                    this.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), VisibleRegion.Radius));
                else
                    this.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromKilometers(1)));


            AppGeolocator.Current.DesiredAccuracy = 0.5;
        }

        public bool CheckGeoPermissions(bool updateMapField)
        {
            if (Constants.ApplicationActualPermissions.CheckPermissions(Constants.LocationPermissions) == false)
                return false;

            if (!AppGeolocator.Current.IsGeolocationAvailable || !AppGeolocator.Current.IsGeolocationEnabled) 
                return false;
            

            if (updateMapField == true) {
                this.IsShowingUser = true;
                SetGeolocator();
            }
          
            return true;
        }

        private void AskForPermissions(){
            Action<int> actionPermission = HandlePermissions;
            Constants.ApplicationActualPermissions.AskForPermissions(Constants.LocationPermissions, actionPermission);         
        }

        private void HandlePermissions(int number){ 
            CheckGeoPermissions(true);
        }
    }
}