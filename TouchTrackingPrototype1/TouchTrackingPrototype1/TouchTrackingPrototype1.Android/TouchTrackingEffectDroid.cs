using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchTrackingPlatformEffects.Droid.TouchTrackingEffect), "TouchTrackingEffect")]

namespace TouchTrackingPlatformEffects.Droid
{
    public class TouchTrackingEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnAttached():entered");
        }

        protected override void OnDetached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnDetached():entered");
        }
    }
}
