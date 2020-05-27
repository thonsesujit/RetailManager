using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object> ,IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;

        //Dependency injection.
        //Sales view and events these events are needed to be saved in the instance. 
        public ShellViewModel( IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user ,IAPIHelper apiHelper)
        {
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;
            _events.SubscribeOnPublishedThread(this); // when u call an event, you will get back on same thread.
            ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken()); // login view models is per request. IOC is in version control , calibrurn micro biring in . which allows the contrainer to get instances.

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
            TryCloseAsync();
        }

        public async Task UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new CancellationToken()); // login view models is per request. IOC is in version control , calibrurn micro biring in . which allows the contrainer to get instances.

        }

        public async Task  LogOut()
        {
            _user.ResetUserModel(); //clears information 
            _apiHelper.LogOffUser(); //clears out the header
            await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken()); // login view models is per request. IOC is in version control , calibrurn micro biring in . which allows the contrainer to get instances.
            NotifyOfPropertyChange(() => IsLoggedIn);

        }
        // after successful login we have this event triggered.
        //Conductor activates only one item at a time.
        //public void Handle(LogOnEvent message)
        //{
        //    ActivateItem(_salesVM);
        //    NotifyOfPropertyChange(() => IsLoggedIn);
        //}

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            //the reason for this is. return handle async we need to await, if we are not fully loggedin we dont wanna notify. cancellation token will cancel this task.
            await ActivateItemAsync(_salesVM, cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
