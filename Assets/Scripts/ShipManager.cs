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
    void OnDrawGizmos() {
        if (ships.Count > 0) {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(ships[0].gameObject.transform.position, Vector2.one * .1f);
        }
    }
    void Start() {
        var startPosition = Vector2.zero;
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
                var score = currentShip.GetScore();
                genes[i].score = score;
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
                var geneValues = genes[i].values;
                ships[i].nn.SetWeights(geneValues);
            }
        });
    }

    void Update() {
        ga.Epoch();
    }
}
