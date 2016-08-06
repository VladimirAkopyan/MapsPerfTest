using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MapTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Random generator = new Random(5);
        volatile bool drawingPolygon = false;
        volatile bool insideItem = false; 

        MapPolyline Drawing = new MapPolyline(); 


        public MainPage()
        {
            this.InitializeComponent();
            Map.Style = MapStyle.AerialWithRoads;             
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            for (uint i = 0; i < 1000; i++)
            {
                MapPolygon polygon = new MapPolygon();

                polygon.FillColor =  new Windows.UI.Color() {A = 150, B = 200, G = 100, R = 100 }  ;
 
                polygon.StrokeColor = Windows.UI.Colors.White;
                polygon.StrokeThickness = 2;

                

                double lattitude = generator.NextDouble() * 2 + 50;
                double longitude = 0 - generator.NextDouble() * 2;

                BasicGeoposition[] points = new BasicGeoposition[5];
                points[0] = new BasicGeoposition() { Latitude = lattitude, Longitude = longitude };
                points[1] = new BasicGeoposition() { Latitude = lattitude, Longitude = longitude + 0.03 };
                points[2] = new BasicGeoposition() { Latitude = lattitude + 0.015, Longitude = longitude + 0.03 };
                points[3] = new BasicGeoposition() { Latitude = lattitude + 0.015, Longitude = longitude };
                points[4] = new BasicGeoposition() { Latitude = lattitude, Longitude = longitude };


                polygon.Path = new Geopath(points);

                Map.MapElements.Add(polygon);
           
            }
        }

        private void Map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            foreach(var element in args.MapElements)
            {
                if (element.GetType() == typeof(MapPolygon))
                {
                    var it = (MapPolygon)element;
                    it.FillColor = Windows.UI.Colors.White;
                }
                else
                {
                    element.Visible = false;
                }
            }

        }

        private void Map_MapElementPointerEntered(MapControl sender, MapElementPointerEnteredEventArgs args)
        {
            insideItem = true; 
                if (args.MapElement.GetType() == typeof(MapPolygon))
                {
                    var it = (MapPolygon)args.MapElement;
                    it.FillColor = Windows.UI.Colors.Red;
                }

            
        }

        private void Map_MapElementPointerExited(MapControl sender, MapElementPointerExitedEventArgs args)
        {
            insideItem = false; 
            if (args.MapElement.GetType() == typeof(MapPolygon))
            {
                var it = (MapPolygon)args.MapElement;
                it.FillColor = new Windows.UI.Color() { A = 150, B = 200, G = 100, R = 100 };
            }
        }

        private void Map_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            
        }

        private void Map_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
        }

 
        private void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            if (!insideItem && !drawingPolygon)
            {
                Windows.UI.Xaml.Controls.Maps.MapIcon icon = new MapIcon();
                icon.Location = args.Location;
                icon.Title = "You clicked!";
                icon.NormalizedAnchorPoint = new Point(0.3, 0.6);
                Map.MapElements.Add(icon);
            }
            else if(drawingPolygon)
            {
                var point = new BasicGeoposition()
                {
                    Longitude = args.Location.Position.Longitude,
                    Latitude = args.Location.Position.Latitude
                };

                if (Drawing.Path == null) {
                    Drawing.Path = new Geopath(new BasicGeoposition[1] { point });
                    Drawing.StrokeDashed = true;
                    Drawing.StrokeThickness = 4;
                    Drawing.StrokeColor = new Windows.UI.Color() { A = 255, B = 70, G = 70, R = 180 };
                    Map.MapElements.Add(Drawing);
                }
                else {
                    BasicGeoposition[] pathPoints = new BasicGeoposition[Drawing.Path.Positions.Count + 1];
                    Drawing.Path.Positions.ToArray().CopyTo(pathPoints, 0);
                    pathPoints[pathPoints.Count() - 1] = point;
                    var _geopath = new Geopath(pathPoints);
                    if(! Geometry.isSelfIntersecting(_geopath))
                        Drawing.Path = _geopath;
                }                           
            }

        }

        private void toggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            button.Content = "Finish";
            drawingPolygon = true;
        }

        private void toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            button.Content = "DrawPolygon";
            drawingPolygon = false;

            MapPolygon polygon = new MapPolygon();
            polygon.FillColor = new Windows.UI.Color() { A = 150, B = 100, G = 100, R = 200 };
            polygon.StrokeColor = Windows.UI.Colors.White;
            polygon.StrokeThickness = 2;
            polygon.Path = Drawing.Path; 
            
            Map.MapElements.Remove(Drawing);
            Map.MapElements.Add(polygon); 
            Drawing = new MapPolyline();
        }

        
    }
}
