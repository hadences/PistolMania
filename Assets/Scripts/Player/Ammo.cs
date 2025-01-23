using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private int ammoValue = 0;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            PlayerComponent playerComp = collision.gameObject.GetComponent<PlayerComponent>();
            playerComp.maxAmmo += ammoValue;

            Destroy(gameObject);
        }
    }


}
