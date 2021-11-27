using ADFEditor.Services;
using ADFEditor.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using System.Windows;

namespace ADFEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDialogService, FileDialogService>();
            containerRegistry.Register<IPackageService, PackageService>();
        }
    }
}
