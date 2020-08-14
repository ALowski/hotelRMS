using Autofac;
using Hotels.Config;
using Hotels.Core.DaytimeExtrapolators;
using Hotels.Core.DemandSlopePredictors;
using Hotels.Core.ForecastRounders;
using Hotels.Core.LoadPredictors;
using Hotels.Core.QuadraticProgramming.ProblemSolvers;
using Hotels.Core.QuadraticProgramming.ProblemTransformers;

namespace Hotels.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule<ConfigModule>();
            builder.RegisterType<DiscreteDaytimeExtrapolator>().As<IDaytimeExtrapolator>();
            builder.RegisterType<PreserveForecastRounder>().As<IForecastRounder>();
            builder.RegisterType<LeastSquareDemandSlopePredictor>().As<IDemandSlopePredictor>();
            builder.RegisterType<GeneralLoadPredictor>().As<ILoadPredictor>();
            builder.RegisterType<AccordProblemSolver>().As<IProblemSolver>();
            builder.RegisterType<MatLabProblemSolver>().As<IProblemSolver>();
            builder.RegisterType<MatLabProblemTransformer>().As<IProblemTransformer>();
            builder.RegisterType<Solver>().As<ISolver>();
        }
    }
}
