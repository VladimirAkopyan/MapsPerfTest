using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HiveDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool removed;
        Random generator = new Random(5);

        public MainWindow()
        {
            InitializeComponent();
            
            myMap.LayoutUpdated += (sender, args) =>
            {
                if (!removed)
                {
                    RemoveOverlayTextBlock();
                }
            };
        }


        private void RemoveOverlayTextBlock()
        {
            var children = myMap.Children;
            Microsoft.Maps.MapControl.WPF.Overlays.LoadingErrorMessage error = null;


            error = myMap.Children
                                .OfType<Microsoft.Maps.MapControl.WPF.Overlays.LoadingErrorMessage>()
                                .FirstOrDefault();

            if (error != null)
            {

                error.Opacity = 0; 

                var parentBorder = error.Parent as Border;
                if (parentBorder != null)
                {
                    parentBorder.Visibility = Visibility.Collapsed;
                }

                removed = true;
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            MapLayer imageLayer = new MapLayer();
            

            

            for (int i = 0; i < 1000; i++)
            {
                MapPolygon polygon = new MapPolygon();
                polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
                polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                polygon.StrokeThickness = 2;
                polygon.Opacity = 0.5;

                double lattitude = generator.NextDouble() * 5 + 50;
                double longitude = 0 - generator.NextDouble() * 10; 

                polygon.Locations = new LocationCollection() {
                new Location(lattitude, longitude),
                new Location(lattitude, longitude+ 0.03),
                new Location(lattitude + 0.015, longitude+ 0.03),
                new Location(lattitude + 0.015, longitude)};

                imageLayer.Children.Add(polygon);
            }
            myMap.Children.Add(imageLayer);
        }

        private void button_Click_3(object sender, RoutedEventArgs e)
        {
            MapLayer imageLayer = new MapLayer();


            Image image = new Image();

            //Define the URI location of the image
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"C:\maptest\Deskto\Deskto\Assets\Sentinel.jpg");
            // To save significant application memory, set the DecodePixelWidth or  
            // DecodePixelHeight of the BitmapImage value of the image source to the desired 
            // height or width of the rendered image. If you don't do this, the application will 
            // cache the image as though it were rendered as its normal size rather then just 
            // the size that is displayed.
            // Note: In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.
            //Define the image display properties
            //myBitmapImage.DecodePixelHeight = 150;
            myBitmapImage.EndInit();
            image.Source = myBitmapImage;
            image.Opacity = 1.0;
            image.Stretch = System.Windows.Media.Stretch.UniformToFill;


            double lattitude = generator.NextDouble() * 5 + 50;
            double longitude = 0 - generator.NextDouble() * 10;

            //The map location to place the image at
            Location location = new Location() { Latitude = lattitude, Longitude = longitude };


            LocationRect extent = new LocationRect(location, 0.5, 0.5);
            //Add the image to the defined map layer
            imageLayer.AddChild(image, extent);

            
            //Add the image layer to the map
            myMap.Children.Add(imageLayer);
        }
    }
}
