using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron {
    float[] weights;
    public static int abc = 0;
    public Neuron(int inputCount) {
        weights = new float[inputCount + 1]; // BIAS WEIGHT
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = Random.Range(-1f, 1f);
        }
    }
    public float Activate(float[] inputs) {
        float sum = 0;
        for (int i = 0; i < inputs.Length; i++) {
            sum += weights[i] * inputs[i];
        }
        sum += weights[weights.Length - 1] * 1; // BIAS
        return Sigmoid(sum);
    }
    float Sigmoid(float x) {
        return 1 / (1 + Mathf.Exp(-x));
    }
    public float[] GetWeights() {
        return weights;
    }
    public void SetWeights(float[] newWeights) {
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = newWeights[i];
        }
    }
}
