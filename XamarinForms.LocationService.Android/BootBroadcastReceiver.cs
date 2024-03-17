// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Android;
using Android.App;
using Android.Content;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]
namespace XamarinForms.LocationService.Droid;

[BroadcastReceiver(Name = "com.locationservice.app.BootBroadcastReceiver", Enabled = true, Exported = true)]
[IntentFilter([Intent.ActionBootCompleted])]
public class BootBroadcastReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Action.Equals(Intent.ActionBootCompleted))
        {
            Intent main = new(context, typeof(MainActivity));
            main.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(main);
        }
    }
}