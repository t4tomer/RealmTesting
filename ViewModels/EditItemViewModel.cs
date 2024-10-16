using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using Maui.GoogleMaps;

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

            this.pinsList = NewPinsList;
            this.myMap = newMyMap;

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

        //TODO nned to fix this method 
        [RelayCommand]
        public async Task UploadToCloudPins()
        {

            if (pinsList.Count == 0)
            {
                Console.WriteLine($"--> no pins -error!!!");
                return;
            }



            foreach (var pin in pinsList)
            {
                Console.WriteLine($"PrintPinAddresses -->'{pin.Label}': {pin.Address}");
                await SavePin(pin);
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
                        Latitude = "test21",
                        Longitude = "test22"
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
        public async Task SaveItem()
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

