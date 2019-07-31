using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneticAlgorithm {
    int geneSize;
    float timer = 0;
    Gene[] population;
    const int MUTATION_CHANCE = 1;
    const int CROSSOVER_CHANCE = 90;
    GaDelegate simulationFunction;
    GaDelegate updateFunction;
    EndConditionDelegate simulationEndCondition;
    float simulationTime = 0;
    public bool shouldLoad = true;
    public GeneticAlgorithm(int populationSize, int geneSize) {
        if (populationSize % 2 != 0) Debug.LogError("Try to make it a pair value pls");
        this.geneSize = geneSize;
        if (shouldLoad) {
            population = DataManager.LoadGeneData();
        } else {
            population = new Gene[populationSize];
            for (int i = 0; i < populationSize; i++) {
                var gene = new Gene(geneSize);
                for (int j = 0; j < geneSize; j++) {
                    gene.values[j] = Random.Range(-1f, 1f);
                }
                population[i] = gene;
            }
        }
    }
    public delegate void GaDelegate(Gene[] genes);

    public void SetSimulationFunction(float simulationTime, GaDelegate simulationFunction) {
        this.simulationTime = simulationTime;
        if (simulationFunction != null) {
            this.simulationFunction = simulationFunction;
        } else {
            Debug.LogError("Simulation function has already been set");
        }
    }

    void Procreate() {
        int i = 2;
        var newPopulation = new Gene[population.Length];
        var sortedPopulation = population.OrderBy(gene => gene.score).Reverse().ToArray();
        newPopulation[0] = new Gene(geneSize);
        newPopulation[1] = new Gene(geneSize);
        sortedPopulation[0].values.CopyTo(newPopulation[0].values, 0);
        sortedPopulation[1].values.CopyTo(newPopulation[1].values, 0);
        while (i < newPopulation.Length) {
            var shouldCrossover = Random.Range(0, CROSSOVER_CHANCE);
            if (shouldCrossover <= CROSSOVER_CHANCE) {
                var rndSplitPoint = Random.Range(0, geneSize);
                var mother = PickRandomGene();
                var father = PickRandomGene();
                float[] motherFirst = mother.values.Take(rndSplitPoint).ToArray();
                float[] motherSecond = mother.values.Skip(rndSplitPoint).ToArray();
                float[] fatherFirst = father.values.Take(rndSplitPoint).ToArray();
                float[] fatherSecond = father.values.Skip(rndSplitPoint).ToArray();

                newPopulation[i] = new Gene(geneSize);
                motherFirst.CopyTo(newPopulation[i].values, 0);
                fatherSecond.CopyTo(newPopulation[i].values, motherFirst.Length);
                MutateGene(newPopulation[i]);

                newPopulation[i + 1] = new Gene(geneSize);
                fatherFirst.CopyTo(newPopulation[i + 1].values, 0);
                motherSecond.CopyTo(newPopulation[i + 1].values, fatherFirst.Length);
                MutateGene(newPopulation[i + 1]);
            } else {
                newPopulation[i] = new Gene(geneSize);
                newPopulation[i + 1] = new Gene(geneSize);
                var mother = PickRandomGene();
                var father = PickRandomGene();
                mother.values.CopyTo(newPopulation[i].values, 0);
                father.values.CopyTo(newPopulation[i + 1].values, 0);
                MutateGene(newPopulation[i]);
                MutateGene(newPopulation[i + 1]);
            }

            i += 2;
        }
        this.population = newPopulation;
    }

    void MutateGene(Gene gene) {
        for (int i = 0; i < geneSize; i++) {
            var rnd = Random.Range(0, 100);
            if (rnd <= MUTATION_CHANCE) {
                Debug.Log("Mutated");
                gene.values[i] = Random.Range(-1f, 1f);
            }
        }
    }

    Gene PickRandomGene() {
        var sortedPopulation = population.OrderBy(gene => gene.score).Reverse().ToArray();
        var totalScore = sortedPopulation.Sum(gene => gene.score);
        var rnd = Random.Range(0, totalScore);
        float sum = 0;
        foreach(Gene gene in sortedPopulation) {
            if (rnd < sum + gene.score) {
                return gene;
            } else {
                sum += gene.score;
            }
        }
        Debug.LogError("You did something wrong here");
        return null;
    }

    public delegate bool EndConditionDelegate();
    public void SetSimulationEndCondition(EndConditionDelegate simulationEndCondition) {
        if (simulationEndCondition == null) {
            simulationEndCondition = delegate() { return false; };
        } else {
            this.simulationEndCondition = simulationEndCondition;
        }
    }

    public void SetUpdateFunction(GaDelegate updateFunction) {
        this.updateFunction = updateFunction;
    }
    public void Epoch() {
        if (simulationFunction == null) Debug.LogError("No simulation Function");
        if (updateFunction == null) Debug.LogError("No update Function");
        timer += Time.deltaTime;
        if (timer >= simulationTime || simulationEndCondition()) {
            timer = 0;
            this.simulationFunction(this.population);
            DataManager.SaveGeneData(population);
            this.Procreate();
            this.updateFunction(this.population);
        }
    }
}
