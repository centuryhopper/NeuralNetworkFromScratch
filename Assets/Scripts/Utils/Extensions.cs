

using System;
using System.Linq;
using UnityEngine;

namespace Scripts.Utils
{
    public static class Extensions
    {
        public static float[] GetRow(this float[,] array2d, int rowIdx)
        {
            if (array2d.Length == 0)
            {
                return Array.Empty<float>();
            }
            if (rowIdx < 0 || rowIdx > array2d.GetLength(0))
            {
                return Array.Empty<float>();
            }

            // Debug.Log($"array2d.GetLength(1): {array2d.GetLength(1)}");

            return Enumerable.Range(0, array2d.GetLength(1)).Select(j => array2d[rowIdx, j]).ToArray();
        }
    }
}