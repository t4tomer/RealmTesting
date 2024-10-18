using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
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

namespace RealmTodo.ViewModels

{
    public partial class EditItemViewModel : BaseViewModel, IQueryAttributable
    {
        [ObservableProperty]
        private Item initialItem;

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
        Maui.GoogleMaps.Map myMap;

        public EditItemViewModel()
        {
            Console.WriteLine($"----> empty constructor,EditItemViewModel");


        }

        public EditItemViewModel(List<Pin> NewPinsList, Maui.GoogleMaps.Map newMyMap)
        {
            Console.WriteLine($"-->  EditItemViewModel(pinsList,myMAp)!!");

            this.pinsList = NewPinsList;
            this.myMap = newMyMap;
            if(pinsList == null || myMap==null)
                Console.WriteLine($"--> pinsList or myMap  is null !!");



        }



        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.Count > 0 && query["item"] != null) // we're editing an Item
            {
                InitialItem = query["item"] as Item;
                Summary = InitialItem.Summary;
                Mapname = InitialItem.Mapname;
                Labelpin = InitialItem.Labelpin;
                Address = InitialItem.Address;
                Latitude = InitialItem.Latitude;
                Longtiude = InitialItem.Longitude;
                PageHeader = $"Modify Item {InitialItem.Id}";
            }
            else // we're creating a new item
            {
                Summary = "";
                Mapname = "";
                Labelpin = "";
                Address = "";
                Latitude = "";
                Longtiude = "";

                PageHeader = "Create a New Item";
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

            Console.WriteLine("UploadToCloudPins --EditItemViewModel.");
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


            var realm = RealmService.GetMainThreadRealm();
            await realm.WriteAsync(() =>
            {
                if (InitialItem != null) // editing an item
                {
                    InitialItem.Summary = Summary;
                    InitialItem.Mapname = Summary;
                    InitialItem.Labelpin = Labelpin;
                    InitialItem.Address = Address;
                    InitialItem.Latitude = Latitude;
                    InitialItem.Longitude = Longtiude;

                }
                else // creating a new item
                {
                    realm.Add(new Item()
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        Summary = summary,
                        Mapname = summary,
                        Labelpin = newPin.Label,
                        Address = newPin.Address,
                        Latitude = newPin.Position.Latitude.ToString(),
                        Longitude = newPin.Position.Longitude.ToString()
                    });
                }
            });

            //double number = 123.45;
            //string result = number.ToString();
            //Console.WriteLine(result); // Output: "123.45"

            // If you're getting this app code by cloning the repository at
            // https://github.com/mongodb/template-app-maui-todo, 
            // it does not contain the data explorer link. Download the
            // app template from the Atlas UI to view a link to your data.
            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }




        [RelayCommand]
        public async Task SaveItem()
        {


            List<Pin> pinList = MapPage.Instance.GetPinList();
            int numberOfPins = pinsList.Count;
            Console.WriteLine($"--> number of pins(SaveItem):{numberOfPins}!!!");


            var realm = RealmService.GetMainThreadRealm();
            await realm.WriteAsync(() =>
            {
                if (InitialItem != null) // editing an item
                {
                    InitialItem.Summary = Summary;
                    InitialItem.Mapname = Summary;
                    InitialItem.Labelpin = Labelpin;
                    InitialItem.Address = Address;
                    InitialItem.Latitude = Latitude;
                    InitialItem.Longitude = Longtiude;

                }
                else // creating a new item
                {
                    realm.Add(new Item()
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        Summary = summary,
                        Mapname = summary,
                        Labelpin = summary,
                        Address = summary,
                        Latitude = "test13",
                        Longitude = "test14"
                    });
                }
            });

            // If you're getting this app code by cloning the repository at
            // https://github.com/mongodb/template-app-maui-todo, 
            // it does not contain the data explorer link. Download the
            // app template from the Atlas UI to view a link to your data.
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

