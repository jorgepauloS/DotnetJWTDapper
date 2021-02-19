using DotnetJWTDapper.UWP.Models;
using DotnetJWTDapper.UWP.Services;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DotnetJWTDapper.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public ModelUser User { get; set; }

        public RegisterPage()
        {
            User = new ModelUser();
            this.InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            ApiClient client = new ApiClient();

            try
            {
                ModelUser user = await client.SignUp(User);
                new Windows.UI.Popups.MessageDialog($"User {user.Name} created with success.").ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                new Windows.UI.Popups.MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
