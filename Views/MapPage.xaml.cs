using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Maui.GoogleMaps;
using RealmTodo.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Input;
using Position = Maui.GoogleMaps.Position;

namespace RealmTodo.Views
{
    public partial class MapPage : ContentPage
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        public ICommand NavigateCommand { get; private set; }
        int randomNumberTest = 1;
        public MapPage()
        {
            InitializeComponent();
            /*    explaining NavigateCommand         
             *  Initialize NavigateCommand as a new Command<Type> object.
             *  This command takes a lambda expression that specifies what should happen when the command is executed.
             * The lambda expression accepts a parameter of type Type (representing the type of page to navigate to).                             
             */


            //-->point object !!!
            //ChoreDataAccess db = new ChoreDataAccess();
            //db.CreatePoint(new PointOnMap() { inputLabel="test",inputAddress="FUCK" });
            //--> person object 

            //--> UserModel object!!!





            //
            //TODO aaafghhgh
            //! mark af
            //Pin pin1 = getPoint(31.267747377228275, 34.80811007275169, "busStopNearHouse", "HaDaatStr"); // bus stop near house
            //Pin pin2 = getPoint(31.264841221137907, 34.81186612445039, "busStopGavYam", "YakovMarshStr"); // bus near GavYam 
            //Pin pin3 = getPoint(31.263721857951772, 34.81224914158527, "busStopGym", "Torat HaYahasut"); // bus Torat hauyhasot 

            //double dist = GetDistance(pin1, pin3);

            //m.test(); // Using m object

        }

        // Method to get the list of pins on the map
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
        private void CenterMap(double x,double y)
        {
            //var position = new Position(31.268333463883636, 34.80691033370654);
            var position = new Position(x, y);
            var mapSpan = Maui.GoogleMaps.MapSpan.FromCenterAndRadius(position, Maui.GoogleMaps.Distance.FromMeters(1)); // Adjust the radius as needed
            myMap.MoveToRegion(mapSpan);
        }
        private void ZoomToMyLocation_Clicked(object sender, EventArgs  e)
        {
            Console.WriteLine($"----> LogLongitude_Clicked pressed!!!");
            GetCurrentLocation();
            //Location loc = await GetCurrentLocation();




        }



        // add new point on the map 
        private void addPointOnMap(object sender, MapClickedEventArgs e)
        {
            //Console.WriteLine($"----> Added Point to screen1");

            double latitude = e.Point.Latitude;
            double longitude = e.Point.Longitude;

            // Print the coordinates to the console
            Console.WriteLine($"Map clicked at Latitude22: {latitude}, Longitude: {longitude}");





            //int randomNumberTest = GenerateRandomNumber();

            Console.WriteLine($"----> Added Point to {randomNumberTest}");


            var pin = new Maui.GoogleMaps.Pin
            {
                Label = randomNumberTest.ToString(),
                Address = "Adresss"+randomNumberTest.ToString(),
                Position = e.Point,
                Type = PinType.Place
            };

            randomNumberTest++;

            //Pin pin1 = getPoint(31.267747377228275, 34.80811007275169, "busStopNearHouse", "HaDaatStr"); // bus stop near house
            //Pin pin2 = getPoint(31.264841221137907, 34.81186612445039, "busStopGavYam", "YakovMarshStr"); // bus near GavYam 
            //Pin pin3 = getPoint(31.263721857951772, 34.81224914158527, "busStopGym", "Torat HaYahasut"); // bus Torat hauyhasot 
            //double dist = GetDistance(pin1, pin3);

            myMap.Pins.Add(pin);
            numberOfPoints(sender, e); // Call the numberOfPoints method to print the number of points
            PrintPinAddresses(sender, e); // Print the addresses of the pins 
            //MapHelperObject.test(); // Use m object
            //List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            //MapHelperObject.set_pinsList(pinsList);
            //MapHelperObject.PrintPinAddresses(sender, e);
            //MapHelperObject.calculateTotalDistance();
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

        // Event handler for the + button 
        private async void EditPoint(object sender, EventArgs e)
        {
            Console.WriteLine($"----> Add Point ");
            ////toPage("TestPage");
            ////string pageName = "NotePage";


            //List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            //int pinCount = pinsList.Count;
            //Console.WriteLine($"number of pins in the list(Add_Point_Clicked): -->'{pinCount}': {pinCount}");

            ////triggerObject.SetPinsList(pinsList);
            //var triggerPage = new PropertyTriggerXaml();
            //triggerPage.SetPinsList(pinsList);
            //string pageName = "PropertyTriggerXaml";
            ////! Pass the pinsList directly when navigating to the triggerPage
            //await Navigation.PushAsync(triggerPage);


        }

        private async void Reset_Map_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine($"----> Remove all Points  ");
            myMap.Pins.Clear();
            randomNumberTest = 1;

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

        // Remove point from map 
        private void OnPinClicked(object sender, MapClickedEventArgs e)
        {
            Console.WriteLine($"----> Remove the point ");

            var pin = sender as Maui.GoogleMaps.Pin;
            if (pin != null)
            {
                myMap.Pins.Remove(pin);
                Console.WriteLine($"----> Pin removed: {pin.Label}");
            }
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
