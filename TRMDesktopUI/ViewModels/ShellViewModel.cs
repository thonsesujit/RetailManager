using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object> ,IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;


        //Dependency injection.
        //Sales view and events these events are needed to be saved in the instance. 
        public ShellViewModel( IEventAggregator events, SalesViewModel salesVM)
        {
            _events = events;
            _salesVM = salesVM;
            _events.Subscribe(this);
            ActivateItem(IoC.Get<LoginViewModel>()); // login view models is per request. IOC is in version control , calibrurn micro biring in . which allows the contrainer to get instances.

        }

        // after successful login we have this event triggered.
        //Conductor activates only one item at a time.
        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
        }
    }
}
