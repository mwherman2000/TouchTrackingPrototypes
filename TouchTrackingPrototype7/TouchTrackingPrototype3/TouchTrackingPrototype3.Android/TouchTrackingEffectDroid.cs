using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;
using Android.Content.Res;
using BTTN4KNFE;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchTrackingPlatformEffects.Droid.TouchTrackingEffect), "TouchTrackingEffect")]

namespace TouchTrackingPlatformEffects.Droid
{
    public class TouchTrackingEffect : PlatformEffect
    {
        Android.Views.View view;
        DateTime dtStart;
        DateTime dtEnd;
        DateTime dtPrev;
        BTTN4KNFEValues nfeValues = null;

        protected async override void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnAttached():entered");

            var statusw = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();
            var statusr = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageRead>();

            dtStart = DateTime.UtcNow;
            dtPrev = dtStart;

            view = Control == null ? Container : Control;
            if (view != null)
            {
                view.Touch += OnTouchTrackingHandler;
            }

            if (nfeValues == null) nfeValues = new BTTN4KNFEValues();
        }

        protected override void OnDetached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnDetached():entered");

            dtEnd = DateTime.UtcNow;
            TimeSpan timeSpan = dtEnd - dtStart;
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnDetached(): " + timeSpan.TotalMilliseconds.ToString() + "ms");

            if (view != null)
            {
                view.Touch -= OnTouchTrackingHandler;
                view = null;
            }

            var activity = TouchTrackingPrototype3.Droid.MainActivity.Instance; // mwh
            var nfeJson = BTTN4KNFEFactory.FillTemplate(activity, nfeValues);
            BTTN4KNFEFactory.SaveNfe("mykiss.json", nfeJson); // file path is stored in global variable nfePath
            nfeValues = null;
        }

        void OnTouchTrackingHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.ButtonPress:
                case MotionEventActions.ButtonRelease:
                    {
                        break;
                    }
                case MotionEventActions.Down:
                    {
                        DateTime dtDown = DateTime.UtcNow;
                        TimeSpan timeSpan = dtDown - dtPrev;
                        System.Diagnostics.Debug.WriteLine("OnTouchTrackingHandler: " + args.Event.ActionMasked.ToString() + ": "
                                                                                      + motionEvent.Pressure.ToString() + "\t"
                                                                                      + timeSpan.TotalMilliseconds.ToString() + "ms");
                        dtPrev = dtDown;
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        DateTime dtMove = DateTime.UtcNow;
                        TimeSpan timeSpan = dtMove - dtPrev;
                        System.Diagnostics.Debug.WriteLine("OnTouchTrackingHandler: " + args.Event.ActionMasked.ToString() + ": "
                                                                                      + motionEvent.Pressure.ToString() + "\t"
                                                                                      + timeSpan.TotalMilliseconds.ToString() + "ms");
                        dtPrev = dtMove;
                        break;
                    }
                case MotionEventActions.Up:
                    {
                        DateTime dtUp = DateTime.UtcNow;
                        TimeSpan timeSpan = dtUp - dtPrev;
                        System.Diagnostics.Debug.WriteLine("OnTouchTrackingHandler: " + args.Event.ActionMasked.ToString() + ": "
                                                              + motionEvent.Pressure.ToString() + "\t"
                                                              + timeSpan.TotalMilliseconds.ToString() + "ms");
                        dtPrev = dtUp;
                        break;
                    }
                case MotionEventActions.Outside:
                    {
                        break;
                    }
                case MotionEventActions.HoverEnter:
                case MotionEventActions.HoverExit:
                case MotionEventActions.HoverMove:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
