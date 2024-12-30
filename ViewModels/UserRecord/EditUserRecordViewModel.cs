using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealmTodo.Models;
using RealmTodo.Services;
using RealmTodo.ViewModels;

using RealmTodo.Views; // Correct namespace for TestPage
using Realms;
using Microsoft.Maui.Controls; // Required for navigation
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;
using Realms.Sync;


using Microsoft.Maui.Controls; // Required for navigation
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;

using Position = Maui.GoogleMaps.Position;
using RealmTodo.Views; // Correct namespace for TestPage

using Realms.Sync;

namespace RealmTodo.ViewModels

{
    public partial class EditUserRecordViewModel : BaseViewModel, IQueryAttributable
    {



        [ObservableProperty]
        private string inputUserName;// value in the xaml page

        [ObservableProperty]
        private UserRecord initialUserRecord;

        [ObservableProperty]
        private string profileNameNew;


        [ObservableProperty]
        private string mapNameNew;


        [ObservableProperty]
        private string trackTimeNew;

        [ObservableProperty]
        private string uploadDateTimeNew;

        [ObservableProperty]
        private string comment;


        [ObservableProperty]
        private string pageHeader;

        public EditUserRecordViewModel()
        {
            Console.WriteLine($"----> empty constructor,EditMapPinViewModel");



        }

        //used to update the name of the pin number on the map 
        private void OnDoneButtonClicked(object sender, EventArgs e)
        {

            Console.WriteLine(" ---------->>>>> OnDoneButtonClicked");
            
        }

        [RelayCommand]
        public void PrintName()
        {
            // Print the name entered in the Entry field
            if (!string.IsNullOrWhiteSpace(InputUserName))
            {
                Console.WriteLine($"Entered Name: {InputUserName}");
            }
            else
            {
                Console.WriteLine("No name was entered.");
            }
        }
        [RelayCommand]
        public void OnOKClicked()
        {
            if (!string.IsNullOrWhiteSpace(InputUserName))
            {
                Console.WriteLine($"Entered Name: {InputUserName}");
            }
            else
            {
                Console.WriteLine("No name was entered.");
            }

            Console.WriteLine("OK button command executed.");
        }




        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {



            if (query.Count > 0 && query["userrecord"] != null) // we're editing an Item
            {

                InitialUserRecord = query["userrecord"] as UserRecord;
                ProfileNameNew = InitialUserRecord.ProfileName;
                MapNameNew = InitialUserRecord.MapName;
                TrackTimeNew = InitialUserRecord.TrackTime;
                UploadDateTimeNew= InitialUserRecord.UploadDateTime;
                Comment = InitialUserRecord.Comment;
                //Latitude = InitialMapPin.Latitude;
                //Longtiude = InitialMapPin.Longitude;
                //PageHeader = $"Modify Map: {InitialMapPin.Mapname}(PinMap)";
            }
            else // we're creating a new user record
            {

                ProfileNameNew = "";
                MapNameNew = "";
                TrackTimeNew = "";
                UploadDateTimeNew = "";
                Comment = "";
 

                PageHeader = "Create new Record User ";
            }
        }








        [RelayCommand]
        public async Task Test()
        {
            Console.WriteLine($"Test Command ,user name:{InputUserName}");

        }






        [RelayCommand]
        public async Task SaveUserRecord()
        {
            Console.WriteLine($"SaveUserRecord EditUserRecordViewModel -->");

            var singleton = ObjectSingleton.Instance;
            singleton.SetUserRecordType();

            var realm = RealmService.GetMainThreadRealm();

            var userRecordsSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "DogSubscription");

            if (!userRecordsSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Dog. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var userRecordQuery = realm.All<UserRecord>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(userRecordQuery, new SubscriptionOptions { Name = "DogSubscription" });
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
                if (InitialUserRecord != null) // editing an item
                {
                    InitialUserRecord.ProfileName = profileNameNew;
                    InitialUserRecord.MapName = mapNameNew;
                    InitialUserRecord.TrackTime = trackTimeNew;
                    InitialUserRecord.UploadDateTime = uploadDateTimeNew;

                }
                else // creating a new item
                {
                    realm.Add(new UserRecord()
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        ProfileName = "test1",
                        MapName = "mapTest",
                        TrackTime = "tracktime",
                        UploadDateTime = "uploadDateTest",
                        Comment="CommentTest"
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

