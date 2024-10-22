namespace RealmTodo.Views;

public partial class EditItemPage : ContentPage, IQueryAttributable
{
    public EditItemPage()
    {
        InitializeComponent();
        //TextBox.IsVisible = false;

    }

    public EditItemPage(bool ans)
    {
        InitializeComponent();
        TextBox.IsVisible = ans;
    }

    //remove the OkButton and Text box from user's 
    public void EditDisplay(bool ans)
    {
        //InitializeComponent();
        TextBox.IsVisible = ans;
        OkButton.IsVisible = ans;

    }

    // Implement the IQueryAttributable interface to handle query parameters
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("isEditVisible") &&
            bool.TryParse(query["isEditVisible"].ToString(), out bool isVisible))
        {
            EditDisplay(isVisible);
        }
    }

    //public void Test()
    //{
    //    Console.WriteLine($"---> Edit Item Page--Test ");

    //}


}
