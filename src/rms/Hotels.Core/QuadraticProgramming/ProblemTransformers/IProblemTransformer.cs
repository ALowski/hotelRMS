using System.Collections.Generic;
using Hotels.Basic;
using Hotels.Config.ConfigModel;

namespace Hotels.Core.QuadraticProgramming.ProblemTransformers
{
    public interface IProblemTransformer
    {
        // todo - rename input parameters
        ProblemInput Transform(int day, int roomTypes, int categoryTypes, int mealTypes, IEnumerable<ChildRooms> childRooms, double[, ,] a, double[, ,] b, double[,] h,
            double[, ,] L, double[, ,] U, double[] R, double w, PriceRelation[] T, bool withACondition = true);
        ProblemInput TransformForW(int day, int roomTypes, int categoryTypes, int mealTypes, double[, ,] a, double[, ,] b, double[,] h, double[, ,] L, double[, ,] U, double[] R, bool withACondition = true);
    }
}
