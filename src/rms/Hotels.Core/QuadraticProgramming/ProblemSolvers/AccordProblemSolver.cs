using System.Linq;
using Accord.Math.Optimization;

namespace Hotels.Core.QuadraticProgramming.ProblemSolvers
{
    public sealed class AccordProblemSolver : IProblemSolver
    {
        public ProblemOutput[] Solve(ProblemInput[] problem)
        {
            return null;
        }
        public ProblemOutput Solve(ProblemInput problem)
        {
            var goldfarbIdnani = new GoldfarbIdnani(problem.Quadratic, problem.Linear, problem.ConstraintMatrix, problem.ConstraintValues);

            var hasSolution = goldfarbIdnani.Maximize();

            return new ProblemOutput
            {
                HasSolution = hasSolution,
                Value=hasSolution ? goldfarbIdnani.Value:double.NaN,
                Solution = hasSolution ? goldfarbIdnani.Solution : Enumerable.Repeat(double.NaN, problem.N).ToArray()
            };
        }
    }
}
