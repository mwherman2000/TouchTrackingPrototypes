using System.Diagnostics;
using Xamarin.Forms;

namespace TouchTrackingPlatformEffects
{
    public class TouchTrackingEffect : RoutingEffect
    {
        public TouchTrackingEffect() : base("XamarinDocs.TouchTrackingEffect")
        {
            Debug.WriteLine("TouchTrackingEffect():entered");
        }
    }
}
