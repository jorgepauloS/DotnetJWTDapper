using DotnetJWTDapper.UWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DotnetJWTDapper.UWP.ViewModel
{
    public class UserPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ModelUser> listUsers { get; set; }
        public ObservableCollection<ModelUser> ListUsers
        {
            get
            {
                return listUsers;
            }
            set
            {
                listUsers = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public UserPageViewModel()
        {
            ListUsers = new ObservableCollection<ModelUser>();
        }
    }
}
