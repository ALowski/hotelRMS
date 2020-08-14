using System.Collections.Generic;
namespace Hotels.Core.QuadraticProgramming
{
    /// <summary>
    /// Defines input data for quadratic programming problem
    /// </summary>
    public sealed class ProblemInput
    {
        /// <summary>
        /// Number of day
        /// </summary>
        public int D { get;  set; }
        /// <summary>
        /// Number of variables
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// Number of constraints
        /// </summary>
        public int M { get; private set; }

        /// <summary>
        /// The NxN-dimensional symmetric matrix Q of quadratic terms defining the objective function
        /// </summary>
        public double[,] Quadratic { get;  set; }

        /// <summary>
        /// The valued, N-dimensional vector C of linear terms defining the objective function
        /// </summary>
        public double[] Linear { get; set; }

        /// <summary>
        /// The M×N-dimensional constraints matrix A
        /// </summary>
        public double[,] ConstraintMatrix { get; private set; }

        /// <summary>
        /// The M-dimensional constraints values vector B
        /// </summary>
        public double[] ConstraintValues { get; private set; }

        public IEnumerable<MinusIndex> SkipedIndexes { get;  set; }

        public ProblemInput(int n, int m)
        {
            N = n;
            M = m;

            Quadratic = new double[n, n];
            Linear = new double[n];
            ConstraintMatrix = new double[m, n];
            ConstraintValues = new double[m];
        }
    }
    public struct MinusIndex
    {
        public int i;
        public int j;
        public int k;
    }
}