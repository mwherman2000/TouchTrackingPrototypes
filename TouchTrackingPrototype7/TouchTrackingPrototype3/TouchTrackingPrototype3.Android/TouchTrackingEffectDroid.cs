using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;
using Android.Content.Res;
using BTTN4KNFE;
using System.Diagnostics;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchTrackingPlatformEffects.Droid.TouchTrackingEffect), "TouchTrackingEffect")]

namespace TouchTrackingPlatformEffects.Droid
{
    public class TouchTrackingEffect : PlatformEffect
    {
        Android.Views.View view;
        BTTN4KNFEValues nfeValues = null;
        DateTime dtNow;
        DateTime dtPrev;
        static MotionEventActions lastAction;

        protected override void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnAttached():entered");

            view = Control == null ? Container : Control;
            if (view != null)
            {
                view.Touch += OnTouchTrackingHandler;
            }

            if (nfeValues == null) nfeValues = new BTTN4KNFEValues();
            dtNow = DateTime.UtcNow;
            nfeValues.tod0approach = dtNow;
            nfeValues.tod1press = dtNow;
            nfeValues.tod2sustain = dtNow;
            nfeValues.tod3release = dtNow;
            nfeValues.tod4recovery = dtNow;
            nfeValues.tod5finish = dtNow;

            nfeValues.d0approachcurve[0] = 0;
            nfeValues.d0approachtime[0] = 0;
            nfeValues.n0samples = 1;

            dtPrev = DateTime.UtcNow;
            lastAction = MotionEventActions.HoverEnter; // placeholder for OnAttached()
        }

        protected override void OnDetached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnDetached():entered");

            TimeSpan timeSpan;
            dtNow = DateTime.UtcNow;
            if (nfeValues.n3samples >= 1)// end the Release curve
            {
                timeSpan = dtNow - dtPrev;
                nfeValues.d3releasecurve[nfeValues.n3samples] = nfeValues.d3releasecurve[nfeValues.n3samples-1];
                nfeValues.d3releasetime[nfeValues.n3samples] = 0;
                nfeValues.n3samples++;
            }
            // initialize the Recovery curve
            nfeValues.tod4recovery = dtNow;
            timeSpan = dtNow - dtPrev;
            nfeValues.d4recoverycurve[nfeValues.n4samples] = 0;
            nfeValues.d4recoverytime[nfeValues.n4samples] = 0;
            nfeValues.n4samples++;
            nfeValues.d4recoverycurve[nfeValues.n4samples] = 0;
            nfeValues.d4recoverytime[nfeValues.n4samples] = (float)timeSpan.TotalMilliseconds;
            nfeValues.n4samples++;

            if (nfeValues.n0samples == 1)
            {
                nfeValues.d0approachcurve[nfeValues.n0samples] = 0;
                nfeValues.d0approachtime[nfeValues.n0samples] = 0;
                nfeValues.n0samples++;
            }

            dtPrev = dtNow;

            nfeValues.tod5finish = dtNow;
            lastAction = MotionEventActions.HoverExit; // placeholder for Detached()

            if (view != null)
            {
                view.Touch -= OnTouchTrackingHandler;
                view = null;
            }

