using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using RealmTodo.ViewModels;

using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using Maui.GoogleMaps;
using RealmTodo.Views; // Correct namespace for TestPage
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
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;
using Realms.Sync;

namespace RealmTodo.ViewModels

{
    public partial class EditMapPinViewModel : BaseViewModel, IQueryAttributable
    {




        [ObservableProperty]
        private MapPin initialMapPin;

        [ObservableProperty]
        private string summary;


        [ObservableProperty]
        private string mapname;


        [ObservableProperty]
        private string labelpin;


        [ObservableProperty]
        private string address;


        [ObservableProperty]
        private string latitude;


        [ObservableProperty]
        private string longtiude;



        [ObservableProperty]
        private string pageHeader;

        List<Maui.GoogleMaps.Pin> pinsList;// the list of pins in the map
        private Maui.GoogleMaps.Map myMap;
        private MapHelper MapHelperObject; // OBEJECT   THAT is used to show track of map
        public EditMapPinViewModel()
        {
            Console.WriteLine($"----> empty constructor,EditMapPinViewModel");
            MapHelperObject = new MapHelper(); // Initialize m in the constructor



        }

        //public EditMapPinViewModel(List<Pin> NewPinsList, Maui.GoogleMaps.Map newMyMap)
        //{
        //    Console.WriteLine($"-->  EditMapPinViewModel(pinsList,myMAp)!!");

        //    this.pinsList = NewPinsList;
        //    this.myMap = newMyMap;
        //    if(pinsList == null || myMap==null)
        //        Console.WriteLine($"--> pinsList or myMap  is null !!");



        //}



        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {



            if (query.Count > 0 && query["mappin"] != null) // we're editing an Item
            {

                InitialMapPin = query["mappin"] as MapPin;
                Mapname = InitialMapPin.Mapname;
                Labelpin = InitialMapPin.Labelpin;
                Address = InitialMapPin.Address;
                Latitude = InitialMapPin.Latitude;
                Longtiude = InitialMapPin.Longitude;
                PageHeader = $"Modify Map: {InitialMapPin.Mapname}(PinMap)";
            }
            else // we're creating a new pin map
            {
                Mapname = "";
                Labelpin = "";
                Address = "";
                Latitude = "";
                Longtiude = "";

                PageHeader = "Create a New Map";
            }
        }

        //Show the track of pins that are stored in realm db 
        [RelayCommand]
        public async Task ShowTrack()
        {

            string trackName = InitialMapPin.Mapname;
            var realm = RealmService.GetMainThreadRealm();

            // Query Realm for all items with a matching Summary.
            var matchingMapPins = realm.All<MapPin>().Where(i => i.Mapname == trackName);

            var mapPinsList = realm.All<MapPin>().ToList(); // Fetch all items into memory

            // Now you can safely use Select
            var summaries = mapPinsList
                .Where(i => i.Mapname == trackName)  // Filter if needed
                .Select(i => new Maui.GoogleMaps.Pin
                {
                    Label = i.Labelpin,
                    Address = i.Address,
                    Position = new Position(Convert.ToDouble(i.Latitude), Convert.ToDouble(i.Longitude))
                })
                .ToList();

            // Loop through the matching items and print their Summary.
            foreach (var pin in summaries)
            {
                Console.WriteLine($"Address of pin (MapHelper class) -->pin label:'{pin.Label}'pin addr: {pin.Address}");
            }


            // Navigate to the singleton instance of MapPage
            var mapPage = MapPage.Instance;
            List<Maui.GoogleMaps.Pin> pinList = MapPage.Instance.GetPinList();
            mapPage.set_pinsList(summaries);
            mapPage.ShowTrack_Clicked();

            if (await mapPage.IsLocationEnabled())
            {
                if (InitialMapPin.IsMine)
                {
                    Console.WriteLine($"-->Track is  mine!!!");
                    mapPage.ShowButtonsOnMap(true); // show buttons 
                    mapPage._canAddPins = true;
                    await Shell.Current.Navigation.PushAsync(mapPage);
                }
                else
                {
                    Console.WriteLine($"-->Track is not mine!!!");
                    mapPage.ShowButtonsOnMap(false); // Remove buttons from the map 
                    mapPage._canAddPins = false;
                    await Shell.Current.Navigation.PushAsync(mapPage);

                }
            }




            if (!matchingMapPins.Any())
            {
                Console.WriteLine($"No pinmaps found with the summary: {trackName}");
            }


        }

        [RelayCommand]
        public async Task PrintList()
        {
            Console.WriteLine("PrintList --EditItemViewModel.");
            if (MapPage.Instance == null)
                Console.WriteLine("MapPage instance is null.");
            else
                Console.WriteLine($"MapPage instance initialized with {MapPage.Instance.GetPinList().Count} pins.");



            List<Maui.GoogleMaps.Pin> pinsList1 = MapPage.Instance.GetPinList();

        }


        //TODO nned to fix this method 
        [RelayCommand]
        public async Task UploadToCloudPins()
        {

            Console.WriteLine("UploadToCloudPins --EditMapPinViewModel.");
            if (MapPage.Instance == null)
                Console.WriteLine("MapPage instance is null.");
            else
                Console.WriteLine($"MapPage instance initialized with(UploadToCloudPins) {MapPage.Instance.GetPinList().Count} pins.");

            List<Maui.GoogleMaps.Pin> pinsList = MapPage.Instance.GetPinList();

            foreach (var pin in pinsList)
            {
                Console.WriteLine($"PrintPinAddresses -->'{pin.Label}': {pin.Address}");
                await SavePin(pin);//TODO continue from this point 
            }
        }


        [RelayCommand]
        public async Task SavePin(Maui.GoogleMaps.Pin newPin)
        {
            Console.WriteLine($"SavePin EditMapPin -->'{newPin.Label}': {newPin.Address}");

            var realm = RealmService.GetMainThreadRealm();



            var mapPinSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "DogSubscription");

            if (!mapPinSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Dog. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var dogQuery = realm.All<MapPin>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(dogQuery, new SubscriptionOptions { Name = "DogSubscription" });
                });

                Console.WriteLine("MapPin subscription added. Waiting for synchronization...");

                // Wait for synchronization
                await realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("MapPin synchronized successfully.");
            }
            else
            {
                Console.WriteLine("MapPin subscription already exists.");
            }










            await realm.WriteAsync(() =>
            {
                if (InitialMapPin != null) // editing an item
                {
                    InitialMapPin.Mapname = Summary;
                    InitialMapPin.Labelpin = Labelpin;
                    InitialMapPin.Address = Address;
                    InitialMapPin.Latitude = Latitude;
                    InitialMapPin.Longitude = Longtiude;

                }
                else // creating a new item
                {
                    realm.Add(new MapPin()
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        Mapname = summary,
                        Labelpin = newPin.Label,
                        Address = newPin.Address,
                        Latitude = newPin.Position.Latitude.ToString(),
                        Longitude = newPin.Position.Longitude.ToString()
                    });
                }
            });




            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }





        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

