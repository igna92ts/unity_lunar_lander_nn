using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet {

    Layer[] layers;
    public NeuralNet(int inputCount, params int[] layersCount) {
        int previousLayerCount = inputCount;
        layers = new Layer[layersCount.Length];
        for (int i = 0; i < layersCount.Length; i++) {
            var layer = new Layer(previousLayerCount, layersCount[i]);
            layers[i] = layer;
            previousLayerCount = layersCount[i];
        }
    }

    public float[] Activate(params float[] initialInputs) {
        if (initialInputs.Length != layers[0].inputCount) {
            Debug.LogError("Neural Net: Input missing");
        }
        float[] newInputs = initialInputs;
        foreach(Layer layer in layers) {
            newInputs = layer.Activate(newInputs);
        }
        return newInputs;
    }

    public float[] GetWeights() {
        float[] netWeights = new float[GetWeightCount()];
        int lastIndex = 0;
        for (int i = 0; i < layers.Length; i++) {
            var layerWeights = layers[i].GetWeights();
            layerWeights.CopyTo(netWeights, lastIndex);
            lastIndex += layerWeights.Length;
        }
        return netWeights;
    }
    public void SetWeights(float[] netWeights) {
        int currentIndex = 0;
        foreach(Layer layer in layers) {
            var layerWeightCount = (layer.inputCount + 1) * layer.neuronCount;
            float[] layerWeights = new float[layerWeightCount];
            for (int i = 0; i < layerWeightCount; i++) {
                layerWeights[i] = netWeights[currentIndex];
                currentIndex++;
            }
            layer.SetWeights(layerWeights);
        }
    }
    public int GetWeightCount() {
        int count = 0;
        foreach(Layer layer in layers) {
            count += layer.neuronCount * (layer.inputCount + 1); // BIAS
        }
        return count;
    }
}
