using RealmTodo.Views;

namespace RealmTodo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("mapEdit", typeof(EditMapPage));
        Routing.RegisterRoute("chooseMapFromList", typeof(MapPage));//TODO fix this 



    }
}

