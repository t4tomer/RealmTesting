﻿namespace RealmTodo.Views;

public partial class ItemsPage : ContentPage
{
	public ItemsPage()
	{
		InitializeComponent();
	}

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        bool newValue = e.Value;
        Console.WriteLine($"IsShowAllTasks toggled to: {newValue}");

    }

}
