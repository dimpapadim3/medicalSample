using AspectMap;
using Business.Interfaces;
using Common;
using Logger;
using StructureMap;

namespace Business.Tests
{

    public class CoreTestRegistry : AspectsRegistry
    {
        public CoreTestRegistry()
        {
        
            For<IConfiguration>().Use<DefaultTestConfiguration>();


            SetAllProperties(y => y.OfType<IConfiguration>());
            //SetAllProperties(y => y.OfType<IocConfigurator>());
            For<ILogger>().Use<Log4NetAdapter>();
            SetAllProperties(y => y.OfType<ILogger>());

 
         }
    }

    public class IocConfigurator
    {
        public static void Configure()
        {
            ObjectFactory.Initialize(x =>
                {
                    x.UseDefaultStructureMapConfigFile = true;
                    
                    x.AddRegistry(new CoreTestRegistry());
                    //x.AddRegistry(new IocBussinesRegistry());  
                 
                });
        }

    }
}