            var activity = TouchTrackingPrototype3.Droid.MainActivity.Instance; // mwh
            var nfeJson = BTTN4KNFEFactory.FillTemplate(activity, nfeValues);
            BTTN4KNFEFactory.SaveNfe("mykiss4.json", nfeJson); // file path is stored in global variable nfePath
            nfeValues = null;
        }

        void OnTouchTrackingHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            Debug.WriteLine(args.Event.ActionMasked.ToString() + "\t" + motionEvent.Pressure.ToString());

            switch (motionEvent.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.ButtonPress:
                    {
                        if (lastAction != MotionEventActions.Down) // reset after each Down that follows an Up or anything else. Curves aren't zeroed
                        {
                            nfeValues.n1samples = 0;
                            nfeValues.n2samples = 0;
                            nfeValues.n3samples = 0;
                            nfeValues.n4samples = 0;
                        }

                        if (nfeValues.n1samples >= BTTN4KNFEValues.MAXSAMPLES) break;

                        TimeSpan timeSpan;
                        dtNow = DateTime.UtcNow;
                        if (nfeValues.n1samples == 0) // first Press - end the Approach curve, initialize Press curve
                        {
                            timeSpan = dtNow - nfeValues.tod0approach; // only for bootstrap from Approach to Press
                            nfeValues.d0approachcurve[nfeValues.n0samples] = 0;
                            nfeValues.d0approachtime[nfeValues.n0samples] = (float)timeSpan.TotalMilliseconds;
                            nfeValues.n0samples++;

                            nfeValues.tod1press = dtNow;
                        }
                        // add point to Press curve
                        timeSpan = dtNow - dtPrev;
                        nfeValues.d1presscurve[nfeValues.n1samples] = motionEvent.Pressure;
                        nfeValues.d1presstime[nfeValues.n1samples] = (float)timeSpan.TotalMilliseconds;
                        nfeValues.n1samples++;
                        dtPrev = dtNow;
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        if (nfeValues.n2samples >= BTTN4KNFEValues.MAXSAMPLES) break;

                        TimeSpan timeSpan;
                        dtNow = DateTime.UtcNow;
                        if (nfeValues.n2samples == 0) // first Move - end the Press curve, initialize the Sustain curve
                        {
                            timeSpan = dtNow - dtPrev;
                            nfeValues.d1presscurve[nfeValues.n1samples] = motionEvent.Pressure;
                            nfeValues.d1presstime[nfeValues.n1samples] = (float)timeSpan.TotalMilliseconds;
                            nfeValues.n1samples++;

                            nfeValues.tod2sustain = dtNow;
                        }
                        // initialize the Sustain curve
                        timeSpan = dtNow - dtPrev;
                        nfeValues.d2sustaincurve[nfeValues.n2samples] = motionEvent.Pressure;
                        nfeValues.d2sustaintime[nfeValues.n2samples] = (float)timeSpan.TotalMilliseconds;
                        nfeValues.n2samples++;
                        dtPrev = dtNow;
                        break;
                    }
                case MotionEventActions.Up:
                case MotionEventActions.ButtonRelease:
                    {
                        if (nfeValues.n3samples >= BTTN4KNFEValues.MAXSAMPLES) break;

                        TimeSpan timeSpan;
                        dtNow = DateTime.UtcNow;

                        if (lastAction == MotionEventActions.Down) // end Press curve, make a dummy Sustain curve
                        {
                            timeSpan = dtNow - dtPrev;
                            nfeValues.d1presscurve[nfeValues.n1samples] = motionEvent.Pressure;
                            nfeValues.d1presstime[nfeValues.n1samples] = (float)timeSpan.TotalMilliseconds;
                            nfeValues.n1samples++;

                            nfeValues.tod2sustain = dtNow;
                            timeSpan = dtNow - dtPrev;
                            nfeValues.d2sustaincurve[nfeValues.n2samples] = motionEvent.Pressure;
                            nfeValues.d2sustaintime[nfeValues.n2samples] = 0;
                            nfeValues.n2samples++;
                            nfeValues.d2sustaincurve[nfeValues.n2samples] = motionEvent.Pressure;
                            nfeValues.d2sustaintime[nfeValues.n2samples] = (float)timeSpan.TotalMilliseconds;
                            nfeValues.n2samples++;
                        }

                        if (nfeValues.n3samples == 0) // first Up - end the Sustain curve, initialize the Release curve
                        {
                            timeSpan = dtNow - dtPrev;
                            nfeValues.d2sustaincurve[nfeValues.n2samples] = motionEvent.Pressure;
                            nfeValues.d2sustaintime[nfeValues.n2samples] = (float)timeSpan.TotalMilliseconds;
                            nfeValues.n2samples++;

                            nfeValues.tod3release = dtNow;
                        }
                        // initialize the Sustain curve
                        timeSpan = dtNow - dtPrev;
                        nfeValues.d3releasecurve[nfeValues.n3samples] = motionEvent.Pressure;
                        nfeValues.d3releasetime[nfeValues.n3samples] = (float)timeSpan.TotalMilliseconds;
                        nfeValues.n3samples++;
                        dtPrev = dtNow;
                        break;
                    }
                case MotionEventActions.Outside:
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
            lastAction = motionEvent.ActionMasked;
        }
    }
}
