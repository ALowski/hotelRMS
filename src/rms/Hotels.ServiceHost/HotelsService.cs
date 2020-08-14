using System.ServiceProcess;
using Autofac;

namespace Hotels.ServiceHost
{
    public class HotelsService : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            DoStart();
        }

        public void DoStart()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<HostModule>();
            var lifetimeScope = builder.Build();
        }

        protected override void OnStop()
        {
            DoStop();
        }

        public void DoStop()
        {
        }
    }
}
