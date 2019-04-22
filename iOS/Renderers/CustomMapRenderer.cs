using System;
using AepApp.iOS.Renderers;
using AepApp.MaterialForms;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMapView), typeof(CustomMapRenderer))]
namespace AepApp.iOS.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap != null)
                {
                    nativeMap.RemoveAnnotations(nativeMap.Annotations);
                    nativeMap.GetViewForAnnotation = null;
                    nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                    nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                    nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
                }
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMapView)e.NewElement;
                var nativeMap = Control as MKMapView;
                nativeMap.AddAnnotation(new CustomAnnotation()
                {
                    Title = "标题",
                    Coordinate = new CLLocationCoordinate2D(App.currentLocation.Latitude,App.currentLocation.Longitude),
                });
                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }


        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            CustomAnnotation customAnnotation = annotation as CustomAnnotation;
            if (customAnnotation != null)
            {
                MKAnnotationView pinView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation("11");
                if (pinView == null)
                    pinView = new MKPinAnnotationView(annotation, "11");
                //string path = NSBundle.MainBundle.PathForResource("orangetarget.png", "");
                UIImage image = UIImage.FromFile("orangetarget.png");
                UIImageView imageView = new UIImageView(image);
                imageView.Frame = new CoreGraphics.CGRect(0, 0, 50, 50);
                imageView.Center = new CoreGraphics.CGPoint(25, 25);
                pinView.Add(imageView);
                pinView.BackgroundColor = UIColor.Blue;
                pinView.Image = image;
                pinView.CanShowCallout = true;
                return pinView;
            }

            return null;
    
        }

        private void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
      
          }

        private void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
        
        
        }

        private void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
        }
    }



    public class CustomAnnotation : MKPointAnnotation
    {

        public string imageSours { get; set; }

    }
}
