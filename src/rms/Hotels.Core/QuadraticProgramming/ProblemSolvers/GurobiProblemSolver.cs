using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using Hotels.Core.Tools;

namespace Hotels.Core.QuadraticProgramming.ProblemSolvers
{
    public sealed class GurobiProblemSolver: IProblemSolver
    {
        public ProblemOutput[] Solve(ProblemInput[] problem)
        {
            return null;
        }
        public ProblemOutput Solve(ProblemInput problem)
        {
            try {
              GRBEnv env = new GRBEnv();
              double[] lb = ArrayHelper.CreateArray<double>(problem.Linear.Length);
              double[] sol = ArrayHelper.CreateArray<double>(problem.Linear.Length);
              double val = 0.0;
              var success = GurobiSolve(env, problem.ConstraintMatrix.GetLength(0), problem.ConstraintMatrix.GetLength(1), problem.Linear, problem.Quadratic, problem.ConstraintMatrix, problem.ConstraintValues, lb, null, null, sol,ref val);
              var solution= new ProblemOutput
              {
                  HasSolution = success,
                  Value = success ? val : double.NaN,
                  Solution = success ? sol : Enumerable.Repeat(double.NaN, problem.N).ToArray()
              };  
              // Dispose of environment
              env.Dispose();
              return solution;  
            } catch (GRBException e) {
                return null;
            }
        }    
        
    




      private static bool GurobiSolve(GRBEnv env, int rows, int cols,
                       double[]  c,      // linear portion of objective function
                       double[,] Q,      // quadratic portion of objective function
                       double[,] A,      // constraint matrix
                       double[]  rhs,    // RHS vector
                       double[]  lb,     // variable lower bounds
                       double[]  ub,     // variable upper bounds
                       char[]    vtype,  // variable types (continuous, binary, etc.)
                       double[]  solution,
                       ref double val ) {

        bool success = false;

        try {
          GRBModel model = new GRBModel(env);
    
          // Add variables to the model

          GRBVar[] vars = model.AddVars(lb, ub, null, null, null);
          model.Update();

          // Populate A matrix

          for (int i = 0; i < rows; i++) {
            GRBLinExpr expr = new GRBLinExpr();
            for (int j = 0; j < cols; j++)
              if (A[i,j] != 0)
                expr.AddTerm(A[i,j], vars[j]); // Note: '+=' would be much slower
            model.AddConstr(expr, '>', rhs[i], "");
          }

          // Populate objective

          GRBQuadExpr obj = new GRBQuadExpr();
          if (Q != null) {
            for (int i = 0; i < cols; i++)
              for (int j = 0; j < cols; j++)
                if (Q[i,j] != 0)
                  obj.AddTerm(Q[i,j], vars[i], vars[j]); // Note: '+=' would be much slower
            for (int j = 0; j < cols; j++)
              if (c[j] != 0)
                obj.AddTerm(c[j], vars[j]); // Note: '+=' would be much slower
            model.SetObjective(obj,GRB.MAXIMIZE);
          }

          // Solve model

          model.Optimize();

          // Extract solution

          if (model.Get(GRB.IntAttr.Status) == GRB.Status.OPTIMAL)
          {
              success = true;

              for (int j = 0; j < cols; j++)
                  solution[j] = vars[j].Get(GRB.DoubleAttr.X);
          }
          else
              success = false;
          val=obj.Value;  
          model.Dispose();

        } catch (GRBException e) {
            return false;
        }

        return success;
      }
    }

}
