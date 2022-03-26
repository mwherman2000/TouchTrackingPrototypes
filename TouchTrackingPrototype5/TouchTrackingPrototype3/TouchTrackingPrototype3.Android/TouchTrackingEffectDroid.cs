using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;
using Android.Content.Res;

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
        static string jsonEnvelope = null;
        static string jsonProof = null;

        protected override void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnAttached():entered");

            dtStart = DateTime.UtcNow;
            dtPrev = dtStart;

            view = Control == null ? Container : Control;
            if (view != null)
            {
                view.Touch += OnTouchTrackingHandler;
            }

            if (String.IsNullOrEmpty(jsonEnvelope))
            {
                int proof4 = TouchTrackingPrototype3.Droid.Resource.Raw.proof4;
                int envelope4 = TouchTrackingPrototype3.Droid.Resource.Raw.envelope4;

                jsonProof = LoadJsonTemplate(proof4);
                jsonEnvelope = LoadJsonTemplate(envelope4);
            }

        }

        // https://stackoverflow.com/questions/56744611/get-current-activity-xamarin-forms
        // https://github.com/jamesmontemagno/GifImageView-Xamarin.Android
        // https://github.com/conceptdev/xamarin-forms-samples/blob/main/EmployeeDirectoryXaml/EmployeeDirectoryXaml.Android/MainActivity.cs#L28
        public string LoadJsonTemplate(int resourceId)
        {
            var activity = TouchTrackingPrototype3.Droid.MainActivity.Instance; // mwh
            var inputStream =  activity.Resources.OpenRawResource(resourceId);
            var json = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                json = sr.ReadToEnd();
            }

            return json;
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
        }

        void OnTouchTrackingHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            switch(args.Event.ActionMasked)
            {
                case MotionEventActions.ButtonPress:
                case MotionEventActions.ButtonRelease:
                    {
                        break;
                    }
                case MotionEventActions.Down:
                    {   DateTime dtDown = DateTime.UtcNow;
                        TimeSpan timeSpan = dtDown - dtPrev;
                        System.Diagnostics.Debug.WriteLine("OnTouchTrackingHandler: " + args.Event.ActionMasked.ToString() + ": "
                                                                                      + motionEvent.Pressure.ToString() + "\t"
                                                                                      + timeSpan.TotalMilliseconds.ToString() + "ms");
                        dtPrev = dtDown;
                        break;
                    }
                case MotionEventActions.Move :
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
