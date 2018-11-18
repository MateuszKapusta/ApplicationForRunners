using ApplicationForRunners.DataObjects;
using ApplicationForRunners.ItemManager;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApplicationForRunners.AplicationPages
{
    public partial class WorkoutListPage : ContentPage
    {
        WorkoutItemManager managerWorkout;
        MapPointManager managerMapPoints;


        public WorkoutListPage()
        {
            InitializeComponent();

            managerWorkout = DBConnection.MainConnection.WorkoutItem;
            managerMapPoints= DBConnection.MainConnection.MapPoint;
            StackLUWP.IsVisible = false;

            switch (Device.RuntimePlatform){
                    case Device.UWP://Windows:
                    StackLUWP.IsVisible = true;

                    var syncButton = new Button{
                            Text = "Items synchronization",
                            HeightRequest = 30
                        };
                        syncButton.Clicked += OnRefreshButtonUWP;
                        buttonsPanel.Children.Add(syncButton);
                        break;
                }
        }
  

        protected override async void OnAppearing()
        {
            base.OnAppearing();
 
            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(true, syncItems: true);
        }



        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var Workout = (MapPreparedData)e.SelectedItem ;
            if(Workout == null)
                return;

            var PointsList = await managerMapPoints.GetSpecificItemsAsync(Workout.WorkoutId);
            if (PointsList != null && PointsList.Count > 1)
            {
                var list = new List<MapPointItem>(PointsList);
                List<MapPointItem> sortedList = list.OrderBy(o => o.Order).ToList();
                List<Position> convertedList = new List<Position>();

                foreach (MapPointItem item in sortedList)               
                    convertedList.Add(MapPointItem.ConvertToPosition(item));
                
                await Navigation.PushAsync(new MapResultPage(convertedList, Workout));
            }
            else {
                await Navigation.PushAsync(new MapResultPage(Workout));
            }

            workoutList.SelectedItem = null;
        }





        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);


            var Workout = WorkoutItem.ConvertToWorkoutItem( (MapPreparedData)mi.CommandParameter );


            await CompleteItem(Workout);
        }

        async Task CompleteItem(WorkoutItem item)
        {
            await managerWorkout.DeleteItemAsync(item, true);


            workoutList.ItemsSource = await ReturnRefreshedItem(false);
        }


        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }

        public async void OnRefreshButtonUWP(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                
                workoutList.ItemsSource = await ReturnRefreshedItem(syncItems);
            }
        }

        private async Task<List<MapPreparedData>> ReturnRefreshedItem(bool syncItems) {

            var dataList = await managerWorkout.GetItemsAsync(syncItems);
            List<WorkoutItem> sortedList = dataList.OrderBy(o => o.WorkoutDate).ToList();
            sortedList.Reverse();

            List<MapPreparedData> dataListConverted = new List<MapPreparedData>();
            foreach (WorkoutItem item in sortedList)
            {
                dataListConverted.Add(MapPreparedData.ConvertToPreparedData(item));
            }
            return dataListConverted;
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }
}

