using DotnetJWTDapper.UWP.Models;
using DotnetJWTDapper.UWP.Services;
using DotnetJWTDapper.UWP.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DotnetJWTDapper.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            ApiClient client = new ApiClient();

            try
            {
                ModelUser user = await client.Login(Email, Password);
                App.UserLogged = user;
                this.Frame.Navigate(typeof(UsersPage));
            }
            catch (Exception ex)
            {
                new Windows.UI.Popups.MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }
    }
}
