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
using Position = Maui.GoogleMaps.Position;
using Realms.Sync;



namespace RealmTodo.ViewModels
{
    public partial class MapsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllTasks;

        [ObservableProperty]
        private IQueryable<MapPin> maps;

        [ObservableProperty]
        public string dataExplorerLink = RealmService.DataExplorerLink;

        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;


        //-->Start--->used for testing -upload user record object to mongodb 

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

        //-->End--->used for testing -upload user record object to mongodb 








        public ICommand NavigateCommand { get; private set; }

        public MapsViewModel()
        {
            //set singlton to mappin 
            var singleton = ObjectSingleton.Instance;
            singleton.SetMapPinType();
            
            realm = RealmService.GetMainThreadRealm();
            currentUserId = RealmService.CurrentUser.Id;
        }

        public async void deleteExistingMapPinFromCloude(Maui.GoogleMaps.Pin newPin, string mapNameToDelete)
        {
            Console.WriteLine($"----> deleteExistingMapPinFromCloude OwnerId:{RealmService.CurrentUser.Id} ");
            Console.WriteLine($"----> deleteExistingMapPinFromCloude mapName:{mapNameToDelete} ");
            Console.WriteLine($"----> deleteExistingMapPinFromCloude label:{newPin.Label} ");
            Console.WriteLine($"----> deleteExistingMapPinFromCloude Address:{newPin.Address} ");


            var mapPinToDelete = new MapPin
            {
                OwnerId = RealmService.CurrentUser.Id, // Assuming `RealmService` is initialized
                Mapname = mapNameToDelete,
                Labelpin = newPin.Label,
                Address = newPin.Address,
                Latitude = newPin.Position.Latitude.ToString(), // Convert latitude to string
                Longitude = newPin.Position.Longitude.ToString() // Convert longitude to string
            };

            Console.WriteLine($"----> deleteExistingMapPinFromCloude ");

            await DeleteSinglePin(mapPinToDelete);

 
        }

        private static string GetCurrentDateTime()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Format it as a string
            //string formattedDateTime = now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedDateTime = now.ToString("dd-MM-yyyy HH:mm:ss");


