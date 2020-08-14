using System;
using System.Collections.Generic;
using System.Linq;
using Hotels.Basic;
using Hotels.Config.ConfigModel;

/*
*  max sum(i in room)sum(j in cat)((a[i][j] + b[i][j]*p[i][j])*(p[i][j] - h[i]) - W*y[i][j]*y[i][j])
*  L[i][j] <= p[i][j] <= U[i][j] + y[i][j]
*  a[i][j] + b[i][j]*p[i][j] >= 0
*  p[i][j] - h[i] >= 0
*  sum(j in cat)(a[i][j] + b[i][j]*p[i][j]) <= R[i]
*  p[t1][t2] <= p[t3][t4]
*/

namespace Hotels.Core.QuadraticProgramming.ProblemTransformers
{
    public sealed class MatLabProblemTransformer : IProblemTransformer
    {
        // maximize x'*Q*x + C'*x subject to Ax >= B

        public ProblemInput Transform(int day, int roomTypes, int categoryTypes, int mealTypes,IEnumerable<ChildRooms> childRooms, double[, ,] a, double[, ,] b, double[,] h,
            double[, ,] L, double[, ,] U, double[] R, double w, PriceRelation[] T, bool withACondition = true)
        {

            var allTypes = roomTypes * categoryTypes * mealTypes;
            var allTypes2 = allTypes;

            int minusIndex = 0;
            int[, ,] minusIndexes = new int[roomTypes, categoryTypes, mealTypes];

            var skipedIndexes = GetSkipedIndexes(roomTypes, categoryTypes, mealTypes, a, b, L, U, R);

            for (var i = 0; i < roomTypes; ++i)
                for (var j = 0; j < categoryTypes; ++j)
                    for (var k = 0; k < mealTypes; ++k)
                        if (skipedIndexes.Any(s => s.i == i && s.j == j && s.k == k))
                            minusIndex++;
                        else
                            minusIndexes[i, j, k] = minusIndex;
                    

            allTypes2 -= minusIndex;
            var n = 2 * allTypes2;
            var m = 4 * allTypes2 + roomTypes + (T.Length + (childRooms.Count() + childRooms.Select(v => v.Parent).Distinct().Count())*mealTypes) * categoryTypes;
            var result = new ProblemInput(n, m);
            result.D = day;
            result.SkipedIndexes = skipedIndexes;
            var q = result.Quadratic;
            var c = result.Linear;
            var A = result.ConstraintMatrix;  // A
            var B = result.ConstraintValues;  // B
            double[] debug = new double[allTypes2];
            minusIndex = 0;

            for (var i = 0; i < roomTypes; ++i)
            {
                B[3 * allTypes2 + i] = -R[i];
                for (var j = 0; j < categoryTypes; ++j)
                    for (var k = 0; k < mealTypes; ++k)
                        if (!skipedIndexes.Any(s => s.i == i && s.j == j && s.k == k))
                        {
                            var offset = i * categoryTypes * mealTypes + j * mealTypes + k - minusIndex;

                            q[offset, offset] = 2 * b[i, j, k];
                            c[offset] = a[i, j, k] - b[i, j, k] * h[i, k];

                            c[allTypes2 + offset] = -w;
                            A[offset, offset] = 1;
                            A[offset, allTypes2 + offset] = 1;
                            var lowerBound = L[i, j, k] > h[i, k] ? L[i, j, k] : h[i, k];

                            B[offset] = lowerBound > 0 ? lowerBound : 0;

                            A[allTypes2 + offset, offset] = -1;
                            A[allTypes2 + offset, allTypes2 + offset] = 1;
                            B[allTypes2 + offset] = -U[i, j, k];
                            if (withACondition)
                            {
                                A[(2 * allTypes2 + offset), offset] = b[i, j, k];
                                B[2 * allTypes2 + offset] = -a[i, j, k];
                            }

                            //Group rooms restriction with vacant rooms number
                            B[3 * allTypes2 + i] += a[i, j, k];
                            A[3 * allTypes2 + i, offset] = -b[i, j, k];

                            if (childRooms.Any(v => v.Parent == i))
                            {
                                var childs=childRooms.Where(v=>v.Parent==i).ToList();
                                var total=childs.Sum(v=>v.Quantity);
                                foreach (var child in childs)
                                {
                                    if (!skipedIndexes.Any(s => s.i == child.Child && s.j == j && s.k == k))
                                    {
                                        var koef =((double) child.Quantity) / (childRooms.Where(v => v.Child == child.Child).Sum(v => v.Quantity) * total);
                                        B[3 * allTypes2 + i] += koef * (a[child.Child, j, k] + (child.Quantity * R[i] - R[child.Child]));
                                        var newOffset = child.Child * categoryTypes * mealTypes + j * mealTypes + k - minusIndexes[child.Child, j, k];
                                        A[3 * allTypes2 + i, newOffset] = -b[child.Child, j, k] * koef;
                                    }
                                }
                            }
                            else if (childRooms.Any(v => v.Child == i))
                            {
                                foreach (var parent in childRooms.Where(v => v.Child == i))
                                {
                                    if (!skipedIndexes.Any(s => s.i == parent.Parent && s.j == j && s.k == k))
                                    {
                                        B[3 * allTypes2 + i] += parent.Quantity * a[parent.Parent, j, k];
                                        var newOffset = parent.Parent * categoryTypes * mealTypes + j * mealTypes + k - minusIndexes[parent.Parent, j, k];
                                        A[3 * allTypes2 + i, newOffset] = -b[parent.Parent, j, k] * parent.Quantity;
                                    }
                                }
                            }                            
                            B[3 * allTypes2 + roomTypes + offset] = 0;
                            A[3 * allTypes2 + roomTypes + offset, allTypes2 + offset] = 1;
                        }
                        else
                            minusIndex++;
            }

            int index = 1;
            for (var i = 0; i < T.Length; ++i)
                for (var j = 0; j < categoryTypes; ++j)
                    if (!skipedIndexes.Any(s => (s.i == T[i].R1 || s.i == T[i].R2) && s.j == j && (s.k == T[i].M1 || s.k == T[i].M2)))
                    {
                        A[m /*- T.Length * categoryTypes*/ - index, T[i].R2 * categoryTypes * mealTypes + j * mealTypes + T[i].M2 - minusIndexes[T[i].R2, j, T[i].M2]] = 1.0;
                        A[m - index, T[i].R1 * categoryTypes * mealTypes + j * mealTypes + T[i].M1 - minusIndexes[T[i].R1, j, T[i].M1]] = -1.0;
                        B[m - index] = 0;
                        index++;
                    }
            //price restrictions for group rooms
            for (var j = 0; j < categoryTypes; ++j)
                for (var k = 0; k < mealTypes; ++k)
                {
                    foreach (var child in childRooms)
                    {
                        A[m - index, child.Child * categoryTypes * mealTypes + j * mealTypes + k - minusIndexes[child.Child, j, k]] = -1;
                        A[m - index, child.Parent * categoryTypes * mealTypes + j * mealTypes + k - minusIndexes[child.Parent, j, k]]= 1;
                        B[m - index] = 0;
                        index++;
                    }
                    foreach (var parent in childRooms.Select(v => v.Parent).Distinct().ToList())
                    {
                        A[m - index, parent * categoryTypes * mealTypes + j * mealTypes + k - minusIndexes[parent, j, k]] = -1;
                        foreach (var child in childRooms.Where(v => v.Parent == parent).ToList())
                            A[m - index, child.Child * categoryTypes * mealTypes + j * mealTypes + k - minusIndexes[child.Child, j, k]] = 1;
                        B[m - index] = 0;
                        index++;
                    }
                }
            return result;

        }
        public ProblemInput TransformForW(int day, int roomTypes, int categoryTypes, int mealTypes, double[,,] a, double[,,] b, double[,] h, double[,,] L, double[,,] U, double[] R, bool withACondition = true)
        {
            var allTypes = roomTypes * categoryTypes * mealTypes;
            var allTypes2 = allTypes;

            int minusIndex = 0;
            int[,,] minusIndexes = new int[roomTypes, categoryTypes, mealTypes];
            var skipedIndexes = GetSkipedIndexes(roomTypes, categoryTypes,mealTypes, a, b, L, U, R);
            for (var i = 0; i < roomTypes; ++i)
                for (var j = 0; j < categoryTypes; ++j)
                    for (var k = 0; k < mealTypes; ++k)
                {
                    if ( skipedIndexes.Any(s => s.i == i && s.j == j && s.k == k))
                    {
                        minusIndex++;
                    }
                    else
                        minusIndexes[i, j, k] = minusIndex;
                }
            
            allTypes2 -= minusIndex;
            var n = allTypes2;
            var m = 3 * allTypes2;
            var result = new ProblemInput(n, m);
            result.D = day;
            result.SkipedIndexes = skipedIndexes;
            var q = result.Quadratic;
            var c = result.Linear;
            var A = result.ConstraintMatrix;  // A
            var B = result.ConstraintValues;  // B

            minusIndex = 0;
            for (var i = 0; i < roomTypes; ++i)
            {
                for (var j = 0; j < categoryTypes; ++j)
                {
                    for (var k = 0; k < mealTypes; ++k)
                    {
                        if (!skipedIndexes.Any(s => s.i == i && s.j == j && s.k == k))
                        {
                            var offset = i * categoryTypes * mealTypes + j * mealTypes + k - minusIndex;


                            q[offset, offset] = 2 * b[i, j, k];
                            c[offset] = a[i, j, k] - b[i, j, k] * h[i, k];

                            A[offset, offset] = 1;
                            var koef = h[i, k];

                            B[offset] = koef > 0 ? koef : 0;
                            A[(allTypes2 + offset), offset] = -1;
                            B[allTypes2 + offset] = -10000000;
                            if (withACondition)
                            {
                                if (a[i, j, k] <= 0)
                                {
                                    A[(2 * allTypes2 + offset), offset] = 1;
                                    B[2 * allTypes2 + offset] = 0;
                                }
                                else
                                {
                                    A[(2 * allTypes2 + offset), offset] = b[i, j, k];
                                    B[2 * allTypes2 + offset] = -a[i, j, k];
                                }
                            }
                        }
                        else
                            minusIndex++;
                    }
                }
            }

            return result;
        }

