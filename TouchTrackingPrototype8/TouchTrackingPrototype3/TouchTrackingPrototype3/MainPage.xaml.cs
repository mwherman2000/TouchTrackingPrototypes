using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TouchTrackingPrototype3
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void TrackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TouchTrackerPage());
        }
    }
}
