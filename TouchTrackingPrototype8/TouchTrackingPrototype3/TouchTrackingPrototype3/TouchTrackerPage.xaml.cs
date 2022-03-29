using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TouchTrackingPrototype3
{
    public partial class TouchTrackerPage : ContentPage
    {
        public TouchTrackerPage()
        {
            InitializeComponent();
        }
        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new DonePage());
            await Navigation.PopAsync();
        }
    }
}
