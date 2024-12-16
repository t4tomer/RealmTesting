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
    public partial class EditUserRecordViewModel : BaseViewModel, IQueryAttributable
    {




        [ObservableProperty]
        private UserRecord initialUserRecord;

        [ObservableProperty]
        private string profile_name;


        [ObservableProperty]
        private string map_name;


        [ObservableProperty]
        private string track_time;

        [ObservableProperty]
        private string upload_date_time;


        [ObservableProperty]
        private string pageHeader;

        public EditUserRecordViewModel()
        {
            Console.WriteLine($"----> empty constructor,EditMapPinViewModel");



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



            if (query.Count > 0 && query["userrecord"] != null) // we're editing an Item
            {

                InitialUserRecord = query["userrecord"] as UserRecord;
                Profile_name = InitialUserRecord.Profilename;
                Map_name = InitialUserRecord.Mapname;
                Track_time = InitialUserRecord.TrackTime;
                Upload_date_time= InitialUserRecord.UploadDateTime;
                //Latitude = InitialMapPin.Latitude;
                //Longtiude = InitialMapPin.Longitude;
                //PageHeader = $"Modify Map: {InitialMapPin.Mapname}(PinMap)";
            }
            else // we're creating a new pin map
            {
                //Mapname = "";
                //Labelpin = "";
                //Address = "";
                //Latitude = "";
                //Longtiude = "";

                PageHeader = "Create a New Map";
            }
        }

        





        



 


        //[RelayCommand]
        //public async Task SavePin(Maui.GoogleMaps.Pin newPin)
        //{
        //    Console.WriteLine($"SavePin EditMapPin -->'{newPin.Label}': {newPin.Address}");

        //    var realm = RealmService.GetMainThreadRealm();

        //    var mapPinSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "DogSubscription");

        //    if (!mapPinSubscriptionExists)
        //    {
        //        Console.WriteLine("No existing subscription for Dog. Adding one now...");

        //        // Add the subscription synchronously
        //        realm.Subscriptions.Update(() =>
        //        {
        //            var dogQuery = realm.All<MapPin>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
        //            realm.Subscriptions.Add(dogQuery, new SubscriptionOptions { Name = "DogSubscription" });
        //        });

        //        Console.WriteLine("MapPin subscription added. Waiting for synchronization...");

        //        // Wait for synchronization
        //        await realm.Subscriptions.WaitForSynchronizationAsync();
        //        Console.WriteLine("MapPin synchronized successfully.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("MapPin subscription already exists.");
        //    }





        //    await realm.WriteAsync(() =>
        //    {
        //        if (InitialMapPin != null) // editing an item
        //        {
        //            InitialMapPin.Mapname = Summary;
        //            InitialMapPin.Labelpin = Labelpin;
        //            InitialMapPin.Address = Address;
        //            InitialMapPin.Latitude = Latitude;
        //            InitialMapPin.Longitude = Longtiude;

        //        }
        //        else // creating a new item
        //        {
        //            realm.Add(new MapPin()
        //            {
        //                OwnerId = RealmService.CurrentUser.Id,
        //                Mapname = summary,
        //                Labelpin = newPin.Label,
        //                Address = newPin.Address,
        //                Latitude = newPin.Position.Latitude.ToString(),
        //                Longitude = newPin.Position.Longitude.ToString()
        //            });
        //        }
        //    });




        //    Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
        //    await Shell.Current.GoToAsync("..");
        //}





        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

