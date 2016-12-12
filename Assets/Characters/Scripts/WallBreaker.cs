using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallBreaker : MonoBehaviour {

    bool canBreak = false;
    bool isBreaking = false;
    List<Collider2D> collidedObjects;
    Dictionary<Vector2, Wall> breakWalls;
    Wall wallBeingBroke = null;
    Vector2 breakDirection = Vector2.zero;

	void Start () {
        GetComponent<CharacterController2D>().onControllerCollidedEvent += AddToCollisionList;
        collidedObjects = new List<Collider2D>();
        breakWalls = new Dictionary<Vector2, Wall>();
        breakWalls.Add(Vector2.down, null);
        breakWalls.Add(Vector2.left, null);
        breakWalls.Add(Vector2.right, null);
    }

    void Update() {
        GetComponent<Animator>().SetBool("IsBreaking", isBreaking);
    }

    void FixedUpdate() {
        breakWalls[Vector2.down] = null;
        breakWalls[Vector2.left] = null;
        breakWalls[Vector2.right] = null;

        canBreak = false;

        foreach (Collider2D coll in collidedObjects)
        {
            if (coll.GetComponent<Wall>() && coll.gameObject.layer == LayerMask.NameToLayer(WallGrid.PlacedWallLayer))
            {
                canBreak = true;
                List<Vector2> keys = new List<Vector2>(breakWalls.Keys);
                foreach (Vector2 v in keys) {
                    if (Vector2.Dot(v, coll.transform.position - GetComponent<Collider2D>().bounds.center) > 0.1) {
                        breakWalls[v] = coll.GetComponent<Wall>();   
                    }
                }
            }
        }

        if (!GetComponent<CharacterController2D>().isGrounded)
        {
            canBreak = false;
        }

        if (wallBeingBroke != null && (!canBreak || !breakWalls.ContainsValue(wallBeingBroke))) {
            StopBreakWall();
        }

        collidedObjects = new List<Collider2D>();
    }

    public void BreakWall(Vector2 direction) {
        if (isBreaking && direction != breakDirection) {
            StopBreakWall();
            return;
        }

        if (!breakWalls.ContainsKey(direction) || !canBreak || isBreaking)
            return;


        isBreaking = true;
        wallBeingBroke = breakWalls[direction];
        if (wallBeingBroke == null)
        {
            breakDirection = Vector2.zero;
            wallBeingBroke = null;
            isBreaking = false;
            return;
        }
        wallBeingBroke.GetComponent<Animator>().SetBool("IsBreaking", true);
        breakDirection = direction;
        GetComponent<Animator>().SetFloat("yIn", direction.y);
    }

    public void StopBreakWall()
    {
        wallBeingBroke.GetComponent<Animator>().SetBool("IsBreaking", false);
        breakDirection = Vector2.zero;
        wallBeingBroke = null;
        isBreaking = false;
    }
	
	void AddToCollisionList(RaycastHit2D collision) {
        collidedObjects.Add(collision.collider);
    }
}
