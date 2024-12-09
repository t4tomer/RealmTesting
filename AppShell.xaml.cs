using RealmTodo.Views;

namespace RealmTodo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("itemEdit", typeof(EditItemPage));
        Routing.RegisterRoute("mapEdit", typeof(EditMapPage));

    }
}

