using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour {
    public GameObject shipPrefab;
    [Range(1, 300)]
    public int simulationCount = 100;
    List<Ship> ships = new List<Ship>();
    float[] successfulWeights;
    public GeneticAlgorithm ga;
    void Awake() {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }
    void Start() {
        var startPosition = new Vector2(Random.Range(0, 3), Random.Range(0, 3));
        for (int i = 0; i < simulationCount; i++) {
            var newGameObj = Instantiate(shipPrefab, startPosition, Quaternion.identity);
            var newShip = newGameObj.GetComponent<Ship>();
            newShip.startPosition = startPosition;
            ships.Add(newShip);
        }

        ga = new GeneticAlgorithm(simulationCount, ships[0].nn.GetWeightCount());
        var simulationTime = 30f;
        ga.SetSimulationFunction(simulationTime, delegate (Gene[] genes) {
            for (int i = 0; i < ships.Count; i++) {
                var currentShip = ships[i];
                // if (ships[i].landed) successfulWeights = ships[i].nn.GetWeights();
                var score = currentShip.GetScore();
                genes[i].Score = score;
                currentShip.Restart();
            }
        });
        ga.SetSimulationEndCondition(delegate () {
            bool allDead = true;
            foreach(Ship ship in ships) {
                if (ship.isAlive) allDead = false;
            }
            return allDead;
        });
        ga.SetUpdateFunction(delegate (Gene[] genes) {
            for (int i = 0; i < genes.Length; i++) {
                if (successfulWeights != null) {
                    Debug.Log("Should be landing");
                    ships[i].nn.SetWeights(successfulWeights);
                } else {
                    var geneValues = genes[i].values;
                    ships[i].nn.SetWeights(geneValues);
                }
            }
        });
    }

    void Update() {
        ga.Epoch();
    }
}