        public struct CheckObject
        {
            public MinusIndex index;
            public double value;
        }

        private IEnumerable<MinusIndex> GetSkipedIndexes(int roomTypes, int categoryTypes, int mealTypes, double[, ,] a, double[, ,] b, double[, ,] L, double[, ,] U, double[] R)
        {
                List<CheckObject> objects = new List<CheckObject>();
                List<MinusIndex> skipedIndexes = new List<MinusIndex>();

                for (var i = 0; i < roomTypes; ++i)
                    for (var j = 0; j < categoryTypes; ++j)
                        for (var k = 0; k < mealTypes; ++k)
                        {
                            var index = new MinusIndex { i = i, j = j, k = k };
                            objects.Add(new CheckObject { index = index, value = a[i, j, k] + b[i, j, k] * 3 * U[i, j, k] });
                            var sum = objects.Sum(s => s.value);
                            while (sum > R[i])
                            {
                                if (!objects.Any())
                                    break;
                                var max = objects.Max(s => s.value);
                                foreach (var obj in objects.Where(s => s.value == max).ToList())
                                {
                                    if (sum <= R[i])
                                        break;
                                    sum -= max;
                                    objects.Remove(obj);
                                    if (!skipedIndexes.Contains(obj.index))
                                        skipedIndexes.Add(obj.index);
                                }                                
                            }
                            if ((a[i, j, k] <= 0 || (-a[i, j, k] / b[i, j, k]) <= L[i, j, k] || R[i] < 1) && !skipedIndexes.Any(s => s.i == i && s.j == j && s.k == k))
                                skipedIndexes.Add(index);
                        }
                return skipedIndexes;
            
        }        //public ProblemInput TransformWithoutY(int roomTypes, int categoryTypes, double[,] a, double[,] b, double[] h,  
        //    double[,] L, double[,] U, int[] R, PriceRelation[] T, int day, bool withACondition = true)             
        //{

