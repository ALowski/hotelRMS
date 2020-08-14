using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using Hotels.Core.Tools;
using MathWorks.MATLAB.NET.Arrays;
using SolveQP;

namespace Hotels.Core.QuadraticProgramming.ProblemSolvers
{
    public sealed class MatLabProblemSolver : IProblemSolver
    {
        public ProblemOutput[] Solve(ProblemInput[] problem)
        {
            ProblemOutput[] solutions = new ProblemOutput[problem.Length];
            var inputLength = problem.Where(c => c.N > 0).Count();
            MWArray[] input = new MWArray[4 * inputLength];
            int index = 0;
            for (int i = 0; i < problem.Length; i++)
            {
                if (problem[i].N > 0)
                {
                    MWNumericArray _h = problem[i].Quadratic;
                    MWNumericArray _f = problem[i].Linear;
                    MWNumericArray _A = problem[i].ConstraintMatrix;
                    MWNumericArray _B = problem[i].ConstraintValues;
                    input[index * 4] = _h;
                    input[index * 4 + 1] = _f;
                    input[index * 4 + 2] = _A;
                    input[index * 4 + 3] = _B;
                    index++;
                }

                solutions[i] = new ProblemOutput
                {
                    D=problem[i].D,
                    HasSolution = false,
                    Value = double.NaN,
                    Solution = Enumerable.Repeat(double.NaN, problem[i].N).ToArray(),
                    SkipedIndexes=problem[i].SkipedIndexes
                };                
            }
            try
            {
                if (inputLength > 0)
                {
                    SolveQPClass r = new SolveQPClass();
                    MWArray[] result = new MWArray[3 * inputLength];
                    r.SolveQP(result.Length, ref result, input);
                    index = 0;
                    for (int i = 0; i < problem.Length; i++)
                    {
                        if (problem[i].N > 0)
                        {
                            bool success = ((MWNumericArray)result[index * 3 + 2]).ToScalarDouble() > 0;
                            if (success)
                            {
                                double[] x = (double[])(((MWNumericArray)result[index * 3]).ToVector(MWArrayComponent.Real));
                                double val = ((MWNumericArray)result[index * 3 + 1]).ToScalarDouble();
                                solutions[i].HasSolution = success;
                                solutions[i].Value = -val;
                                solutions[i].Solution = x;
                            }
                            index++;
                        }
                    }
                    r.Dispose();
                }
                return solutions;
            }
            catch (Exception e)
            {
                return solutions;
            }
        }
        public ProblemOutput Solve(ProblemInput problem)
        {
            ProblemOutput solution = new ProblemOutput
                {
                    D = problem.D,
                    HasSolution = false,
                    Value = double.NaN,
                    Solution = Enumerable.Repeat(double.NaN, problem.N).ToArray(),
                    SkipedIndexes = problem.SkipedIndexes
                };
            if (problem.N > 0)
            {
                MWArray[] input = new MWArray[4];    
                MWNumericArray _h = problem.Quadratic;
                MWNumericArray _f = problem.Linear;
                MWNumericArray _A = problem.ConstraintMatrix;
                MWNumericArray _B = problem.ConstraintValues;
                input[0] = _h;
                input[1] = _f;
                input[2] = _A;
                input[3] = _B;
                try
                {
                    SolveQPClass r = new SolveQPClass();
                    MWArray[] result = new MWArray[3];
                    r.SolveQP(result.Length, ref result, input);
                    bool success = ((MWNumericArray)result[2]).ToScalarDouble() > 0;
                    if (success)
                    {
                        double[] x = (double[])(((MWNumericArray)result[0]).ToVector(MWArrayComponent.Real));
                        double val = ((MWNumericArray)result[1]).ToScalarDouble();
                        solution.HasSolution = success;
                        solution.Value = -val;
                        solution.Solution = x;
                    }
                    r.Dispose();
                }
                catch (Exception e)
                {
                    return solution;
                }
            }
            return solution;
        }
    }

}
