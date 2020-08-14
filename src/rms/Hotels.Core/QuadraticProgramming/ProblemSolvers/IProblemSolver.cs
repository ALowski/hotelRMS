namespace Hotels.Core.QuadraticProgramming.ProblemSolvers
{
    public interface IProblemSolver
    {
        ProblemOutput[] Solve(ProblemInput[] problem);
        ProblemOutput Solve(ProblemInput problem);
    }
}
