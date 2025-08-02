using System;
using Scripts.Utils;
using UnityEngine;

namespace Scripts
{
    /*
    Forward Pass with Multiple Hidden Layers

Let’s assume you have:

    1 input layer

    2 hidden layers

    1 output layer

Step-by-step:

    Input → Hidden Layer 1:
    h1=activation(W1⋅x+b1)
    h1​=activation(W1​⋅x+b1​)

    Hidden Layer 1 → Hidden Layer 2:
    h2=activation(W2⋅h1+b2)
    h2​=activation(W2​⋅h1​+b2​)

    Hidden Layer 2 → Output Layer:
    y=activation(W3⋅h2+b3)
    y=activation(W3​⋅h2​+b3​)

Each layer computes a linear combination of inputs and applies a non-linear activation function (e.g., ReLU, Sigmoid, Tanh).
🔁 Backpropagation with Multiple Hidden Layers

Backpropagation now needs to compute gradients layer by layer, from the output back to the input:

    Output Layer Error:
    δoutput=(ytarget−ypred)⋅dActivation(y)
    δoutput​=(ytarget​−ypred​)⋅dActivation(y)

    Hidden Layer 2 Error:
    δ2=(W3T⋅δoutput)⋅dActivation(h2)
    δ2​=(W3T​⋅δoutput​)⋅dActivation(h2​)

    Hidden Layer 1 Error:
    δ1=(W2T⋅δ2)⋅dActivation(h1)
    δ1​=(W2T​⋅δ2​)⋅dActivation(h1​)

    Update Weights:
    Each layer's weight matrix is updated using its delta and the activation from the previous layer.

📦 Code Structure (Conceptually)

You’ll typically store:

    A list/array of weight matrices: weights[i] for layer i

    A list of bias vectors

    A list of activations and their derivatives

You loop forward over layers for prediction and loop backward for training.
✅ Benefits of Multiple Hidden Layers

    Can model complex, non-linear relationships

    Deep networks (with many layers) can extract hierarchical features

    But more layers → higher computational cost and risk of overfitting

📌 Rule of Thumb

Start with 1–2 hidden layers. Add more only if:

    Your model underfits (can’t capture patterns)

    You have sufficient data

    You're solving complex tasks (e.g., image recognition, NLP)
    */
    public class NeuralNetwork
    {
        private int numInputs, numHidden, numOutputs;
        private Matrix weightsIH, weightsHO;
        private Matrix biasH, biasO;

        public NeuralNetwork(int numInputs, int numHidden, int numOutputs)
        {
            this.numInputs = numInputs;
            this.numHidden = numHidden;
            this.numOutputs = numOutputs;

            weightsIH = new Matrix(numHidden, numInputs).Randomize();
            weightsHO = new Matrix(numOutputs, numHidden).Randomize();

            biasH = new Matrix(numHidden, 1).Randomize();
            biasO = new Matrix(numOutputs, 1).Randomize();
        }

        // Forward pass
        public Matrix Predict(Matrix input)
        {
            // 1 Hidden layer
            Matrix hidden = weightsIH * input;
            hidden += biasH;
            UnityEngine.Debug.Log($"weightsIH.Shape(): {weightsIH.Shape()}");
            UnityEngine.Debug.Log($"input.Shape(): {input.Shape()}");
            UnityEngine.Debug.Log($"hidden.Shape(): {hidden.Shape()}");
            hidden.Map(ActivationFunctions.Tanh);  // or your activation function

            // Output layer
            // this hidden value is now the latest value that is ready to be fed forward to the output
            Matrix output = weightsHO * hidden;
            output += biasO;
            UnityEngine.Debug.Log($"weightsHO.Shape(): {weightsHO.Shape()}");
            UnityEngine.Debug.Log($"hidden.Shape(): {hidden.Shape()}");
            UnityEngine.Debug.Log($"output.Shape(): {output.Shape()}");
            output.Map(ActivationFunctions.Sigmoid);  // for classification tasks

            return output;
        }

        public void Train(float[] inputArray, float[] targetArray, float learningRate = 0.4f)
        {
            // Convert arrays to column vectors (Matrix)
            Matrix inputs = Matrix.FromArray(inputArray);  // [inputNodes x 1]
            Matrix targets = Matrix.FromArray(targetArray); // [outputNodes x 1]

            // Forward pass
            // inputs * weight + biases
            Matrix hiddenInputs = weightsIH * inputs;
            hiddenInputs += biasH;
            hiddenInputs.Map(ActivationFunctions.Tanh);

            Matrix finalInputs = weightsHO * hiddenInputs;
            finalInputs += biasO;
            finalInputs.Map(ActivationFunctions.Sigmoid);

            // Output error = target - actual
            Matrix outputErrors = targets - finalInputs;
            // targets: [1 x 1]
            // finalInputs: [1 x 1]

            // Output gradient
            Matrix gradients = finalInputs.Clone(); // [1 x 1]
            gradients.Map(ActivationFunctions.DSigmoid);
            gradients = Matrix.ElementWiseMultiply(gradients, outputErrors);
            gradients.Scale(learningRate);

            // Calculate deltas for weightsHO
            Matrix hiddenTransposed = Matrix.Transpose(hiddenInputs);
            Matrix weightHODeltas = gradients * hiddenTransposed;

            // Update weightsHO and biasO (tweaking weights and biases starts here in the backpropagation process)
            weightsHO += weightHODeltas;
            biasO += gradients;

            // Calculate hidden layer error
            Matrix weightsHOT = Matrix.Transpose(weightsHO);
            Matrix hiddenErrors = weightsHOT * outputErrors;

            // Hidden gradient
            Matrix hiddenGradient = hiddenInputs.Clone();
            hiddenGradient.Map(ActivationFunctions.DTanh);
            hiddenGradient = Matrix.ElementWiseMultiply(hiddenGradient, hiddenErrors);
            hiddenGradient.Scale(learningRate);

            // Calculate deltas for weightsIH
            Matrix inputsTransposed = Matrix.Transpose(inputs);
            Matrix weightIHDeltas = hiddenGradient * inputsTransposed;

            // Update weightsIH and biasH (more tweaking just like before)
            weightsIH += weightIHDeltas;
            biasH += hiddenGradient;
        }
    }

}
