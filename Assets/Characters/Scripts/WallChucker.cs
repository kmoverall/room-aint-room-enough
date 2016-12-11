using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallChucker : MonoBehaviour {

    public Wall wallPrefab;
    bool isThrowingFloor;
    public float spawnDistance;
    public int ammo = 12;

	public void ThrowWall(Vector2 direction) {
        if (ammo == 0)
            return;
            
        Wall newWall = Instantiate(wallPrefab, transform.position + direction.WithZ(0) * spawnDistance, Quaternion.identity);
        newWall.isFloor = isThrowingFloor;
        newWall.Throw(direction);
        ammo--;
        GetComponent<Animator>().SetTrigger("Throw");
    }

    public void SwapDirection() {
        isThrowingFloor = !isThrowingFloor;
    }
}
