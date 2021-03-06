﻿using UnityEngine;
using System.Collections;

public class ProcMesh : MonoBehaviour {
    Mesh mesh;
    MeshRenderer meshRenderer;
    public Vector2 samples = new Vector2(20, 20);

    Texture2D texture;

	// Use this for initialization
	void Start () {
        mesh = gameObject.AddComponent<MeshFilter>().mesh;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh.Clear();

        texture = new Texture2D((int)samples.x , (int) samples.y);
        texture.filterMode = FilterMode.Point;
        GenerateTexture();
        GenerateMesh();

        meshRenderer.material.SetTexture(0, texture);
    }

    

    void GenerateTexture()
    {
        for (int y = 0; y < samples.y; y++)
        {
            for (int x = 0; x < samples.x; x++)
            {
                texture.SetPixel(x, y, Color.blue);                     
            }
        }
        texture.Apply();
    }

    float Sample(float x, float y)
    {
        float amplitude = 10;

        float theta = Mathf.PI;
        return Mathf.Sin(theta * (x / samples.x)) * Mathf.Sin(theta * (y / samples.y)) * amplitude;
        //return Mathf.PerlinNoise(x * scale, y * scale) * amplitude;
    }

    void GenerateMesh()
    {
        // We will start the actual xyz's of the mesh from this position
        Vector3 bottomLeft = new Vector3(- samples.x / 2, 0, - samples.y / 2);

        // 3 vertices per triangle and 2 triangles
        int verticesPerCell = 6;
        int vertexCount = (int) (verticesPerCell * samples.x * samples.y);

        // Allocate the arrays
        Vector3[] initialVertices = new Vector3[vertexCount];
        int[] initialTriangles = new int[vertexCount];
        Vector2[] initialUVs = new Vector2[vertexCount];
        int vertexIndex = 0;


        for (int y = 0; y < samples.y; y++)
        {
            for (int x = 0; x < samples.x; x++)
            {
                // Make the vertex positions
                Vector3 cellBottomLeft = bottomLeft + new Vector3(x, 0, y);
                Vector3 cellTopLeft = bottomLeft + new Vector3(x, 0, y + 1);
                Vector3 cellTopRight = bottomLeft + new Vector3(x + 1, 0, y + 1);
                Vector3 cellBotomRight = bottomLeft + new Vector3(x + 1, 0, y);

                // Sample for the y co-ord
                cellBottomLeft.y += Sample(x, y);
                cellTopLeft.y += Sample(x, y + 1);
                cellTopRight.y += Sample(x + 1, y + 1);
                cellBotomRight.y += Sample(x + 1, y);
                
                // Make the vertices for the triangles
                int startVertex = vertexIndex;
                initialVertices[vertexIndex++] = cellBottomLeft;
                initialVertices[vertexIndex++] = cellTopLeft;
                initialVertices[vertexIndex++] = cellTopRight;
                initialVertices[vertexIndex++] = cellBottomLeft;
                initialVertices[vertexIndex++] = cellTopRight;
                initialVertices[vertexIndex++] = cellBotomRight;

                // Make the triangles and UV's
                for (int i = 0; i < 6; i++)
                {
                    initialTriangles[startVertex + i] = startVertex + i;
                    initialUVs[startVertex + i] = new Vector2((float) x / samples.x, (float) y / samples.y);
                }
            }
            
            // Assign them back to the mesh
            mesh.vertices = initialVertices;
            mesh.triangles = initialTriangles;
            mesh.uv = initialUVs;

            mesh.RecalculateNormals();            
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
