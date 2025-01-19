using UnityEngine;

public class BulletComponent : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Wall")) return;
        Destroy(gameObject);
    }
}
