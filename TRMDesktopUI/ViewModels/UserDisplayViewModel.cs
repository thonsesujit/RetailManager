using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndPoint _userEndpoint;

        BindingList<UserModel> _users;

        public BindingList<UserModel> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndPoint userEndpoint)
        {
            _status = status;
            _window = window;
            _userEndpoint = userEndpoint;
        }

        //
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                //setting for message box
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "SystemError";

                //this is an option instead of calling it from the constructor. we can have an instance inside your method.
                //var info = IoC.Get<StatusInforViewModel>();

                if (ex.Message == "Unauthorized")
                {
                    //we have modified that form
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales form");
                    //you need to acknowledge the dialog box first
                    _window.ShowDialog(_status, null, settings);

                }
                else
                {            //we have modified that form
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    //you need to acknowledge the dialog box first
                    _window.ShowDialog(_status, null, settings);

                }

                TryClose();
            }
        }

        //to avoid async in constructor. Because constructor is supposed to be fast. 
        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();
            Users = new BindingList<UserModel>(userList);
          
        }

     
    }


}


