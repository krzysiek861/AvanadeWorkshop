using Autofac;
using Autofac.Integration.Mvc;
using Avanade.AzureWorkshop.WebApp.BusinessLogic;
using Avanade.AzureWorkshop.WebApp.Services;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Avanade.AzureWorkshop.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterDependencies();
            InitializeSecrets();
        }

        protected void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<GamesService>();
            builder.RegisterType<TeamsService>();
            builder.RegisterType<PlayersService>();
            builder.RegisterType<SendgridService>();
            builder.RegisterType<NewsletterService>();
            builder.RegisterType<TeamsRepository>();
            builder.RegisterType<ImagesService>();
            builder.RegisterType<BinaryFilesRepository>();
            builder.RegisterType<CsvReader>(); 
            builder.RegisterGeneric(typeof(TopicService<>));
            builder.RegisterType<TelemetryService>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected void InitializeSecrets()
        {
            var manager = new SecretsManager();
            manager.Initialize();
        }
    }
}
