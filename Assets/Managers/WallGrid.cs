using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallGrid : Singleton<WallGrid> {

    class GridSquare
    {
        public Dictionary<Vector2, bool> blocked;
        public GridSquare() {
            blocked = new Dictionary<Vector2, bool>();
            blocked.Add(Vector2.up, false);
            blocked.Add(Vector2.down, false);
            blocked.Add(Vector2.left, false);
            blocked.Add(Vector2.right, false);
        }

        public List<Vector2> Neighbors()
        {
            List<Vector2> n = new List<Vector2>();
            foreach (Vector2 v in blocked.Keys) {
                if (!blocked[v]) {
                    n.Add(v);
                }
            }
            return n;
        }
    }

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

    GridSquare[,] rooms;

    void Start()
    {
        verticalSnapPoints = new Vector2[gridWidth - 1, gridHeight];
        verticalWalls = new bool[gridWidth - 1, gridHeight];
        horizontalSnapPoints = new Vector2[gridWidth, gridHeight - 1];
        horizontalWalls = new bool[gridWidth, gridHeight - 1];

        rooms = new GridSquare[gridWidth, gridHeight];


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

                rooms[i, j] = new GridSquare();

                if (i == 0) {
                    rooms[i, j].blocked[Vector2.left] = true;
                }
                else if (i == gridWidth-1)
                {
                    rooms[i, j].blocked[Vector2.right] = true;
                }

                if (j == 0)
                {
                    rooms[i, j].blocked[Vector2.down] = true;
                }
                else if (j == gridHeight-1)
                {
                    rooms[i, j].blocked[Vector2.up] = true;
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
                Instance.rooms[xIndex, yIndex].blocked[Vector2.up] = true;
                Instance.rooms[xIndex, yIndex+1].blocked[Vector2.down] = true;
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
                w.PlayAudio(w.hitSound);
                w.transform.position = point.WithZ(0);
                w.gameObject.layer = LayerMask.NameToLayer(Instance.placedWallLayer);
                w.velocity = Vector2.zero;
                w.isPlaced = true;
                Instance.verticalWalls[xIndex, yIndex] = true;
                Instance.rooms[xIndex+1, yIndex].blocked[Vector2.left] = true;
                Instance.rooms[xIndex, yIndex].blocked[Vector2.right] = true;
            }
            else
            {
                didSnap = false;
                w.StartTumble();
            }
        }

        return didSnap;
    }

    public static void BreakWall(Wall w)
    {
        for (int i = 0; i < Instance.gridWidth; i++)
        {
            for (int j = 0; j < Instance.gridHeight; j++)
            {
                if (i != Instance.gridWidth - 1 && w.transform.position.xy() == Instance.verticalSnapPoints[i,j]) {
                    Instance.verticalWalls[i, j] = false;
                    Instance.rooms[i, j].blocked[Vector2.right] = false;
                    Instance.rooms[i+1, j].blocked[Vector2.left] = false;
                    return;
                }

                if (j != Instance.gridHeight - 1 && w.transform.position.xy() == Instance.horizontalSnapPoints[i, j])
                {
                    Instance.horizontalWalls[i, j] = false;
                    Instance.rooms[i, j].blocked[Vector2.up] = false;
                    Instance.rooms[i, j+1].blocked[Vector2.down] = false;
                    return;
                }
            }
        }
    }

    void Update() {
        Vector2 p1Room;
        Vector2 p2Room;

        Vector2 p1Pos = InputManager.Player1.transform.position.xy();
        Vector2 p2Pos = InputManager.Player2.transform.position.xy();

        p1Room = (p1Pos - origin) / gridSpacing;
        p1Room.x = Mathf.Floor(p1Room.x);
        p1Room.y = Mathf.Floor(p1Room.y);

        p2Room = (p2Pos - origin) / gridSpacing;
        p2Room.x = Mathf.Floor(p2Room.x);
        p2Room.y = Mathf.Floor(p2Room.y);

        if (ArePlayersConnected(p1Room, p2Room))
        {
            return;
        }

        int p1RoomSize = GetPlayerRoomSize(p1Room);
        int p2RoomSize = GetPlayerRoomSize(p2Room);

        if (p1RoomSize > p2RoomSize)
            Debug.Log("Red Wins!");
        else if (p1RoomSize < p2RoomSize)
            Debug.Log("Blue Wins!");
        else
            Debug.Log("Draw!");
    }

    bool ArePlayersConnected(Vector2 p1Pos, Vector2 p2Pos) {

        if (p1Pos == p2Pos)
            return true;

        List<Vector2> visited = new List<Vector2>();
        visited.Add(p1Pos);
        Queue<Vector2> Q = new Queue<Vector2>();
        Q.Enqueue(p1Pos);
        while (Q.Count > 0) 
        {
            Vector2 current = Q.Dequeue();
            foreach (Vector2 n in rooms[(int)current.x, (int)current.y].Neighbors())
            {
                if (current + n == p2Pos)
                {
                    return true;
                }
                if (!visited.Contains(current + n))
                {
                    visited.Add(current + n);
                    Q.Enqueue(current + n);
                }
            }
        }

        return false;
    }

    int GetPlayerRoomSize(Vector2 playerPos) 
    {
        List<Vector2> visited = new List<Vector2>();
        visited.Add(playerPos);
        Queue<Vector2> Q = new Queue<Vector2>();
        Q.Enqueue(playerPos);
        while (Q.Count > 0)
        {
            Vector2 current = Q.Dequeue();
            foreach (Vector2 n in rooms[(int)current.x, (int)current.y].Neighbors())
            {
                if (!visited.Contains(current + n))
                {
                    visited.Add(current + n);
                    Q.Enqueue(current + n);
                }
            }
        }

        return visited.Count;
    }
}
