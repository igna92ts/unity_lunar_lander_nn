using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour {
    public GameObject shipPrefab;
    [Range(1, 300)]
    public int simulationCount = 100;
    List<Ship> ships = new List<Ship>();
    public GeneticAlgorithm ga;
    void Start() {
        var startPosition = new Vector2(Random.Range(0, 3), Random.Range(0, 3));
        for (int i = 0; i < simulationCount; i++) {
            var newGameObj = Instantiate(shipPrefab, startPosition, Quaternion.identity);
            var newShip = newGameObj.GetComponent<Ship>();
            newShip.startPosition = startPosition;
            ships.Add(newShip);
        }

        ga = new GeneticAlgorithm(simulationCount, ships[0].nn.GetWeightCount());
    }

    void Update() {

    }
}
