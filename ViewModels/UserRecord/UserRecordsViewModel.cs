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



namespace RealmTodo.ViewModels
{
    public partial class UserRecordsViewModel : BaseViewModel
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

        public ICommand NavigateCommand { get; private set; }

        public UserRecordsViewModel()
        {
            realm = RealmService.GetMainThreadRealm();
            currentUserId = RealmService.CurrentUser.Id;
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
            mapPage.SetTitle(mapName);
            if (await mapPage.IsLocationEnabled())
            {
                if (map.IsMine)
                {
                    Console.WriteLine($"-->Track is  mine!!!");
                    mapPage.ShowButtonsOnMap(true); // show buttons 
                    mapPage._canAddPins = true;
                    await Shell.Current.Navigation.PushAsync(mapPage);//1 way 
                    //await Shell.Current.GoToAsync($"chooseMapFromList");//2 way

                }
                else
                {
                    Console.WriteLine($"-->Track is not mine!!!");
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

            if (!await CheckItemOwnership(pin))
            {
                return;
            }

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
