using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;
using ApplicationForRunners.UWP;
using Xamarin.Forms;
using ApplicationForRunners.SharedClasses;

[assembly: ExportRenderer(typeof(CommonMap), typeof(NativeMap))]
namespace ApplicationForRunners.UWP
{
    public class NativeMap : MapRenderer
    {
        List<Plugin.Geolocator.Abstractions.Position> routeCoordinates;
        List<Plugin.Geolocator.Abstractions.Position> lastCoordinates;
        CommonMap formsMap;
        MapControl nativeMap;
        int positionCount ;
        int refreshRoud;

        List<BasicGeoposition> wholeRoad;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                formsMap = (CommonMap)e.NewElement;
                nativeMap = Control as MapControl;

                routeCoordinates = formsMap.RouteCoordinates;
                lastCoordinates = formsMap.LastCoordinates;
                positionCount = formsMap.positionCount;
                refreshRoud = formsMap.refreshRoud;

                if (formsMap.platformRoad.GetType() == typeof(object))
                    formsMap.platformRoad = new List<BasicGeoposition>();

                wholeRoad = (List<BasicGeoposition>)formsMap.platformRoad;               
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("DrawPoint") && formsMap.LocFlag == CommonMap.LocationFlag.Start
                && formsMap.DrawPoint != null ) {

                Device.BeginInvokeOnMainThread(() => {
                    if ( lastCoordinates.Count >= positionCount)
                    {
                        var polylineOptions = CreatePolylineOpty(lastCoordinates);
                        lastCoordinates.RemoveRange(0, positionCount - 1);
                        nativeMap.MapElements.Add(polylineOptions);                                  
                    }
                }); 

                Device.BeginInvokeOnMainThread(async () => {
                    if (formsMap.DrawPoint != null) { 
                        BasicGeoposition pointGeoPos = new BasicGeoposition() {
                            Latitude = formsMap.DrawPoint.Latitude,
                            Longitude = formsMap.DrawPoint.Longitude,
                        };                    
                        Geopoint myPosition = new Geopoint(pointGeoPos);
                        await nativeMap.TrySetViewAsync(myPosition, nativeMap.ZoomLevel, null, null, MapAnimationKind.Linear);
                    }
                });
            }

            if ((e.PropertyName.Equals("LastPFlag")) && formsMap.LocFlag == CommonMap.LocationFlag.Stop
                && formsMap.LastPFlag != null)
            {
                if (lastCoordinates.Count > 1)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var polylineOptions = CreatePolylineOpty(lastCoordinates);
                        lastCoordinates.RemoveRange(0, lastCoordinates.Count - 1);
                        nativeMap.MapElements.Add(polylineOptions);
                    });
                }
            }

            if ((e.PropertyName.Equals("LastPFlag")) && formsMap.LocFlag == CommonMap.LocationFlag.Stop
                && formsMap.LastPFlag == null)
            {
                nativeMap.MapElements.Clear();


                formsMap.platformRoad = new List<BasicGeoposition>();//new inicjation
                wholeRoad = (List<BasicGeoposition>)formsMap.platformRoad;                
            }

            if (e.PropertyName.Equals("DrawPoint") && formsMap.LocFlag == CommonMap.LocationFlag.Start &&
                routeCoordinates != null && routeCoordinates.Count > 1 && (routeCoordinates.Count % refreshRoud) == 0)
            {
                RoudCreateOptimization();
            }

        }

        private void  RoudCreateOptimization()
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                nativeMap.MapElements.Clear();
            });

            if (wholeRoad != null && wholeRoad.Count > 1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var polyline = new MapPolyline
                    {
                        StrokeColor = Windows.UI.Color.FromArgb(255, 255, 0, 0),
                        StrokeThickness = 5
                    };
                    polyline.Path = new Geopath(wholeRoad);
                    nativeMap.MapElements.Add(polyline);
                });
            }

      
            if (formsMap.EntirePathObjTmp != null && ((List<BasicGeoposition>)formsMap.EntirePathObjTmp).Count > 1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    List<BasicGeoposition> polylinePoints = (List<BasicGeoposition>)formsMap.EntirePathObjTmp;
                    var polyline = new MapPolyline
                    {
                        StrokeColor = Windows.UI.Color.FromArgb(255, 255, 0, 0),
                        StrokeThickness = 5
                    };
                    polyline.Path = new Geopath(polylinePoints);
                    nativeMap.MapElements.Add(polyline);
                });
            }
            else if (formsMap.EntirePathListTmp != null && formsMap.EntirePathListTmp.Count > 1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var polyline = CreatePolyline(formsMap.EntirePathListTmp);
                    formsMap.EntirePathObjTmp = polyline;

                    nativeMap.MapElements.Add(polyline);
                });
            }      
        }


        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            RoudCreateOptimization();
            
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        MapPolyline CreatePolylineOpty(List<Plugin.Geolocator.Abstractions.Position> useList)
        {
            var coordinates = new List<BasicGeoposition>();
            var polyline = new MapPolyline
            {
                StrokeColor = Windows.UI.Color.FromArgb(255, 255, 0, 0),
                StrokeThickness = 5
            };

            if (wholeRoad.Count > 0)
                wholeRoad.RemoveAt(wholeRoad.Count - 1);

            if (useList.Count > 1)
            {
                foreach (var position in useList)
                {
                    coordinates.Add(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude });
                    wholeRoad.Add(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude });
                }
                polyline.Path = new Geopath(coordinates);
            }
            return polyline;
        }

        MapPolyline CreatePolyline(List<Plugin.Geolocator.Abstractions.Position> useList)
        {
            var coordinates = new List<BasicGeoposition>();
            var polyline = new MapPolyline
            {
                StrokeColor = Windows.UI.Color.FromArgb(255, 255, 0, 0),
                StrokeThickness = 5
            };

            if (useList.Count > 1)
            {
                foreach (var position in useList)
                {
                    coordinates.Add(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude });
                }
                polyline.Path = new Geopath(coordinates);
            }
            return polyline;
        }

    }
}
