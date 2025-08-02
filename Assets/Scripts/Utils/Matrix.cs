using System;
using System.Linq;
using UnityEngine;


namespace Scripts.Utils
{
    /// <summary>
    /// Matrix class for 2D matrix operations, suitable for Unity development.
    /// </summary>
    public class Matrix
    {
        private readonly int _rows;
        private readonly int _cols;
        private float[,] _data;

        public float[,] Data => _data;
        public int Rows => _rows;
        public int Cols => _cols;

        // Constructor
        public Matrix(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            // 2-d array with all 0s
            _data = new float[rows, cols];
        }

        public static Matrix From2dArray(float[,] inputArray)
        {
            Matrix m = new Matrix(inputArray.GetLength(0), inputArray.GetLength(1));
            for (int i = 0; i < inputArray.GetLength(0); i++)
            {
                for (int j = 0; j < inputArray.GetLength(1); j++)
                {
                    m._data[i, j] = inputArray[i, j];
                }
            }
            return m;
        }

        // Convert float[] to Matrix
        public static Matrix FromArray(float[] inputArray)
        {
            Matrix m = new Matrix(inputArray.Length, 1);
            for (int i = 0; i < inputArray.Length; i++)
            {
                m._data[i, 0] = inputArray[i];
            }
            return m;
        }

        // Convert Matrix back to float[]
        public static float[] ToArray(Matrix m)
        {
            float[] result = new float[m.Rows];
            for (int i = 0; i < m.Rows; i++)
            {
                result[i] = m._data[i, 0];
            }
            return result;
        }


        // Transpose a matrix (rows become columns and vice versa)
        public static Matrix Transpose(Matrix m)
        {
            Matrix result = new Matrix(m._cols, m._rows);
            for (int i = 0; i < result._rows; i++)
            {
                for (int j = 0; j < result._cols; j++)
                {
                    result._data[i, j] = m._data[j, i];
                }
            }
            return result;
        }

        // Get the shape (rows by columns) of a matrix
        public (int, int) Shape()
        {
            return (Rows, Cols);
        }

        public void Scale(float scalar)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                for (int j = 0; j < _data.GetLength(1); j++)
                {
                    _data[i, j] *= scalar;
                }
            }
        }

        public void SumBias(float[] biases)
        {
            if (Cols != biases.Length) return;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    _data[i, j] += biases[j];
                }
            }
        }

        public void Sum(float scalar)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                for (int j = 0; j < _data.GetLength(1); j++)
                {
                    _data[i, j] += scalar;
                }
            }
        }

        public void Sub(float scalar)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                for (int j = 0; j < _data.GetLength(1); j++)
                {
                    _data[i, j] -= scalar;
                }
            }
        }

        // Generate a matrix with random values normalized between -1 and 1
        public Matrix Randomize()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    _data[i, j] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
            return this;
        }

        public Matrix Clone()
        {
            Matrix copy = new Matrix(_rows, _cols);
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    copy._data[i, j] = _data[i, j];
                }
            }
            return copy;
        }

        public void Map(Func<float, float> func)
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    float val = _data[i, j];
                    _data[i, j] = func(val);
                }
            }
        }

        public void Fill(float[,] data)
        {
            if (data.Length != _rows || _data.GetLength(1) != _cols)
            {
                Debug.LogError("Shape mismatch in Fill().");
                return;
            }
            _data = data;
        }

        public void Print()
        {
            var value = "";
            for (int i = 0; i < _rows; i++)
            {
                var row = Enumerable.Range(0, _cols)
                                .Select(j => _data[i, j].ToString("0.##"));
                // Debug.Log(string.Join("\t", row));
                value += $"{string.Join("\t", row)}\n";
            }

            Debug.Log(value);
        }

        public void Reset()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    _data[i, j] = 0;
                }
            }
        }

        public static float DeterminantNaive(Matrix matrix)
        {
            if ((matrix._rows != matrix._cols) || (matrix._rows < 1 && matrix._cols < 1))
            {
                Debug.LogError("Cannot get determinant you can only evaluate the determinant of a squared matrix");
                return 0;
            }

            Matrix getSubMatrix(Matrix m, int colToSkip)
            {
                var mat = new Matrix(m.Rows - 1, m.Cols - 1);
                // always skip the first row of the superset matrix (m)
                for (int i = 1; i < m.Rows; i++)
                {
                    int colsToPopulate = 0;
                    for (int j = 0; j < m.Cols; j++)
                    {
                        if (j == colToSkip)
                        {
                            continue;
                        }
                        mat[i - 1, colsToPopulate++] = m[i, j];
                    }
                }

                return mat;
            }

            // m x n matrix where n must
            float runner(Matrix mat, int m, int n)
            {
                if (m == 1 && n == 1)
                {
                    return mat[0, 0];
                }
                if (m == 2 && n == 2)
                {
                    return (mat[0, 0] * mat[1, 1]) - (mat[0, 1] * mat[1, 0]);
                }

                float sum = 0;
                for (int j = 0; j < n; j++)
                {
                    var subMat = getSubMatrix(mat, j);
                    sum += mat[0, j] * runner(subMat, m - 1, n - 1) * ((j % 2 == 0) ? 1 : -1);
                }

                return sum;
            }

            return runner(matrix, matrix._rows, matrix._cols);
        }

        public float[] this[int rowIdx]
        {
            get => Data.GetRow(rowIdx);
        }

        // Indexer to access elements with m[row, col]
        public float this[int row, int col]
        {
            get => _data[row, col];
            set => _data[row, col] = value;
        }

        public void Apply(Func<float, float> activation)
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    _data[i, j] = activation(_data[i, j]);
        }

        // needed for backpropagation
        public static Matrix ElementWiseMultiply(Matrix m1, Matrix m2)
        {
            if (m1._rows != m2._rows || m1._cols != m2._cols)
            {
                Debug.LogError("Shape mismatch for element-wise multiplication.");
                return null;
            }

            Matrix result = new Matrix(m1._rows, m1._cols);
            for (int i = 0; i < m1._rows; i++)
                for (int j = 0; j < m1._cols; j++)
                    result._data[i, j] = m1._data[i, j] * m2._data[i, j];
            return result;
        }

        public Matrix Softmax(Matrix logits)
        {
            for (int i = 0; i < logits.Rows; i++)
            {
                float max = float.MinValue;
                for (int j = 0; j < logits.Cols; j++)
                    if (logits.Data[i, j] > max) max = logits.Data[i, j];

                float sum = 0f;
                float[] exp = new float[logits.Cols];
                for (int j = 0; j < logits.Cols; j++)
                {
                    exp[j] = Mathf.Exp(logits.Data[i, j] - max);
                    sum += exp[j];
                }

                for (int j = 0; j < logits.Cols; j++)
                    logits._data[i, j] = exp[j] / sum;
            }

            return logits;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Cols != b.Cols)
                throw new InvalidOperationException("Matrix dimensions must match for addition");

            Matrix result = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Cols; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }
            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Cols != b.Cols)
                throw new InvalidOperationException("Matrix dimensions must match for subtraction");

            Matrix result = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Cols; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix m, float scalar)
        {
            Matrix result = new Matrix(m.Rows, m.Cols);
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Cols; j++)
                {
                    result[i, j] = m[i, j] * scalar;
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
                throw new InvalidOperationException("Matrix dimensions are incompatible for multiplication");

            Matrix result = new Matrix(a.Rows, b.Cols);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Cols; j++)
                {
                    float sum = 0f;
                    for (int k = 0; k < a.Cols; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            return result;
        }



    }

}
