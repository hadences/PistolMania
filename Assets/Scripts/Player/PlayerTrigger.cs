using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] PlayerComponent playerComponent;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Enemy") {
            playerComponent.damage(1);
            Destroy(collision.gameObject);
        }
    }
}
