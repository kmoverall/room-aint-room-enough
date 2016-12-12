using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreventPassThrough : MonoBehaviour {

    List<Vector3> posRecord = new List<Vector3>();
    public int maxCheck = 4;

	void Start ()
    {
        AddRecord(transform.position);
    }
	
	void Update () {

        RaycastHit2D hit = Physics2D.Raycast(posRecord[0], transform.position - posRecord[0], (transform.position - posRecord[0]).magnitude, Physics2D.GetLayerCollisionMask(gameObject.layer));

        if (hit)
        {
            Debug.DrawRay(posRecord[0], transform.position - posRecord[0], Color.red, 0.1f);
            if (hit.collider.gameObject != this.gameObject)
            {
                transform.position = hit.centroid;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                GetComponent<Wall>().HandleCollision(hit.collider.gameObject);
                this.enabled = GetComponent<Wall>().isLoose;
            }
        }

        AddRecord(transform.position);
    }

    void AddRecord(Vector3 pos) {
        posRecord.Add(pos);
        if (posRecord.Count > maxCheck) {
            posRecord.RemoveAt(0);
        }
    }
}
