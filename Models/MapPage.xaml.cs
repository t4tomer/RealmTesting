﻿using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Maui.GoogleMaps;
using RealmTodo.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Input;
using Position = Maui.GoogleMaps.Position;
using RealmTodo.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using Realms;
using RealmTodo.Views; // Correct namespace for TestPage
using Microsoft.Maui.Controls; // Required for navigation
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
//using static Xamarin.Google.Crypto.Tink.Shaded.Protobuf.Internal;


namespace RealmTodo.Views
{
    public partial class MapPage : ContentPage
    {
        private static MapPage _instance; // Singleton instance
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        private MapHelper MapHelperObject; // Declare m as a class-level variable
        private EditItemViewModel CloudPage; //new 
        List<Maui.GoogleMaps.Pin> pinsList;// the list of pins in the map
        public bool _canAddPins = true; // Controls if pins can be added

        int strokeColorPolyline = 0;

        public ICommand NavigateCommand { get; private set; }
        int randomNumberTest = 1;

        // Private constructor to prevent direct instantiation
        private MapPage()
        {
            InitializeComponent();
        }

        // Public static property to get the singleton instance
        public static MapPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapPage();
                }
                return _instance;
            }
        }

        public void set_pinsList(List<Maui.GoogleMaps.Pin> newpinstList)
        {
            this.pinsList = newpinstList;
        }

        public async Task<bool> IsLocationEnabled()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                // If we get a valid location, services are likely enabled.
                if (location != null)
                {
                    Console.WriteLine($"Location: Lat {location.Latitude}, Lon {location.Longitude}");
                    return true;
                }
                else
                {
                    Console.WriteLine("No location found. Location services may be off.");
                    await DialogService.ShowAlertAsync("Error", "No location found. Location services may be off", "OK");

                    return false;
                }
            }
            catch (FeatureNotEnabledException)
            {
                Console.WriteLine("Location services are disabled.");
                await DialogService.ShowAlertAsync("Error", "Turnon location on your device", "OK");

                return false;
            }
            catch (PermissionException)
            {
                Console.WriteLine("Location permission not granted.");
                await DialogService.ShowAlertAsync("Error", "Turnon location on your device", "OK");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                await DialogService.ShowAlertAsync("Error", "Unexpected error", "OK");

                return false;
            }
        }

        // Public method to get the list of pins from the map
        public List<Maui.GoogleMaps.Pin> GetPinList()
        {
            return myMap.Pins.ToList();
        }

        private List<Maui.GoogleMaps.Pin> GetPins(object sender, EventArgs e)
        {
            return myMap.Pins.ToList();
        }

        // Method to print the number of points on the map
        private void numberOfPoints(object sender, MapClickedEventArgs e)
        {
            int pointCount = myMap.Pins.Count;
            Console.WriteLine($"----> Number of points on the map!!!: {pointCount}");
        }
        // create random number 
        public int GenerateRandomNumber()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 1000); // The upper bound is exclusive, so use 1000 to get numbers from 1 to 999
            return randomNumber;
        }
        // method that is used to zoom to my location
        public async Task<Location> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    CenterMap(location.Latitude, location.Longitude);

                }

                return location;
            }
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }


        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
        // zoom the google map to the cordinates- of x and y 
        private void CenterMap(double x, double y)
        {
            //var position = new Position(31.268333463883636, 34.80691033370654);
            var position = new Position(x, y);
            var mapSpan = Maui.GoogleMaps.MapSpan.FromCenterAndRadius(position, Maui.GoogleMaps.Distance.FromMeters(1)); // Adjust the radius as needed
            myMap.MoveToRegion(mapSpan);
        }
        private void ZoomToMyLocation_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine($"----> LogLongitude_Clicked pressed!!!");
            GetCurrentLocation();

        }

        private void DeletLastPoint_Clicked(object sender, EventArgs e)
        {
            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();

            Console.WriteLine($" \t\t-->Number of pins:(MapPage)" + pinsList.Count);

            MapHelperObject.deleteLastPoint(pinsList);
        }



        // add new point on the map 
        private void addPointOnMap(object sender, MapClickedEventArgs e)
        {

            if (!_canAddPins) return; // Stop if adding pins is disabled

            //Console.WriteLine($"----> Added Point to screen1");

            double latitude = e.Point.Latitude;
            double longitude = e.Point.Longitude;

            // Print the coordinates to the console







            int pinCount = myMap.Pins.Count + 1;

            var pin = new Maui.GoogleMaps.Pin
            {
                Label = pinCount.ToString(),
                Address = "Adresss" + pinCount.ToString(),
                Position = e.Point,
                Type = PinType.Place
            };

            strokeColorPolyline++;


            myMap.Pins.Add(pin);
            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();

            MapHelperObject = new MapHelper(pinsList, myMap); 
            MapHelperObject.DrawLineBetweenAllPins(strokeColorPolyline);

        }

        // Print all addresses of the points 
        private void PrintPinAddresses(object sender, MapClickedEventArgs e)
        {
            var pins = myMap.Pins.ToList();
            foreach (var pin in pins)
            {
                Console.WriteLine($"Address of pin22 -->'{pin.Label}': {pin.Address}");
            }
        }





        private async void AddToCloud_Clicked(object sender, EventArgs e)
        {

            Console.WriteLine($"-->AddToCloud_Clicked ");

            //AddMapToDbPage
            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            int pinCount=pinsList.Count;
            if(await EnoughPins(pinCount))
            {
                MapHelperObject = new MapHelper(pinsList, myMap);
                MapHelperObject.PrintPinAddresses();
                //CloudPage = new EditItemViewModel(pinsList, myMap);
                var AddToCloud = new AddMapToDbPage(pinsList, myMap);
                //EditPinAddrPage.SetPinsList(pinsList);
                //! Pass the pinsList directly when navigating to the triggerPage
                await Navigation.PushAsync(AddToCloud);
            }






        }

        //added the ShowButtonsOnMap method 

        public void ShowButtonsOnMap(bool cond)
        {
            EditPointButton.IsVisible = cond;
            ClearMapButton.IsVisible = cond;
            AddToCloudButton.IsVisible = cond;
            DeleteLastPointButton.IsVisible = cond;
            ZoomButton.IsVisible = true;
            DistanceButton.IsVisible = true;

        }

        public void ShowTrack_Clicked()//show the pins and the lines of the track 
        {

            ClearMap();//clear the map from previus pins 

            MapHelperObject = new MapHelper(pinsList, myMap);
            MapHelperObject.showTrackOnMap();//show the pins on the map .
            MapHelperObject.DrawLineBetweenAllPins(strokeColorPolyline);//draw line between all the pins of the map .

        }


        // method that is used to transfer the user to the edit point page
        private async void Edit_Point_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine($"----> Edit Point clicked ");

            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            int pinCount = pinsList.Count;
            //Console.WriteLine($"number of pins in the list(Add_Point_Clicked): -->'{pinCount}': {pinCount}");

            var EditPinAddrPage = new EditPinAddr(pinsList, myMap);
            //EditPinAddrPage.SetPinsList(pinsList);
            //! Pass the pinsList directly when navigating to the triggerPage
            await Navigation.PushAsync(EditPinAddrPage);

        }
        // remove all the points&polylines from the map 
        private async void Reset_Map_Clicked(object sender, EventArgs e)
        {

            ClearMap();

        }


        private async Task<bool> EnoughPins(int num)
        {
            if (num == 0)
            {
                await DialogService.ShowAlertAsync("Error", "Not Enough Pins(add at least 2 points).", "OK");
                return false;
            }
            else if (num == 1)
            {
                await DialogService.ShowAlertAsync("Error", "Not Enough Pins(add at least 1 point).", "OK");
                return false;
            }
            return true;

        }



        public void ClearMap()
        {
            Console.WriteLine($"----> Remove all Points  ");
            // Clear all pins from the map
            myMap.Pins.Clear();
            // Clear all polylines from the map
            myMap.Polylines.Clear();

            Console.WriteLine("----> All pins and polylines have been cleared.");
        }


        // calculate the distance between all the points on the map
        private async void Calc_Distance_Clicked(object sender, EventArgs e)
        {

            
            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            int pinCount=pinsList.Count;
            if(await EnoughPins(pinCount))
            {
                //MapHelperObject = new MapHelper(pinsList,myMap); // Initialize m in the constructor

                MapHelperObject.set_pinsList(pinsList);
                double totalDistance = MapHelperObject.calculateTotalDistance();


                // Create an instance of TestPage and pass the total distance
                var testPage = new DistancePage(totalDistance, pinsList, MapHelperObject);

                // Navigate to the TestPage
                await Navigation.PushAsync(testPage);
            }


        }


        private async void toPage(string pageName)
        {
            Debug.WriteLine("toPage function!!!!"); // Write a debug message indicating the method was called

            // Construct the full type name including namespace
            string fullTypeName = $"MauiGoogleMapsSample.{pageName}";

            // Get the Type object based on the full type name
            Type pageType = Type.GetType(fullTypeName);

            if (pageType != null)
            {
                // Check if NavigateCommand can execute with this Type
                if (NavigateCommand.CanExecute(pageType))
                {
                    NavigateCommand.Execute(pageType);
                }
            }
            else
            {
                Debug.WriteLine($"Page type '{pageName}' not found.");
            }
        }

        // Method to calculate distance between two pins
        private double GetDistance(Maui.GoogleMaps.Pin p1, Maui.GoogleMaps.Pin p2)
        {
            // Access the Position property of each Pin
            var pos1 = p1.Position;
            var pos2 = p2.Position;

            // Convert Position to Location
            Location loc1 = new Location(pos1.Latitude, pos1.Longitude);
            Location loc2 = new Location(pos2.Latitude, pos2.Longitude);

            // Calculate the distance using Location.CalculateDistance
            double distance = Location.CalculateDistance(loc1, loc2, DistanceUnits.Kilometers);
            Console.WriteLine($"----> Distance between p1 and p2: {distance} km");

            return distance;
        }



        private Maui.GoogleMaps.Pin getPoint(double x, double y, string inputLabel, string inputAddress)
        {
            var position = new Location(x, y);
            var pin = new Maui.GoogleMaps.Pin
            {
                Label = inputLabel,
                Address = inputAddress,
                Type = PinType.Place,
                Position = new Position(position.Latitude, position.Longitude)
            };

            myMap.Pins.Add(pin);
            return pin;
        }
    }
}
