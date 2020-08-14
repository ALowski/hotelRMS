using System.Collections.Generic;
namespace Hotels.Core.QuadraticProgramming
{
    /// <summary>
    /// Defines output result of quadratic programming problem
    /// </summary>
    public sealed class ProblemOutput
    {
        /// <summary>
        /// Number of day
        /// </summary>
        public int D { get; set; }
        /// <summary>
        /// Indicates whether the problem solution was found
        /// </summary>
        public bool HasSolution { get; set; }

        /// <summary>
        /// Gets the solution found, the values of the parameters which optimizes the function
        /// </summary>
        public double[] Solution { get; set; }

        public double Value { get; set; }

        public IEnumerable<MinusIndex> SkipedIndexes { get; set; }
    }
}
