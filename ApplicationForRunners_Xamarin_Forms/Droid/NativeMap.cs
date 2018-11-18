using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps.Model;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using ApplicationForRunners.Droid;
using Android.Gms.Maps;
using ApplicationForRunners.SharedClasses;

[assembly: ExportRenderer(typeof(CommonMap), typeof(NativeMap))]
namespace ApplicationForRunners.Droid
{
    public class NativeMap : MapRenderer
    {
        List<Plugin.Geolocator.Abstractions.Position> routeCoordinates;
        List<Plugin.Geolocator.Abstractions.Position> lastCoordinates;
        CommonMap formsMap;
        int positionCount;
        int refreshRoud;
        PolylineOptions wholeRoad;


        public NativeMap(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                formsMap = (CommonMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;
                lastCoordinates = formsMap.LastCoordinates;
                positionCount = formsMap.positionCount;
                refreshRoud = formsMap.refreshRoud;

                if (formsMap.platformRoad.GetType() == typeof(object))
                    formsMap.platformRoad = new PolylineOptions();

                wholeRoad = (PolylineOptions)formsMap.platformRoad;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if ((e.PropertyName.Equals("DrawPoint")) && formsMap.LocFlag == CommonMap.LocationFlag.Start
                && formsMap.DrawPoint != null && lastCoordinates.Count >= positionCount)
            {
                var polylineOptions = CreatePolylineOpty(lastCoordinates);
                lastCoordinates.RemoveRange(0, positionCount - 1);
                NativeMap.AddPolyline(polylineOptions);
            }

            if ((e.PropertyName.Equals("LastPFlag")) && formsMap.LocFlag == CommonMap.LocationFlag.Stop
                && formsMap.LastPFlag != null)
            {
                if (lastCoordinates.Count > 1)
                {
                    var polylineOptions = CreatePolylineOpty(lastCoordinates);
                    lastCoordinates.RemoveRange(0, lastCoordinates.Count - 1);
                    NativeMap.AddPolyline(polylineOptions);
                }
            }

            if ((e.PropertyName.Equals("LastPFlag")) && formsMap.LocFlag == CommonMap.LocationFlag.Stop
                && formsMap.LastPFlag == null)
            {
                NativeMap.Clear();

                formsMap.platformRoad = new PolylineOptions();//new inicjation
                wholeRoad = (PolylineOptions)formsMap.platformRoad;
            }


            if (e.PropertyName.Equals("DrawPoint") && formsMap.LocFlag == CommonMap.LocationFlag.Start &&
                routeCoordinates != null && routeCoordinates.Count > 1 && (routeCoordinates.Count % refreshRoud) == 0)
            {
                RoudCreateOptimization();
            }

        }


        private void RoudCreateOptimization() {

            NativeMap.Clear();

            if (wholeRoad!=null && wholeRoad.Points.Count > 1)
            {
                wholeRoad.InvokeColor(-65535);//red
                wholeRoad.InvokeStartCap(new RoundCap());
                wholeRoad.InvokeEndCap(new RoundCap());

                NativeMap.AddPolyline(wholeRoad);
            }


            if (formsMap.EntirePathObjTmp != null && ((PolylineOptions)formsMap.EntirePathObjTmp).Points.Count>1)
            {
                PolylineOptions polyline = (PolylineOptions)formsMap.EntirePathObjTmp;
                polyline.InvokeColor(-65535);
                polyline.InvokeStartCap(new RoundCap());
                polyline.InvokeEndCap(new RoundCap());
                NativeMap.AddPolyline(polyline);
            }
            else if (formsMap.EntirePathListTmp != null && formsMap.EntirePathListTmp.Count>1) 
            {
                var polylineOptions = CreatePolyline(formsMap.EntirePathListTmp);
                formsMap.EntirePathObjTmp = polylineOptions;
                NativeMap.AddPolyline(polylineOptions);
            }
        }


        //induced two times
        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            map.UiSettings.MyLocationButtonEnabled = false;
            //onMapReday fires 2 times
            RoudCreateOptimization();         
        }


        PolylineOptions CreatePolylineOpty(List<Plugin.Geolocator.Abstractions.Position> useList )
        {
            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(-65535);//red  //polylineOptions.InvokeColor(unchecked((int)4294901760));0x66FF0000  (-65535)=ffff0000-ffffffff;
            polylineOptions.InvokeStartCap(new RoundCap());
            polylineOptions.InvokeEndCap(new RoundCap());

            Plugin.Geolocator.Abstractions.Position point;

            if(wholeRoad.Points.Count>0)
                wholeRoad.Points.RemoveAt(wholeRoad.Points.Count - 1);

            for (int i = 0; i < useList.Count; i++)
            {
                point = useList[i];
                polylineOptions.Add(new LatLng(point.Latitude, point.Longitude));

                wholeRoad.Add(new LatLng(point.Latitude, point.Longitude));
            }
            return polylineOptions;
        }

        PolylineOptions CreatePolyline(List<Plugin.Geolocator.Abstractions.Position> useList)
        {
            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(-65535);//red 
            polylineOptions.InvokeStartCap(new RoundCap());
            polylineOptions.InvokeEndCap(new RoundCap());

            Plugin.Geolocator.Abstractions.Position point;

            for (int i = 0; i < useList.Count; i++)
            {
                point = useList[i];
                polylineOptions.Add(new LatLng(point.Latitude, point.Longitude));
            }
            return polylineOptions;
        }
    }
}
