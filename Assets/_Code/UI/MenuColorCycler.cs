using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuColorCycler : MonoBehaviour
{
    public int gridSize = 10;
    public float lineSpeed = 1.0f;

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;
    }

    private void Update()
    {
        for (int i = 0; i < gridSize; i++)
        {
            Vector3 startPos = new Vector3(-gridSize / 2 + i, 0, 0);
            Vector3 endPos = new Vector3(-gridSize / 2 + i, gridSize, 0);

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            startPos.x = -gridSize / 2;
            startPos.y = -gridSize / 2 + i;
            endPos.x = gridSize / 2;
            endPos.y = -gridSize / 2 + i;

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }

        lineRenderer.material.mainTextureOffset = new Vector2(0, Time.time * lineSpeed);
    }
}
