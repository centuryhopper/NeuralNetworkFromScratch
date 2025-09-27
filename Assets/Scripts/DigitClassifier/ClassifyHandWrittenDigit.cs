using UnityEngine;
using Unity.InferenceEngine;
using TMPro;

public class ClassifyHandwrittenDigit : MonoBehaviour
{
    public Texture2D inputTexture;
    public ModelAsset modelAsset;
    Worker worker;
    public float[] results;
    private DrawingPad drawingPad;

    private TextMeshProUGUI predictionTextComponent;
    private TextMeshProUGUI confidenceTextComponent;


    void Awake()
    {
        drawingPad = GetComponentInChildren<DrawingPad>();
        predictionTextComponent = transform.Find("PredictionText").GetComponent<TextMeshProUGUI>();
        confidenceTextComponent = transform.Find("ConfidenceText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        // Load model
        Model sourceModel = ModelLoader.Load(modelAsset);

        // Debug.Log($"sourceModel.inputs[0].shape: {sourceModel.inputs[0].shape}");

        // Add softmax for probabilities
        // FunctionalGraph graph = new FunctionalGraph();
        // FunctionalTensor[] inputs = graph.AddInputs(sourceModel);
        // FunctionalTensor[] outputs = Functional.Forward(sourceModel, inputs);
        // FunctionalTensor softmax = Functional.Softmax(outputs[0]);
        // graph.AddOutput(softmax);

        // runtimeModel = graph.Compile();
        worker = new Worker(sourceModel, BackendType.GPUCompute);
    }

    public void Predict()
    {
        // Texture2D resizedTex = ResizeTexture(inputTexture, 28, 28);
        // Color32[] pixels = resizedTex.GetPixels32();
        // Color32[] pixels = inputTexture.GetPixels32();
        Color32[] pixels = drawingPad.GetTexture().GetPixels32();
        float[] inputData = new float[28 * 28];
        for (int i = 0; i < pixels.Length; i++)
        {
            // White = 1, Black = 0
            // invert pixel values before creating the tensor
            inputData[i] = 1f - (pixels[i].r / 255f);
            // inputData[i] = pixels[i].r / 255f;
        }

        using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 784), inputData);

        // Create input data as a tensor
        // using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 1, 28, 28));
        // it’s for image-based models (e.g. CNNs), not your MLP.
        // TextureConverter.ToTensor(inputTexture, inputTensor);

        // Run the model with the input data
        worker.Schedule(inputTensor);

        // Get the result
        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;

        // outputTensor is still pending
        // Either read back the results asynchronously or do a blocking download call
        results = outputTensor.DownloadToArray();

        int predictedDigit = ArgMax(results, out float confidence);
        predictionTextComponent.text = $"Prediction: {predictedDigit}";
        confidenceTextComponent.text = $"Confidence: {confidence:P2}";
    }

    private Texture2D ResizeTexture(Texture2D tex, int width, int height)
    {
        Texture2D resized = new Texture2D(width, height, TextureFormat.RGB24, false);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int srcX = x * tex.width / width;
                int srcY = y * tex.height / height;
                Color pixel = tex.GetPixel(srcX, srcY);
                resized.SetPixel(x, y, pixel);
            }
        }
        resized.Apply();
        return resized;
    }


    public void ClearText()
    {
        predictionTextComponent.text = $"Prediction:";
        confidenceTextComponent.text = $"Confidence:";
    }

    int ArgMax(float[] arr, out float maxVal)
    {
        int maxIdx = 0;
        maxVal = arr[0];
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] > maxVal)
            {
                maxVal = arr[i];
                maxIdx = i;
            }
        }
        return maxIdx;
    }

    void OnDisable()
    {
        // Tell the GPU we're finished with the memory the engine used
        worker.Dispose();
    }
}
