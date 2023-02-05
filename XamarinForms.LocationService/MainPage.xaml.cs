namespace XamarinForms.LocationService
{
    using Xamarin.Forms;
    using XamarinForms.LocationService.ViewModels;

    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel vm;
        readonly ILocationConsent locationConsent;

        public MainPage()
        {
            InitializeComponent();
            locationConsent = DependencyService.Get<ILocationConsent>();
            vm = new MainPageViewModel();
            BindingContext = vm;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await locationConsent.GetLocationConsent();
            await vm.ValidateStatus();
        }
    }
}