        //    var allTypes = roomTypes * categoryTypes;
        //    var allTypes2 = allTypes;

        //    int minusIndex = 0;
        //    int[,] minusIndexes = new int[roomTypes, categoryTypes];
        //    for (var i = 0; i < roomTypes; ++i)
        //    {
        //        for (var j = 0; j < categoryTypes; ++j)
        //        {
        //            if (a[i, j] <= 0 || (-a[i, j] / b[i, j]) <= L[i, j])
        //            {
        //                minusIndex++;
        //            }
        //            else
        //                minusIndexes[i, j] = minusIndex;
        //        }
        //    }
        //    allTypes2 -= minusIndex;
        //    //var relations = allTypes2 * categoryTypes * T.Length;
        //    var n = allTypes2;
        //    var m = 4 * allTypes2 + roomTypes;
        //    var result = new ProblemInput(n, m);
        //    var q = result.Quadratic;
        //    var c = result.Linear;
        //    var A = result.ConstraintMatrix;  // A
        //    var B = result.ConstraintValues;  // B
        //    double[] debug = new double[allTypes2];
        //    minusIndex = 0;
        //    for (var i = 0; i < roomTypes; ++i)
        //    {
        //        B[3 * allTypes2 + i] = -R[i];

        //        for (var j = 0; j < categoryTypes; ++j)
        //        {
        //            if (a[i, j] > 0 && (-a[i, j] / b[i, j]) > L[i, j])
        //            {

