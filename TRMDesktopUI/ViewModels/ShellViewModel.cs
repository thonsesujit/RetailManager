using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object> ,IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;


        //Dependency injection.
        //Sales view and events these events are needed to be saved in the instance. 
        public ShellViewModel( IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user)
        {
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _events.Subscribe(this);
            ActivateItem(IoC.Get<LoginViewModel>()); // login view models is per request. IOC is in version control , calibrurn micro biring in . which allows the contrainer to get instances.

        }

        public bool IsLoggedIn
        {
            get {
                bool output = false;
  
                //FIXME : there is an error here that needs to be fixed.
                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }

                return output;
            }
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public void LogOut()
        {
            _user.LogOffUser();
            ActivateItem(IoC.Get<LoginViewModel>()); // login view models is per request. IOC is in version control , calibrurn micro biring in . which allows the contrainer to get instances.
            NotifyOfPropertyChange(() => IsLoggedIn);

        }
        // after successful login we have this event triggered.
        //Conductor activates only one item at a time.
        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
