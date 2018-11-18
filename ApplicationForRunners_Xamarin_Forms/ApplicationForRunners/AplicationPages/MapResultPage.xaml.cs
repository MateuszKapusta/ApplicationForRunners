using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ApplicationForRunners.DataObjects;
using ApplicationForRunners.ItemManager;
using Xamarin.Forms.Maps;

namespace ApplicationForRunners.AplicationPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapResultPage : ContentPage
	{
        private List<Plugin.Geolocator.Abstractions.Position> resultRoad;
        MapPreparedData resultData;

        private WorkoutItemManager workoutManager;
        private  MapPointManager mapManager;

        WorkoutItem sendWorkout;
        List<MapPointItem> sendMapPointList;

        //using at history
        public MapResultPage( MapPreparedData newTreningData)
        {
            InitializeComponent();

            resultData = new MapPreparedData(newTreningData);         
            ToolbarItems.RemoveAt(1);
            ToolbarItems.RemoveAt(0);
            ResultMap.IsVisible = false;
        }


        public MapResultPage(List<Plugin.Geolocator.Abstractions.Position> newTrening, MapPreparedData newTreningData)
        {
            InitializeComponent();

            resultRoad = new List<Plugin.Geolocator.Abstractions.Position>(newTrening);
            resultData = new MapPreparedData(newTreningData);
            ToolbarItems.RemoveAt(1);
            ToolbarItems.RemoveAt(0);
            ResultMap.DrawRoadAtStart(newTrening);
            ResultMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(newTrening[(newTrening.Count / 2)].Latitude, newTrening[(newTrening.Count / 2)].Longitude), new Distance(1000)));                     
        }


        //using at end of trening
        public MapResultPage (List<Plugin.Geolocator.Abstractions.Position> newTrening, MapPreparedData newTreningData,object mapPolyline)
        {
			InitializeComponent ();

            resultRoad =new List<Plugin.Geolocator.Abstractions.Position>( newTrening);
            resultData = new MapPreparedData(newTreningData);
            workoutManager = DBConnection.MainConnection.WorkoutItem;
            mapManager = DBConnection.MainConnection.MapPoint;
            sendMapPointList = new List<MapPointItem>();

            //ToLocalTime

            switch (Device.RuntimePlatform) {
                case Device.iOS:
                    break;
                case Device.Android:
                    this.Title = "Add";
                    break;
                case Device.UWP:
                    break;
            }

            ResultMap.DrawRoadAtStart(mapPolyline);
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            FillFields(resultData);

            if (workoutManager != null)
            {
                sendWorkout = new WorkoutItem(resultData);
                await workoutManager.SaveItemAsync(sendWorkout, false);

                if (resultRoad.Count > 0)
                {
                    AddPath.IsEnabled = true;
                }
            }
        }


        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            if (workoutManager != null)
            {
                await workoutManager.SyncAsync(true);
                await mapManager.SyncAsync(true);
            }
        }


        public async  void OnDelete(object sender, EventArgs e) {
           
            var action = await DisplayAlert("Delete", "Are you sure you want to delete this workout", "Yes", "No");
            if (action)
            {
                AddPath.IsEnabled = false;
                foreach (MapPointItem item in sendMapPointList) {
                    await mapManager.DeleteItemAsync(item, false);
                }
                await workoutManager.DeleteItemAsync(sendWorkout, false);
                await Navigation.PopAsync();                
            }          
        }

         
        public async void OnAddPath(object sender, EventArgs e)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    await DisplayAlert("Alert", "This function is not available on Windows", "OK");
                    return;
                    //break;
            }

            int a = 0;
            foreach (Plugin.Geolocator.Abstractions.Position value in resultRoad)
            {
                MapPointItem sendMapPoint = new MapPointItem() { Order = a++, Latitude = value.Latitude, Longitude = value.Longitude, WorkoutItemId = sendWorkout.Id };
                sendMapPointList.Add(sendMapPoint);
                await mapManager.SaveItemAsync(sendMapPoint, false);
            }
            AddPath.IsEnabled = false;            
        }

        private void FillFields(MapPreparedData data) {
            FieldDate.Text = data.StartTime.ToString("dd-MM-yyyy HH:mm:ss");

            TimeFild.Text = data.Time.ToString("H:mm:ss");
            DistanceFild.Text = WorkoutConventer.ValueTypeAfter(TypeOfConvAfter.Distance,0,data.RoadDistance).ToString("0.00").Replace(",", ".");
            MaxSpeedFild.Text = WorkoutConventer.ValueTypeAfter(TypeOfConvAfter.Speed, data.MaxSpeed, data.RoadDistance).ToString("0.00").Replace(",", ".");
            AvgSpeedFild.Text = WorkoutConventer.ValueTypeAfter(TypeOfConvAfter.Speed, data.MediumSpeed, data.RoadDistance).ToString("0.00").Replace(",", ".");

            MaxPaceFild.Text = WorkoutConventer.ValueTypeAfter(TypeOfConvAfter.Tempo, data.MaxSpeed, data.RoadDistance).ToString("0.00").Replace(",", ".");
            AvgPaceFild.Text = WorkoutConventer.ValueTypeAfter(TypeOfConvAfter.Tempo, data.MediumSpeed, data.RoadDistance).ToString("0.00").Replace(",", ".");
            MaxAltitudeFild.Text = WorkoutConventer.ValueTypeAfter(TypeOfConvAfter.Altitude, data.MaxAltitude, data.RoadDistance).ToString("0.00").Replace(",", ".");
            MinAltitudeFild.Text = WorkoutConventer.ValueTypeAfter(TypeOfConvAfter.Altitude, data.MinAltitude, data.RoadDistance).ToString("0.00").Replace(",", ".");
        }
    }
}