using Microsoft.Maui.Controls;
using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;
using RealmTodo.Models;
using CommunityToolkit.Mvvm.Input;


namespace RealmTodo.Views
{

    public partial class AddRecordToDb : ContentPage
    {

        List<Maui.GoogleMaps.Pin> pinsList; // the list of pins in the map
        Maui.GoogleMaps.Map myMap;
        private string _recordUserTime = ""; //
        private string _trackName = ""; // Default value
        private string _formattedTime = "";
        private string _formattedDate = "";


        public string CurrentTime
        {
            get => GetCurrentTime();
            set
            {
                if (_formattedTime != value)
                {
                    _formattedTime = value;
                    OnPropertyChanged(nameof(CurrentTime)); // Notify the UI about the change
                }
            }
        }

        public string CurrentDate
        {
            get => GetCurrentDate();
            set
            {
                if (_formattedDate != value)
                {
                    _formattedDate = value;
                    OnPropertyChanged(nameof(CurrentDate)); // Notify the UI about the change
                }
            }
        }


        private static string GetCurrentTime()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            //string formattedDateTime = now.ToString("HH:mm:ss dd/MM/yyyy ");
            string formattedTime = now.ToString("HH:mm:ss ");



            return formattedTime;
        }


        private static string GetCurrentDate()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            string formattedDate = now.ToString("dd/MM/yyyy ");



            return formattedDate;
        }


        public AddRecordToDb()
        {
            InitializeComponent();

            BindingContext = this; // Set the BindingContext to the current page

            // Navigate to the singleton instance of TimerPage
            //var timerPage = TimerPage.Instance;
            //timerPage.ResetTimer();

        }


        public void setRecordUserTime(string newRecordUserTime)
        {
            _recordUserTime = newRecordUserTime; // Update the internal mapTitle field

            BindingContext = null;
            BindingContext = this; // Reset BindingContext to refresh bindings

            Console.WriteLine($"the new record time is--->: {_recordUserTime}");
        }

        public void setTrackName(string newTrackName)
        {
            _trackName = newTrackName;

            BindingContext = null;
            BindingContext = this; // Reset BindingContext to refresh bindings



            Console.WriteLine($"---------> the track name is : {_trackName}");

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



        public string TrackName
        {

            get => _trackName;
            set
            {
                if (_trackName != value)
                {
                    _trackName = value;
                    OnPropertyChanged(nameof(TrackName)); // Notify the UI about the change
                }
            }
        }


        public string TrackNameTitle
        {

            get => $"Map: {_trackName}";
            set
            {
                if (_trackName != value)
                {
                    _trackName = value;
                    OnPropertyChanged(nameof(TrackName)); // Notify the UI about the change
                }
            }
        }


        [RelayCommand]

        public async Task OkPressed()
        {
            Console.WriteLine($"the track name is---> :{_trackName} ");
            Console.WriteLine($"the TrackName is---> :{TrackName} ");

        }



    }

}