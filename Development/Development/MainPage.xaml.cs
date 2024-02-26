using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Development
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Animate();
        }

        public async Task Animate()
        {
            img.Opacity = 0;
            await img.FadeTo(1, 1000);
            Application.Current.MainPage = new NavigationPage(new SecondPage());

        }
    }
}
