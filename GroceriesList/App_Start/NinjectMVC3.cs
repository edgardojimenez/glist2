using System.Web.Mvc;
using GroceriesList.Entities;
using Ninject.Web.Common;

[assembly: WebActivator.PreApplicationStartMethod(typeof(GroceriesList.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(GroceriesList.App_Start.NinjectMVC3), "Stop")]

namespace GroceriesList.App_Start {
    using System.Reflection;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;

    public static class NinjectMVC3 {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static void Start() {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop() {
            bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel() {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        private static void RegisterServices(IKernel kernel) {
            kernel.Bind<DataContext>().ToSelf().InRequestScope();
            kernel.Bind<IGroceryListRepository>().To<GroceryListEfRepository>();
        }
    }
}
