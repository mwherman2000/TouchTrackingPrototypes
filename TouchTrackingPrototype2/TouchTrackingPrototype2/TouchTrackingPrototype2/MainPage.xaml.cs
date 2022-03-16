using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;

namespace TouchTrackingPrototype2
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DonePage());
        }
    }
}
