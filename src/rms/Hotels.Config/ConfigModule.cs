using Autofac;

namespace Hotels.Config
{
    public class ConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<FileDefoConfiguration>().As<IDefoConfiguration>().SingleInstance();
        }
    }
}
