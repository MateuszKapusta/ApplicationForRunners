using System.Collections.Generic;
using UIKit;
using CoreLocation;
using MapKit;
using ObjCRuntime;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using ApplicationForRunners.iOS;
using ApplicationForRunners.SharedClasses;

[assembly: ExportRenderer(typeof(CommonMap), typeof(NativeMap))]
namespace ApplicationForRunners.iOS
{
    public class NativeMap : MapRenderer
    {
        List<Plugin.Geolocator.Abstractions.Position> routeCoordinates;
        List<Plugin.Geolocator.Abstractions.Position> lastCoordinates;
        CommonMap formsMap;
        MKMapView nativeMap;
        int positionCount;
        int refreshRoud;

        List<CLLocationCoordinate2D> wholeRoad;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap != null)
                {
                    nativeMap.RemoveOverlays(nativeMap.Overlays);
                    nativeMap.OverlayRenderer = null;
                }
            }

            if (e.NewElement != null)
            {
                formsMap = (CommonMap)e.NewElement;
                nativeMap = Control as MKMapView;

                routeCoordinates = formsMap.RouteCoordinates;
                lastCoordinates = formsMap.LastCoordinates;
                positionCount = formsMap.positionCount;
                refreshRoud = formsMap.refreshRoud;

                nativeMap.OverlayRenderer = GetOverlayRenderer;
                nativeMap.AddOverlay(new MKPolyline());


                if (formsMap.platformRoad.GetType() == typeof(object))                
                    formsMap.platformRoad = new List<CLLocationCoordinate2D>();

                wholeRoad = (List<CLLocationCoordinate2D>)formsMap.platformRoad;
            }
        }

        MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            MKPolylineRenderer polylineRenderer=null;

            if (!Equals(overlayWrapper, null))
            {
                var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
                polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline)
                {
                    FillColor = UIColor.Blue,
                    StrokeColor = UIColor.Red,
                    LineWidth = 3,
                };
            }
            else
                polylineRenderer = new MKPolylineRenderer();

            return polylineRenderer;
        }


        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            
            if ((e.PropertyName.Equals("DrawPoint")) && formsMap.LocFlag == CommonMap.LocationFlag.Start
                && formsMap.DrawPoint != null && lastCoordinates.Count >= positionCount)
            {               
                var polylineOptions = CreatePolylineOpty(lastCoordinates);
                lastCoordinates.RemoveRange(0, positionCount - 1);
                nativeMap.AddOverlay(polylineOptions);
            }


            if ((e.PropertyName.Equals("LastPFlag")) && formsMap.LocFlag == CommonMap.LocationFlag.Stop
                && formsMap.LastPFlag != null)
            {
                if (lastCoordinates.Count > 1)
                {
                    var polylineOptions = CreatePolylineOpty(lastCoordinates);
                    lastCoordinates.RemoveRange(0, lastCoordinates.Count - 1);
                    nativeMap.AddOverlay(polylineOptions);
                }
            }

            if ((e.PropertyName.Equals("LastPFlag")) && formsMap.LocFlag == CommonMap.LocationFlag.Stop
                && formsMap.LastPFlag == null)
            {
                nativeMap.RemoveOverlays(nativeMap.Overlays);

                formsMap.platformRoad = new List<CLLocationCoordinate2D>();
                wholeRoad = (List<CLLocationCoordinate2D>)formsMap.platformRoad;
            }


            if (e.PropertyName.Equals("DrawPoint") && formsMap.LocFlag == CommonMap.LocationFlag.Start &&
                routeCoordinates != null && routeCoordinates.Count > 1 && (routeCoordinates.Count % refreshRoud) == 0)
            {
                RoudCreateOptimization();
            }
        }

        private void RoudCreateOptimization()
        {
            nativeMap.RemoveOverlays(nativeMap.Overlays);

            if (wholeRoad != null && wholeRoad.Count > 1)
            {
                var polylineOptions = MKPolyline.FromCoordinates(wholeRoad.ToArray());
                nativeMap.AddOverlay(polylineOptions);
            }


            if (formsMap.EntirePathObjTmp != null && ((List<CLLocationCoordinate2D>)formsMap.EntirePathObjTmp).Count > 1)
            {
                List<CLLocationCoordinate2D> polyline = (List<CLLocationCoordinate2D>)formsMap.EntirePathObjTmp;
                var polylineOptions = MKPolyline.FromCoordinates(polyline.ToArray());
                nativeMap.AddOverlay(polylineOptions);
            }
            else if (formsMap.EntirePathListTmp != null && formsMap.EntirePathListTmp.Count > 1)
            {
                var polylineOptions = CreatePolyline(formsMap.EntirePathListTmp);
                formsMap.EntirePathObjTmp = polylineOptions;

                nativeMap.AddOverlay(polylineOptions);
            }
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            RoudCreateOptimization();

            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }



        MKPolyline CreatePolylineOpty(List<Plugin.Geolocator.Abstractions.Position> useList)
        {
            CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[useList.Count];
            MKPolyline routeOverlay = new MKPolyline();

            if (wholeRoad.Count > 0)
                wholeRoad.RemoveAt(wholeRoad.Count - 1);


            if (useList.Count > 1)
            { 
                int index = 0;
                foreach (var position in useList)
                {
                    coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
                    index++;

                    wholeRoad.Add(new CLLocationCoordinate2D(position.Latitude, position.Longitude));
                }
                routeOverlay = MKPolyline.FromCoordinates(coords);
            }

            return routeOverlay;
        }

        MKPolyline CreatePolyline(List<Plugin.Geolocator.Abstractions.Position> useList)
        {
            CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[useList.Count];
            MKPolyline routeOverlay = new MKPolyline();

            if (useList.Count > 1)
            {
                int index = 0;
                foreach (var position in useList)
                {
                    coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
                    index++;
                }
                routeOverlay = MKPolyline.FromCoordinates(coords);
            }

            return routeOverlay;
        }
    }
}
