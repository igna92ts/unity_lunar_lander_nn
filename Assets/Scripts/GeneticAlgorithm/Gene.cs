using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gene {
    public float[] values;
    float score = 0;
    public int id;
    public float Score { set { score = value; } get { return score; } }
    public Gene(int geneSize, int id) {
        this.id = id;
        values = new float[geneSize];
    }
}
