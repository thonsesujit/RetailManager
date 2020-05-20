using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.ViewModels;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Library.Helpers;
using AutoMapper;
using TRMDesktopUI.Models;
using System.Xml;

namespace TRMDesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        //In dependencey injection it will control the instation  of classes. eg: .. = new class(), this thing will be taken
        // care by the container
        private SimpleContainer _container = new SimpleContainer();


        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }


        private IMapper ConfigureAutomapper()
        {
            //Automapper will know at the start of the application. its gonna use reflexion . that it has to map data from one model to other.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductModel, ProductDisplayModel>(); // from product model to product display model.
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            }
            );

            var output = config.CreateMapper(); // takes the configuration and creates a mapper. 

            return output;
        }

        //if you ask for container instance. it will return this instance. We may need to get this container ti manupulate it, etc.
        protected override void Configure()
        {
                      

            _container.Instance(ConfigureAutomapper()); // creates a singleton of IMapper.

            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>()
                .PerRequest<ISaleEndpoint, SaleEndpoint>();

            //specially for caliburn micro
            _container
                //handling window in an out
                .Singleton<IWindowManager, WindowManager>()
                // once piece an raise an event and other piece and handles the even. Its built in Caliburn micro.
                .Singleton<IEventAggregator, EventAggregator>()
                //singleton for http client.
                .Singleton<IAPIHelper, APIHelper>()
                .Singleton<IConfigHelper, ConfigHelper>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>();

            //wiring view models to views. We will use reflextion. There is a small performance hit on startup. 
            // as its on startup.

            //get all the the types for our entiere application, Where is the filter : class types, name of class ends with ViewModel
            // then take that list and create a list. 
            // then thanking for each contrainer and register per request, sheeview model. shellview model name and then again shellview model.
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));

        }




        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //on startup we gonna start ViewModel. 
            DisplayRootViewFor<ShellViewModel>();
        }

        //it passes a type and name . Calibre micro uses convention finds location to give shell view model. here the container gives
        // shell view model
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        //it constructs things
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
