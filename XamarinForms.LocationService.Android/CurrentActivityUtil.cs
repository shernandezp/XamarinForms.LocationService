﻿// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Runtime;
using Android.Util;
using Java.Lang;
using Java.Util;

namespace XamarinForms.LocationService.Android;

internal static class CurrentActivityUtil
{
    public static Activity GetCurrentActivity()
    {
        Activity activity = null;
        List<Object> objects = null;

        var activityThreadClass = Class.ForName("android.app.ActivityThread");
        var activityThread = activityThreadClass.GetMethod("currentActivityThread").Invoke(null);
        var activityFields = activityThreadClass.GetDeclaredField("mActivities");
        activityFields.Accessible = true;

        var obj = activityFields.Get(activityThread);

        if (obj is JavaDictionary)
        {
            var activities = (JavaDictionary)obj;
            objects = new List<Object>(activities.Values.Cast<Object>().ToList());
        }
        else if (obj is ArrayMap)
        {
            var activities = (ArrayMap)obj;
            objects = new List<Object>(activities.Values().Cast<Object>().ToList());
        }
        else if (obj is IMap)
        {
            var activities = (IMap)activityFields.Get(activityThread);
            objects = new List<Object>(activities.Values().Cast<Object>().ToList());
        }

        if (objects != null && objects.Any())
        {
            foreach (var activityRecord in objects)
            {
                var activityRecordClass = activityRecord.Class;
                var pausedField = activityRecordClass.GetDeclaredField("paused");
                pausedField.Accessible = true;

                if (!pausedField.GetBoolean(activityRecord))
                {
                    var activityField = activityRecordClass.GetDeclaredField("activity");
                    activityField.Accessible = true;
                    activity = (Activity)activityField.Get(activityRecord);
                    break;
                }
            }
        }

        return activity;
    }
}