        //                var offset = i * categoryTypes + j - minusIndex;
        //                q[offset, offset] = 2 * b[i, j];
        //                c[offset] = a[i, j] - b[i, j] * h[i];
        //                A[offset, offset] = 1;
                        
        //                var koef = L[i, j] > h[i] ? L[i, j] : h[i];

        //                B[offset] = koef > 0 ? koef : 0;

        //                A[allTypes2 + offset, offset] = -1;
        //                B[allTypes2 + offset] = -U[i, j];
        //                if (withACondition)
        //                {
        //                    A[(2 * allTypes2 + offset), offset] = 1;
        //                    B[2 * allTypes2 + offset] = -a[i, j] / b[i, j];
        //                }
        //                B[3 * allTypes2 + i] += a[i, j];
        //                A[3 * allTypes2 + i, offset] = -b[i, j];                        
        //            }
        //            else
        //                minusIndex++;
        //        }
        //    }
        //    int index = 0;

        //    for (var i = 0; i < T.Length; ++i)
        //    {
        //        for (var j = 0; j < categoryTypes; ++j)
        //        {
        //            if (a[T[i].R2, j] > 0 && a[T[i].R1, j] > 0 && (-a[T[i].R2, j] / b[T[i].R2, j]) > L[T[i].R2, j] && (-a[T[i].R1, j] / b[T[i].R1, j]) > L[T[i].R1, j])
        //            {
        //                A[m - allTypes2 + index, T[i].R2 * categoryTypes + j - minusIndexes[T[i].R2, j]] = 1.0;
        //                A[m - allTypes2 + index, T[i].R1 * categoryTypes + j - minusIndexes[T[i].R1, j]] = -1.0;
        //                B[m - allTypes2 + index] = 0;
        //                index++;
        //            }

        //        }
        //    }

        //    return result;
        //}        
    }
}
