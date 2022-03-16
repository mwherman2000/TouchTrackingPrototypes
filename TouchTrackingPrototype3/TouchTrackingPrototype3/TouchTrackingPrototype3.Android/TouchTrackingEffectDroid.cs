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
        Android.Views.View view;

        protected override void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnAttached():entered");

            view = Control == null ? Container : Control;
            if (view != null)
            {
                view.Touch += OnTouchTrackingHandler;
            }

        }

        protected override void OnDetached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnDetached():entered");

            if (view != null)
            {
                view.Touch -= OnTouchTrackingHandler;
            }
        }

        void OnTouchTrackingHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            System.Diagnostics.Debug.WriteLine("OnTouchTrackingHandler: " + args.Event.Action.ToString() + ": "
                + args.Event.ActionMasked.ToString() + ": "
                + motionEvent.Pressure.ToString());
        }
    }
}
