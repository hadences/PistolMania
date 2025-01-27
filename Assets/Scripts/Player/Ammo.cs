using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] public int ammoValue = 0;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            PlayerComponent playerComp = collision.gameObject.GetComponent<PlayerComponent>();
            playerComp.maxAmmo += ammoValue;
            SoundManager.Instance.playSound(SoundManager.happySound, 0.25f, Random.Range(1.25f, 1.5f));

            Destroy(gameObject);
        }
    }


}
