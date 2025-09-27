// using UnityEngine;
// using TMPro;
// using Unity.InferenceEngine;

// public class MNISTSentisInference : MonoBehaviour
// {
//     public ModelAsset modelAsset;
//     public DrawingPad drawingPad;
//     public TextMeshProUGUI predictionText;
//     public TextMeshProUGUI confidenceText;

//     private FunctionalGraph graph;
//     private FunctionalTensor[] inputTensors;

//     void Start()
//     {
//         // Load the model
//         var model = ModelLoader.Load(modelAsset);

//         // Create a functional graph
//         graph = new FunctionalGraph();

//         // Add inputs to the graph
//         inputTensors = graph.AddInputs(model);

//         // Forward pass and apply softmax
//         var outputs = Functional.Forward(model, inputTensors);
//         var softmax = Functional.Softmax(outputs[0]);

//         // Add output to the graph
//         graph.AddOutput(softmax);

//         // Compile the graph
//         graph.Compile();
//     }

//     public void Predict()
//     {
//         Texture2D tex = drawingPad.GetTexture();
//         Color[] pixels = tex.GetPixels();

//         // Flatten and normalize
//         float[] inputData = new float[28 * 28];
//         for (int i = 0; i < pixels.Length; i++)
//             inputData[i] = pixels[i].r;

//         // Copy input data into the FunctionalTensor
//         inputTensors[0].(inputData); // <- Use CopyFrom, not SetData or SetInput

//         // Run the graph
//         graph.Run();

//         // Get output tensor
//         var outputTensor = graph.GetOutput(0); // first output
//         float[] results = outputTensor.ToReadOnlyArray();

//         // Find predicted digit
//         int predictedDigit = ArgMax(results, out float confidence);

//         // Update UI
//         predictionText.text = $"Prediction: {predictedDigit}";
//         confidenceText.text = $"Confidence: {(confidence * 100f):F2}%";
//     }

//     int ArgMax(float[] arr, out float maxVal)
//     {
//         int maxIdx = 0;
//         maxVal = arr[0];
//         for (int i = 1; i < arr.Length; i++)
//         {
//             if (arr[i] > maxVal)
//             {
//                 maxVal = arr[i];
//                 maxIdx = i;
//             }
//         }
//         return maxIdx;
//     }
// }
