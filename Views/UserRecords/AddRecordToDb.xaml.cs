using Microsoft.Maui.Controls;
using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;
using RealmTodo.Models;


namespace RealmTodo.Views
{

    public partial class AddRecordToDb : ContentPage
    {

        List<Maui.GoogleMaps.Pin> pinsList; // the list of pins in the map
        Maui.GoogleMaps.Map myMap;
        private string _recordUserTime = ""; // Default value

        





        public AddRecordToDb()
        {
            InitializeComponent();
            BindingContext = this; // Set the BindingContext to the current page

        }


        public void setRecordUserTime(string newRecordUserTime)
        {
            _recordUserTime = "time:"+newRecordUserTime; // Update the internal mapTitle field
            Console.WriteLine($"the new record time is--->: {_recordUserTime}");
        }


        public string RecordUserTime
        {
            get => _recordUserTime;
            set
            {
                if (_recordUserTime != value)
                {
                    _recordUserTime = value;
                    OnPropertyChanged(nameof(RecordUserTime)); // Notify the UI about the change
                }
            }
        }



    }

}