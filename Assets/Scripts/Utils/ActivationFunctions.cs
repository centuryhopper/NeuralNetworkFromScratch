using System;
using UnityEngine;

namespace Scripts.Utils
{
    /// <summary>
    /// Common activation functions and their derivatives.
    /// </summary>
    public static class ActivationFunctions
    {
        // Sigmoid function: squashes values between 0 and 1
        public static float Sigmoid(float x)
        {
            return 1f / (1f + Mathf.Exp(-x));
        }

        // Derivative of sigmoid, assuming the input is already sigmoid(x)
        public static float DSigmoid(float y)
        {
            return y * (1f - y);
        }

        // Tanh function: squashes values between -1 and 1
        public static float Tanh(float x)
        {
            float ePos = Mathf.Exp(x);
            float eNeg = Mathf.Exp(-x);
            return (ePos - eNeg) / (ePos + eNeg);
        }

        // Derivative of tanh: 1 - tanh(x)^2, assuming input is tanh(x)
        public static float DTanh(float y)
        {
            return 1f - y * y;
        }

        // ReLU (Rectified Linear Unit)
        public static float ReLU(float x)
        {
            return Mathf.Max(0f, x);
        }

        // Derivative of ReLU
        public static float DReLU(float x)
        {
            return x > 0f ? 1f : 0f;
        }

        // Softmax: applied to the entire output vector, not a single float.
        // Use only for final layer output when doing classification.
        public static Matrix Softmax(Matrix m)
        {
            Matrix result = new Matrix(m.Rows, m.Cols);

            for (int i = 0; i < m.Rows; i++)
            {
                float max = float.MinValue;
                for (int j = 0; j < m.Cols; j++)
                    max = Mathf.Max(max, m.Data[i, j]);

                float sum = 0f;
                for (int j = 0; j < m.Cols; j++)
                    sum += Mathf.Exp(m.Data[i, j] - max); // for numerical stability

                for (int j = 0; j < m.Cols; j++)
                    result.Data[i, j] = Mathf.Exp(m.Data[i, j] - max) / sum;
            }

            return result;
        }
    }

}

