namespace XamarinForms.LocationService.ViewModels
{
    using System.Threading.Tasks;
    using Xamarin.Essentials;
    using Xamarin.Forms;
    using XamarinForms.LocationService.Messages;
    using XamarinForms.LocationService.Utils;

    public class MainPageViewModel : BaseViewModel
    {
        #region vars
        private double latitude;
        private double longitude;
        public string userMessage;
        public bool startEnabled;
        public bool stopEnabled;
        #endregion vars

        #region properties
        public double Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }
        public double Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }
        public string UserMessage
        {
            get => userMessage;
            set => SetProperty(ref userMessage, value);
        }
        public bool StartEnabled
        {
            get => startEnabled;
            set => SetProperty(ref startEnabled, value);
        }
        public bool StopEnabled
        {
            get => stopEnabled;
            set => SetProperty(ref stopEnabled, value);
        }
        #endregion properties

        #region commands
        public Command StartCommand { get; }
        public Command EndCommand { get; }
        #endregion commands

        public MainPageViewModel()
        {
            StartCommand = new Command(async () => await OnStartClick());
            EndCommand = new Command(async() => await OnStopClick());
            HandleReceivedMessages();
            StartEnabled = true;
            StopEnabled = false;
        }

        public async Task OnStartClick()
        {
            await Start();
        }

        public async Task OnStopClick()
        {
            var message = new StopServiceMessage();
            MessagingCenter.Send(message, "ServiceStopped");
            UserMessage = "Location Service has been stopped!";
            await SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "0");
            StartEnabled = true;
            StopEnabled = false;
        }

        public async Task ValidateStatus() 
        {
            var status = await SecureStorage.GetAsync(Constants.SERVICE_STATUS_KEY);
            if (status != null && status.Equals("1")) 
            {
                await Start();
            }
        }

        async Task Start() 
        {
            var message = new StartServiceMessage();
            MessagingCenter.Send(message, "ServiceStarted");
            UserMessage = "Location Service has been started!";
            await SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "1");
            StartEnabled = false;
            StopEnabled = true;
        }

        void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<LocationMessage>(this, "Location", message => {
                Device.BeginInvokeOnMainThread(() => {
                    Latitude = message.Latitude;
                    Longitude = message.Longitude;
                    UserMessage = "Location Updated";
                });
            });
            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message => {
                Device.BeginInvokeOnMainThread(() => {
                    UserMessage = "Location Service has been stopped!";
                });
            });
            MessagingCenter.Subscribe<LocationErrorMessage>(this, "LocationError", message => {
                Device.BeginInvokeOnMainThread(() => {
                    UserMessage = "There was an error updating location!";
                });
            });
        }
    }
}
