using System.Windows;

namespace MyAiLanguageCompanion;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = ServiceProviderFactory.Create();
        var mainWindow = new MainWindow(services);
        mainWindow.Show();
    }
}
