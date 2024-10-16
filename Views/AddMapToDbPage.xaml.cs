using Microsoft.Maui.Controls;
using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;

namespace RealmTodo.Views
{

    public partial class AddMapToDbPage : ContentPage
    {

        List<Maui.GoogleMaps.Pin> pinsList; // the list of pins in the map
        Maui.GoogleMaps.Map myMap;





        public AddMapToDbPage()
        {
            InitializeComponent();
        }

        public AddMapToDbPage(List<Maui.GoogleMaps.Pin> newPinsList)
        {
            InitializeComponent();
        }

        public AddMapToDbPage(List<Maui.GoogleMaps.Pin> newPinsList, Maui.GoogleMaps.Map newMyMap)
        {
            InitializeComponent();
            this.pinsList = newPinsList;
            this.myMap = newMyMap;

        }




    }

}