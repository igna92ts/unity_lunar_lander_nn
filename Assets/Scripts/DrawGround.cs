using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGround : MonoBehaviour {
    public GameObject linePrefab;
    GameObject currentLine;
    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;
    List<Vector2> linePoints = new List<Vector2>();
    public float lineWidth = 0.02f;
    [Range(0.01f, 1)]
    public float stepSize = .5f;
    void Awake() {
        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector2 bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, Camera.main.nearClipPlane));
        bottomLeft.y += lineWidth / 2;
        bottomRight.y += lineWidth / 2;

        CreateLine(bottomLeft);
        BuildLine(bottomLeft, bottomRight);
    }

    void CreateLine(Vector2 startPoint) {
        currentLine = Instantiate(linePrefab, Vector2.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        linePoints.Clear();
        linePoints.Add(startPoint);
        linePoints.Add(startPoint);
        lineRenderer.startWidth = lineWidth;
        lineRenderer.SetPosition(0, linePoints[0]);
        lineRenderer.SetPosition(1, linePoints[1]);
        edgeCollider.points = linePoints.ToArray();
    }

    void BuildLine(Vector2 startPos, Vector2 endPos) {
        var x = startPos.x;
        var modifier = Random.Range(0f, 1f);
        while (x < endPos.x) {
            float noise = MixFrequencies(x, linePoints[0].y + modifier);
            Vector2 newPos = new Vector2(x, linePoints[0].y + noise);
            linePoints.Add(newPos);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
            edgeCollider.points = linePoints.ToArray();
            x += stepSize;
        }
    }

    float MixFrequencies(float posX, float posY) {
        float freq1 = 1, freq2 = 2, freq3 = 4, freq4 = 16;
        float elevation = 5f;
        var combinedFrequencies = 1 * Mathf.PerlinNoise(freq1 * posX, freq1 * posY)
            + 0.5 * Mathf.PerlinNoise(freq2 * posX, freq2 * posY)
            + 0.25 * Mathf.PerlinNoise(freq3 * posX, freq3 * posY);
            // + 0.125 * Mathf.PerlinNoise(freq2 * posX, freq2 * posY);
        return Mathf.Pow((float)combinedFrequencies, elevation);
    }
}
