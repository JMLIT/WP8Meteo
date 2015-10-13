using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration.Pnp;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace WP8Meteo
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel
        {
            get { return this.DataContext as MainPageViewModel; }
            set { this.DataContext = value; }
        }
        private PnpObjectWatcher watcher;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {

        }
    

    /// <summary>
    /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
    /// </summary>
    /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
    /// Ce paramètre est généralement utilisé pour configurer la page.</param>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: préparer la page pour affichage ici.
            var viewModel = new MainPageViewModel();

            if (await viewModel.Init())
            {
                this.ViewModel = viewModel;

            }
            else
            {
                watcher = PnpObject.CreateWatcher(PnpObjectType.DeviceContainer, new string[] { "System.Devices.Connected" }, String.Empty);
                watcher.Updated += watcher_Updated;
                watcher.Start();
            }
        }

        async void watcher_Updated(PnpObjectWatcher sender, PnpObjectUpdate args)
        {
            watcher.Stop();

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await Task.Delay(5000);
                var viewModel = new MainPageViewModel();
                await viewModel.Init();
                this.ViewModel = viewModel;
            });
        }
        // TODO: si votre application comporte plusieurs pages, assurez-vous que vous
        // gérez le bouton Retour physique en vous inscrivant à l’événement
        // Événement Windows.Phone.UI.Input.HardwareButtons.BackPressed.
        // Si vous utilisez le NavigationHelper fourni par certains modèles,
        // cet événement est géré automatiquement.
    

        /*async private void BouConnexion_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BouConnexion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            var viewModel = this.ViewModel;
            this.ViewModel = null;

            await viewModel.Init();
            this.ViewModel = viewModel;

            BouConnexion.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }*/

        async private void Button_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button_Click.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            var viewModel = this.ViewModel;
            this.ViewModel = null;

            await viewModel.Init();
            this.ViewModel = viewModel;

            Button_Click.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
