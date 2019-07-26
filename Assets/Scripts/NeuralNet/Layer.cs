using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer {
    Neuron[] neurons;
    public int inputCount;
    public int neuronCount;
    public Layer(int inputCount, int neuronCount) {
        this.inputCount = inputCount;
        this.neuronCount = neuronCount;
        neurons = new Neuron[neuronCount];
        for (int i = 0; i < neurons.Length; i++) {
            neurons[i] = new Neuron(inputCount);
        }
    }
    public float[] GetWeights() {
        float[] layerWeights = new float[neurons.Length * (inputCount + 1)]; // BIAS
        for(int i = 0; i < neurons.Length; i++) {
            var neuronWeights = neurons[i].GetWeights();
            neuronWeights.CopyTo(layerWeights, neuronWeights.Length * i);
        }
        return layerWeights;
    }
    public void SetWeights(float[] layerWeights) {
        float[] neuronWeights = new float[inputCount + 1]; // BIAS
        int currentIndex = 0;
        for (int neuronIndex = 0; neuronIndex < neurons.Length; neuronIndex++) {
            for (int i = 0; i < neuronWeights.Length; i++) {
                neuronWeights[i] = layerWeights[currentIndex];
                currentIndex++;
            }
            neurons[neuronIndex].SetWeights(neuronWeights);
        }
    }
    public float[] Activate(float[] inputs) {
        float[] results = new float[neurons.Length];
        for (int i = 0; i < neurons.Length; i++) {
            var result = neurons[i].Activate(inputs);
            results[i] = result;
        }
        return results;
    }
}
