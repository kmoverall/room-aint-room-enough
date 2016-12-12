using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wall : MonoBehaviour {

    public float throwSpeed;
    public AudioClip hitSound;
    
    bool _isFloor;
    public bool isFloor
    {
        get { return _isFloor; }
        set
        {
            _isFloor = value;
            transform.rotation = value ? Quaternion.Euler(0, 0, -90) : Quaternion.identity;
        }
    }
    [HideInInspector]
    public bool isPlaced = false;
    [HideInInspector]
    public bool isLoose = false;
    [HideInInspector]
    public Vector2 velocity = Vector2.zero;

    void FixedUpdate() {
        if (!isLoose)
            GetComponent<Rigidbody2D>().velocity = velocity;

        GetComponent<Rigidbody2D>().isKinematic = !isLoose;
        GetComponent<Rigidbody2D>().constraints = isLoose ? RigidbodyConstraints2D.None : RigidbodyConstraints2D.FreezeRotation;
    }

    public void Throw(Vector2 direction) {
        velocity = direction * throwSpeed;
    }

    public void StartTumble()
    {
        gameObject.layer = LayerMask.NameToLayer(WallGrid.TumbleWallLayer);
        isLoose = true;
    }

    public void BreakWall()
    {
        WallGrid.BreakWall(this);
        StartTumble();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.collider.gameObject);
    }

    public void HandleCollision(GameObject other)
    {

        if (!isLoose && !isPlaced)
        {
            if (other.layer == LayerMask.NameToLayer(WallGrid.PlacedWallLayer))
                WallGrid.AddWall(this);
            else
                StartTumble();
        }
    }

    public void PlayAudio(AudioClip sound)
    {
        GetComponent<AudioSource>().PlayOneShot(sound);
    }
}
