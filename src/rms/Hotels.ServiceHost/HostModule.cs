using Autofac;
using Hotels.Core;

namespace Hotels.ServiceHost
{
    class HostModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule<CoreModule>();
        }
    }
}
