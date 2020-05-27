﻿using Caliburn.Micro;
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

        private UserModel _selectedUser;

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set { 
                _selectedUser = value;
                SelectedUserName = value.Email;
                UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                //TODO: calling asny method is not good. this needs to be fixed.
                LoadRoles();
                NotifyOfPropertyChange(() => SelectedUser);

            }
        }

        private string _selectedUserRole;

        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set { 
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);

            }
        }

        private string _selectedAvailableRole;

        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => SelectedAvailableRole);

            }
        }



        private string _selectedUserName;

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);

            }
        }

        private BindingList<string> _userRoles =  new BindingList<string>();

        public BindingList<string> UserRoles
        {
            get { return _userRoles; }
            set {
                _userRoles = value;
                NotifyOfPropertyChange(() => UserRoles);

            }
        }


        private BindingList<string> _availableRoles = new BindingList<string>();

        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);

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
                    await _window.ShowDialogAsync(_status, null, settings);

                }
                else
                {            //we have modified that form
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    //you need to acknowledge the dialog box first
                    await _window.ShowDialogAsync(_status, null, settings);

                }

                TryCloseAsync();
            }
        }

        //to avoid async in constructor. Because constructor is supposed to be fast. 
        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();
            Users = new BindingList<UserModel>(userList);
          
        }

        private async Task LoadRoles()
        {
            var roles = await _userEndpoint.GetAllRoles();

            foreach (var role in roles)
            {
                if (UserRoles.IndexOf(role.Value) < 0)
                {
                    AvailableRoles.Add(role.Value);
                }
            }
        }
        public async void AddSelectedRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);
            UserRoles.Add(SelectedAvailableRole);
            AvailableRoles.Remove(SelectedAvailableRole);
        }

        public async void RemoveSelectedRole()
        {
            await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);
            AvailableRoles.Add(SelectedUserRole);
            UserRoles.Remove(SelectedUserRole);
        }
    }
     

    }





