using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {
    int geneSize;
    float[][] population;
    public GeneticAlgorithm(int populationSize, int geneSize) {
        this.geneSize = geneSize;
        population = new float[populationSize][];
        for (int i = 0; i < populationSize; i++) {
            var gene = new float[geneSize];
            for (int j = 0; j < geneSize; j++) {
                gene[j] = Random.Range(-1f, 1f);
            }
            population[i] = gene;
        }
    }
}
