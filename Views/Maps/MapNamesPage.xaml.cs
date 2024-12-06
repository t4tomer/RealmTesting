namespace RealmTodo.Views;

public partial class MapNamesPage : ContentPage
{
	public MapNamesPage()
	{
		InitializeComponent();
	}

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        bool newValue = e.Value;
        Console.WriteLine($"IsShowAllTasks toggled to: {newValue}");

    }

}
