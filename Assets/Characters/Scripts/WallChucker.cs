using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallChucker : MonoBehaviour {

    public AudioClip throwSound;
    public AudioClip pickupSound;
    public Animator character;
    public Wall wallPrefab;
    public float spawnDistance;
    public int ammo = 30;

	public void ThrowWall(Vector2 direction, bool isFloor) {
        if (ammo == 0)
            return;
            
        Wall newWall = Instantiate(wallPrefab, transform.position + direction.WithZ(0) * spawnDistance, Quaternion.identity);
        newWall.isFloor = isFloor;
        newWall.Throw(direction);
        ammo--;

        GetComponent<AudioSource>().PlayOneShot(throwSound);

        character.SetFloat("ThrowY", direction.y);
        character.SetTrigger("Throw");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Wall>() != null) {
            ammo++;
            GetComponent<AudioSource>().PlayOneShot(pickupSound);
            Destroy(other.gameObject);
        }
    }
}
