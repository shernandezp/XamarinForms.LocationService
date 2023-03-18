namespace XamarinForms.LocationService.Droid
{
    using Android.App;
    using Android.Content;

    [BroadcastReceiver(Name = "com.locationservice.app.BootBroadcastReceiver", Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(Intent.ActionBootCompleted))
            {
                Intent main = new Intent(context, typeof(MainActivity));
                main.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(main);
            }
        }
    }
}