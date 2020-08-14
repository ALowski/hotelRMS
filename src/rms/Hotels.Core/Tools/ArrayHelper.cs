using System;
using System.Linq;

namespace Hotels.Core.Tools
{
    public static class ArrayHelper
    {
        public static T[] CreateArray<T>(int length)
        {
            if (length < 0)
            {
                throw new ArgumentException("Length should be a positive number", length.ToString());
            }

            return new T[length];
        }

        public static T[][] CreateArray<T>(int length1, int length2)
        {
            return (T[][])CreateArray<T>(new[] { length1, length2 });
        }

        public static T[][][] CreateArray<T>(int length1, int length2, int length3)
        {
            return (T[][][])CreateArray<T>(new[] { length1, length2, length3 });
        }

        public static T[][][][] CreateArray<T>(int length1, int length2, int length3, int length4)
        {
            return (T[][][][])CreateArray<T>(new[] { length1, length2, length3, length4 });
        }
        public static T[][][][][] CreateArray<T>(int length1, int length2, int length3, int length4, int length5)
        {
            return (T[][][][][])CreateArray<T>(new[] { length1, length2, length3, length4, length5 });
        }
        public static T[][][][][][] CreateArray<T>(int length1, int length2, int length3, int length4, int length5, int length6)
        {
            return (T[][][][][][])CreateArray<T>(new[] { length1, length2, length3, length4, length5, length6});
        }
        private static Array CreateArray<T>(params int[] lengths)
        {
            if (lengths == null)
            {
                throw new ArgumentNullException(lengths.ToString());
            }

            if (lengths.Length <= 0)
            {
                throw new ArgumentException("Specify at least one dimension length.", lengths.ToString());
            }

            if (lengths.Length == 1)
            {
                return CreateArray<T>(lengths[0]);
            }

            var dims = lengths.Skip(1).ToArray();
            var temp = CreateArray<T>(dims);
            var res = Array.CreateInstance(temp.GetType(), lengths[0]);

            res.SetValue(temp, 0);
            for (var i = 1; i < lengths[0]; ++i)
            {
                res.SetValue(CreateArray<T>(dims), i);
            }

            return res;
        }
    }
}
