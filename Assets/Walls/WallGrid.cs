using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallGrid : Singleton<WallGrid> {

    protected WallGrid() { }

    public int gridWidth;
    public int gridHeight;
    public float gridSpacing;
    public Vector2 origin;

    public float snapRange;

    public string placedWallLayer;
    public static string PlacedWallLayer { get { return Instance.placedWallLayer; } }
    public string tumbleWallLayer;
    public static string TumbleWallLayer { get { return Instance.tumbleWallLayer; } }

    Vector2[,] verticalSnapPoints;
    bool[,] verticalWalls;
    Vector2[,] horizontalSnapPoints;
    bool[,] horizontalWalls;

    void Start()
    {
        verticalSnapPoints = new Vector2[gridWidth - 1, gridHeight];
        verticalWalls = new bool[gridWidth - 1, gridHeight];
        horizontalSnapPoints = new Vector2[gridWidth, gridHeight - 1];
        horizontalWalls = new bool[gridWidth, gridHeight - 1];


        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (i != gridWidth - 1)
                {
                    verticalSnapPoints[i, j] = new Vector2();
                    verticalSnapPoints[i, j].x = (i + 1) * gridSpacing;
                    verticalSnapPoints[i, j].y = (j * gridSpacing + (j + 1) * gridSpacing) / 2;
                    verticalSnapPoints[i, j] += origin;
                    verticalWalls[i, j] = false;
                }

                if (j != gridHeight - 1)
                {
                    horizontalSnapPoints[i, j] = new Vector2();
                    horizontalSnapPoints[i, j].x = (i * gridSpacing + (i + 1) * gridSpacing) / 2;
                    horizontalSnapPoints[i, j].y = (j + 1) * gridSpacing;
                    horizontalSnapPoints[i, j] += origin;
                    horizontalWalls[i, j] = false;
                }
            }
        }
    }

    public static bool AddWall(Wall w)
    {
        bool didSnap = false;
        float closestDistance = float.MaxValue;
        Vector2 point = Vector2.zero;

        int xIndex = 0;
        int yIndex = 0;

        if (w.isFloor) {
            for (int i = 0; i < Instance.gridWidth; i++)
            {
                for (int j = 0; j < Instance.gridHeight - 1; j++)
                {
                    float dist = Vector2.Distance(w.transform.position.xy(), Instance.horizontalSnapPoints[i, j]);
                    if (dist < closestDistance && dist < Instance.snapRange && !Instance.horizontalWalls[i, j])
                    {
                        closestDistance = dist;
                        point = Instance.horizontalSnapPoints[i, j];
                        didSnap = true;
                        xIndex = i;
                        yIndex = j;
                    }
                }
            }

            if (didSnap)
            {
                w.transform.position = point.WithZ(0);
                w.gameObject.layer = LayerMask.NameToLayer(Instance.placedWallLayer);
                w.velocity = Vector2.zero;
                w.isPlaced = true;
                Instance.horizontalWalls[xIndex, yIndex] = true;
            }
            else
            {
                didSnap = false;
                w.StartTumble();
            }
        }
        else 
        {
            for (int i = 0; i < Instance.gridWidth - 1; i++)
            {
                for (int j = 0; j < Instance.gridHeight; j++)
                {
                    float dist = Vector2.Distance(w.transform.position.xy(), Instance.verticalSnapPoints[i, j]);
                    if (dist < closestDistance && dist < Instance.snapRange && !Instance.verticalWalls[i, j])
                    {
                        closestDistance = dist;
                        point = Instance.verticalSnapPoints[i, j];
                        didSnap = true;
                        xIndex = i;
                        yIndex = j;
                    }
                }
            }

            if (didSnap)
            {
                w.transform.position = point.WithZ(0);
                w.gameObject.layer = LayerMask.NameToLayer(Instance.placedWallLayer);
                w.velocity = Vector2.zero;
                w.isPlaced = true;
                Instance.verticalWalls[xIndex, yIndex] = true;
            }
            else
            {
                didSnap = false;
                w.StartTumble();
            }
        }

        return didSnap;
    }
}
