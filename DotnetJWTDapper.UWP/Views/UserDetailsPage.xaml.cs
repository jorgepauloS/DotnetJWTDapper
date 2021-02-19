using DotnetJWTDapper.UWP.Services;
using DotnetJWTDapper.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class UserDetailsPage : Page
    {
        public UserDetailViewModel ViewModel { get; set; }
        public UserDetailsPage()
        {
            ViewModel = new UserDetailViewModel();
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ApiClient client = new ApiClient();
            try
            {
                ViewModel.SelectedUser = await client.GetUser(App.UserLogged, e.Parameter.ToString());
            }
            catch (Exception ex)
            {
                new Windows.UI.Popups.MessageDialog(ex.Message).ShowAsync();
                if (ex.Message.Equals("You need to signin again!"))
                {
                    Frame.Navigate(typeof(MainPage));
                }
            }
            base.OnNavigatedTo(e);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserEditPage), ViewModel.SelectedUser.Id);
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog($"Delete user {ViewModel.SelectedUser.Id}?");

            messageDialog.Commands.Add(new UICommand("Confirm",
                new UICommandInvokedHandler(this.DeleteUser)));
            messageDialog.Commands.Add(new UICommand("Close"));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void DeleteUser(IUICommand command)
        {
            ApiClient client = new ApiClient();
            try
            {
                await client.DeleteUser(App.UserLogged, ViewModel.SelectedUser.Id);
                new Windows.UI.Popups.MessageDialog($"User {ViewModel.SelectedUser.Id} deleted successfully").ShowAsync();
                Frame.Navigate(typeof(UsersPage));
            }
            catch (Exception ex)
            {
                new Windows.UI.Popups.MessageDialog(ex.Message).ShowAsync();
                if (ex.Message.Equals("You need to signin again!"))
                {
                    Frame.Navigate(typeof(MainPage));
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UsersPage));
        }
    }
}
