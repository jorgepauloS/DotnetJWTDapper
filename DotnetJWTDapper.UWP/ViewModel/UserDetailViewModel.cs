using DotnetJWTDapper.UWP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DotnetJWTDapper.UWP.ViewModel
{
    public class UserDetailViewModel : INotifyPropertyChanged
    {
        private ModelUser selectedUser { get; set; }
        public ModelUser SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                selectedUser = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public UserDetailViewModel()
        {
            SelectedUser = new ModelUser();
        }
    }
}
