using System.Linq;
using Scripts.Utils;
using UnityEngine;

namespace Scripts
{
    public class XORTrainer : MonoBehaviour
    {
        NeuralNetwork nn;

        float[,] trainingInputs = {
            { 0, 0 },
            { 0, 1 },
            { 1, 0 },
            { 1, 1 }
        };
        float[,] trainingOutputs = {
            { 0 },
            { 1 },
            { 1 },
            { 0 }
        };

        void Start()
        {
            /*
            Input Layer (2 nodes)
                ↓
            Hidden Layer (2 nodes, e.g. tanh activation)
                ↓
            Output Layer (1 node, sigmoid activation)

            */
            // 2 inputs (i.e. 0/1 and 1/0), 4 hidden nodes, 1 output (0 or 1)
            nn = new NeuralNetwork(2, 4, 1);

            for (int epoch = 0; epoch < 10_000; epoch++) // train for 5000 iterations
            {
                for (int i = 0; i < trainingInputs.GetLength(0); i++)
                {
                    // UnityEngine.Debug.Log($"{i}");
                    var trainingInputsRow = trainingInputs.GetRow(i);
                    var trainingOutputsRow = trainingOutputs.GetRow(i);
                    nn.Train(
                        trainingInputsRow
                        ,
                        trainingOutputsRow
                    );
                }
            }

            TestXOR();
        }

        void TestXOR()
        {
            for (int i = 0; i < trainingInputs.GetLength(0); i++)
            {
                Matrix inputMatrix = Matrix.FromArray(trainingInputs.GetRow(i));
                Matrix output = nn.Predict(inputMatrix);
                float[] result = Matrix.ToArray(output);

                Debug.Log($"Input: {inputMatrix[0][0]}, {inputMatrix[1][0]} => Output: {result[0]}");
            }
        }
    }

}
