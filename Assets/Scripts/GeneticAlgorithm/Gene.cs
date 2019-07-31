using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gene {
    public float[] values;
    public float score = 0;
    public Gene(int geneSize) {
        values = new float[geneSize];
    }
}
