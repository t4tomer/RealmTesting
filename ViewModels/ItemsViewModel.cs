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
    public partial class ItemsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllTasks;

        [ObservableProperty]
        private IQueryable<Item> items;

        [ObservableProperty]
        public string dataExplorerLink = RealmService.DataExplorerLink;

        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;

        public ICommand NavigateCommand { get; private set; }

        public ItemsViewModel()
        {
            realm = RealmService.GetMainThreadRealm();
            currentUserId = RealmService.CurrentUser.Id;
        }


        //!  orginal method-OnAppearing  
        //[RelayCommand]
        //public void OnAppearing()
        //{
        //    Items = realm.All<Item>().OrderBy(i => i.Id);

        //    var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);
        //    IsShowAllTasks = currentSubscriptionType == SubscriptionType.All;
        //}

        //TODO - need to change the delete item 
        //method that elimantes duplicates items with same map name 

 
 

        [RelayCommand]
        public void OnAppearing()
        {
            Console.WriteLine($"IsShowAllTasks is :{IsShowAllTasks} ");

            // Retrieve all items from Realm and convert them to a list.
            var itemsList = realm.All<Item>().ToList();

            // Group the items by Summary and select the first item from each group.
            var distinctItems = itemsList
                .GroupBy(item => item.Summary)
                .Select(group => group.First())
                .OrderBy(item => item.Id)
                .ToList();

            // Assign the filtered list back to Items.
            Items = distinctItems.AsQueryable();

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

        [RelayCommand]
        public async Task AddItem()
        {
            await Shell.Current.GoToAsync($"itemEdit");
        }

        [RelayCommand]
        public async Task EditItem(Item item)
        {
            if (!await CheckItemOwnership(item))
            {
                return;
            }

            var itemParameter = new Dictionary<string, object> { { "item", item } };
            await Shell.Current.GoToAsync($"itemEdit", itemParameter);
        }


        // used to transfer the user to the map page 
        [RelayCommand]
        public async Task ToMapPage() 
        {
            Console.WriteLine($"---> test1 !!!@@@!!! ");
            var itemsWithMap1 = realm.All<Item>().Where(i => i.Mapname == "map1").ToList();

            foreach (var item in itemsWithMap1)
            {
                Console.WriteLine($"The summary of the item is: {item.Summary}");
            }


            // Navigate to the singleton instance of MapPage
            var mapPage = MapPage.Instance;
            List<Maui.GoogleMaps.Pin> pinList = MapPage.Instance.GetPinList();

            int numberOfPins = pinList.Count;
            Console.WriteLine($"--> number of pins(ToMapPage):{numberOfPins}!!!");

            mapPage.ClearMap();
            await Shell.Current.Navigation.PushAsync(mapPage);
        }

        // used to transfer the user to edit point page
        [RelayCommand]
        public async Task ToEditPointPage()
        {



            Console.WriteLine($"---> transfering user to EditPointPage ");
            var page = new PropertyTriggerXaml();
            await Shell.Current.Navigation.PushAsync(page);



        }


        [RelayCommand]
        public async Task ShowTrack(Item item)
        {

            Console.WriteLine($"--->(ShowTrack) item summery:{item.Summary} ");



        }



        //orignal method
        [RelayCommand]
        public async Task DeleteItem(Item item)
        {

            Console.WriteLine($"--->(DeleteItem) item summery:{item.Summary} ");

            if (!await CheckItemOwnership(item))
            {
                return;
            }

            await realm.WriteAsync(() =>
            {
                realm.Remove(item);
            });

        }

        // used to delete map from the items view 
        [RelayCommand]
        public async Task DeleteItems(Item item)
        {

            Console.WriteLine($"--->(DeleteItems) item summery:{item.Summary} ");

            if (!await CheckItemOwnership(item))
            {
                return;
            }

            // Query all items with the same Summary
            var itemsToDelete = realm.All<Item>()
                .Where(i => i.Summary == item.Summary)
                .ToList();

   
                foreach (var itemToDelete in itemsToDelete)
                {
                    await DeleteItem(itemToDelete);
                }
            // Refresh the list after deletion
            OnAppearing();
        }

        [RelayCommand]
        public void Refresh()
        {
            Console.WriteLine($"---> refreshed page ");
            OnAppearing();

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

        private async Task<bool> CheckItemOwnership(Item item)
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