            return formattedDateTime;
        }


        [RelayCommand]
        public void OnAppearing()
        {
            Console.WriteLine($"IsShowAllTasks is :{IsShowAllTasks} ");




            // Retrieve all items from Realm and convert them to a list.
            var mapNamesList = realm.All<MapPin>().ToList();

            // Group the items by Summary and select the first item from each group.
            var distinctMapNames = mapNamesList
                .GroupBy(map => map.Mapname)
                .Select(group => group.First())
                .OrderBy(map => map.Id)
                .ToList();

            // Assign the filtered list back to Items.
            Maps = distinctMapNames.AsQueryable();

            var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);



            IsShowAllTasks = currentSubscriptionType == SubscriptionType.All;
        }


        [RelayCommand]
        public async Task SaveUserRecord()
        {
            Console.WriteLine($"SaveUserRecord EditUserRecord -->");

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
                        UploadDateTime = GetCurrentDateTime(),
                        Comment = "CommentTest"+GetCurrentDateTime()

                    });
                }
            });




            await Shell.Current.GoToAsync("..");
        }



        [RelayCommand]
        public async Task Logout()
        {
            IsBusy = true;
            await RealmService.LogoutAsync();
            IsBusy = false;

            await Shell.Current.GoToAsync($"//login");
        }


        private static List<Maui.GoogleMaps.Pin> getPinsListByName(string trackName)
        {
            var realm = RealmService.GetMainThreadRealm();

            // Query Realm for all items with a matching Summary.
            var matchingMapPins = realm.All<MapPin>().Where(i => i.Mapname == trackName);

            var mapPinsList = realm.All<MapPin>().ToList(); // Fetch all items into memory

            // Now you can safely use Select
            var pinTypeList = mapPinsList
                .Where(i => i.Mapname == trackName)  // Filter if needed
                .Select(i => new Maui.GoogleMaps.Pin
                {
                    Label = i.Labelpin,
                    Address = i.Address,
                    Position = new Position(Convert.ToDouble(i.Latitude), Convert.ToDouble(i.Longitude))
                })
                .ToList();

            // Loop through the matching items and print their Summary.
            foreach (var pin in pinTypeList)
            {
                Console.WriteLine($"Address of pin (MapHelper class) -->pin label:'{pin.Label}'pin addr: {pin.Address}");
            }

            return pinTypeList;
        }



        [RelayCommand]
        public void TestCommand(MapPin pin)
        {
            Console.WriteLine("TestCommand triggered!");
        }


        //method that is used to edit map
        [RelayCommand]
        public async Task ChooseMapFromList(MapPin map)
        {
            string mapName = map.Mapname;
            Console.WriteLine($"(EditMap)MapsViewModel,mapname:{map.Mapname} ");

            //convert MapPin object with the same mapname to list with the same name but with type of Maui.GoogleMaps.Pin
            List<Maui.GoogleMaps.Pin> pinListOfSameMapName = getPinsListByName(mapName);

            var mapPage = MapPage.Instance;
            mapPage.set_pinsList(pinListOfSameMapName);
            mapPage.ShowTrack_Clicked();
            mapPage.SetTitle("map:"+mapName);
            if (await mapPage.IsLocationEnabled())
            {
                if (map.IsMine)
                {
                    Console.WriteLine($"-->Track is  mine!!!");
                    mapPage.ShowStartExerciseButton(true);//show the start exerice button on map
                    mapPage.ShowButtonsOnMap(true); // show buttons 
                    mapPage._canAddPins = true;
                    await Shell.Current.Navigation.PushAsync(mapPage);//1 way 
                    //await Shell.Current.GoToAsync($"chooseMapFromList");//2 way

                }
                else
                {
                    Console.WriteLine($"-->Track is not mine!!!");
                    mapPage.ShowStartExerciseButton(true);//show the start exerice button on map

                    mapPage.ShowButtonsOnMap(false); // Remove buttons from the map 
                    mapPage._canAddPins = false;
                    await Shell.Current.Navigation.PushAsync(mapPage);//1 way 
                    //await Shell.Current.GoToAsync($"chooseMapFromList");//2 way



                }



            }


        }







        // used to transfer the user to the map page 
        [RelayCommand]
        public async Task ToMapPage()
        {
   


            // Navigate to the singleton instance of MapPage
            var mapPage = MapPage.Instance;
            mapPage.SetTitle("Create new map");
            List<Maui.GoogleMaps.Pin> pinList = MapPage.Instance.GetPinList();
            if (await mapPage.IsLocationEnabled())
            {
                mapPage.ShowStartExerciseButton(false);
                mapPage.ClearMap();
                mapPage.ShowButtonsOnMap(true);//show buttons on map
                mapPage._canAddPins = true;// user can add pins on map
                await Shell.Current.Navigation.PushAsync(mapPage);
            }
        }









        // used to delete map from the maps view 
        [RelayCommand]
        public async Task DeleteMap(MapPin pin)
        {


            if (!await CheckItemOwnership(pin))
            {
                return;
            }

            // Query all maps with the same mapname
            var mapToDelete = realm.All<MapPin>()
                .Where(i => i.Mapname == pin.Mapname)
                .ToList();


            foreach (var pinsInMap in mapToDelete)
            {
                await DeleteSinglePin(pinsInMap);
            }
            // Refresh the list after deletion
            OnAppearing();
        }

        //delete single pin from map 
        [RelayCommand]
        public async Task DeleteSinglePin(MapPin pin)
        {

            //Console.WriteLine($"--->(DeleteItem) item summery:{item.Summary} ");

            //if (!await CheckItemOwnership(pin))
            //{
            //    return;
            //}

            await realm.WriteAsync(() =>
            {
                realm.Remove(pin);
            });

        }


        [RelayCommand]
        public void Refresh()
        {
            Console.WriteLine($"---> refreshed page ");
            OnAppearing();

        }




        [RelayCommand]
        public async Task ToTimerPage()//transfer to timer page
        {
            // Navigate to the singleton instance of MapPage
            var timerPage = TimerPage.Instance;
            await Shell.Current.Navigation.PushAsync(timerPage);
        }






        [RelayCommand]
        public void ChangeConnectionStatus()
        {
            isOnline = !isOnline;

            if (isOnline)
            {
                realm.SyncSession.Start();
            }
            else
            {
                realm.SyncSession.Stop();
            }

            ConnectionStatusIcon = isOnline ? "wifi_on.png" : "wifi_off.png";
        }

        [RelayCommand]
        public async Task UrlTap(string url)
        {
            await Launcher.OpenAsync(DataExplorerLink);
        }

        private async Task<bool> CheckItemOwnership(MapPin map)
        {
            //if (!item.IsMine)
            //{
            //    await DialogService.ShowAlertAsync("Error", "You cannot modify items not belonging to you", "OK");
            //    return false;
            //}

            return true;
        }

        async partial void OnIsShowAllTasksChanged(bool value)
        {

            await RealmService.SetSubscription(realm, value ? SubscriptionType.All : SubscriptionType.Mine);

            if (!isOnline)
            {
                await DialogService.ShowToast("Switching subscriptions does not affect Realm data when the sync is offline.");
            }
            Refresh();//refresh the items list 
        }
    }
}
