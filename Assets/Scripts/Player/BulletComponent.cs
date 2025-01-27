using System;
using UnityEngine;

public class BulletComponent : MonoBehaviour
{
    int bounces = 0;

    public bool bouncy = false;
    public bool tether = false;
    public bool inferno = false;
    public bool scatter = false;

    public float infernoRadius = 2f;

    // Radius within which enemies will be affected by the tether
    public float tetherRadius = 5f;

    // Force to pull the enemies to the bullet's position
    public float tetherPullForce = 10f;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && bouncy) {
            bounces++;
        }
        else {
            Destroy(gameObject);
        }

        if (bounces >= 3) {
            Destroy(gameObject);
        }

        if(tether) {
            applyTether();
        }

        if(inferno) {
            applyInferno();
        }

        if(scatter) {
            scatterBullets();
        }
    }

    private void scatterBullets() {
        float radius = 0.2f;
        int bulletCount = 5; // Number of bullets to scatter

        for (int i = 0; i < bulletCount; i++) {
            float angle = i * (2 * Mathf.PI / bulletCount);

            // Calculate the position on the circle
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            Vector3 bulletPosition = transform.position + new Vector3(x, y, 0.0f);

            GameObject bullet = Instantiate(
                GameManager.Instance.playerComponent.bulletPrefab,
                bulletPosition,
                Quaternion.identity,
                GameManager.Instance.entities.transform
            );

            Vector2 direction = new Vector2(x, y).normalized;

            // Apply force in the outward direction
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * 0.5f, ForceMode2D.Impulse);
        }
    }

    private void applyInferno() {
        Vector2 bulletPosition = transform.position;

        ParticleManager.Instance.spawnParticle(ParticleManager.Instance.infernoParticle, transform.position, Quaternion.identity);
        Collider2D[] hits = Physics2D.OverlapCircleAll(bulletPosition, infernoRadius);

        foreach (var hit in hits) {
            if (hit.CompareTag("Enemy")) {
                // Get the Rigidbody2D component of the enemy
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                GhoulComponent ghoulComponent = hit.GetComponent<GhoulComponent>();
                if (enemyRb != null) {
                    ghoulComponent.damage(1);
                }
            }
        }
    }

    private void applyTether() {
        // Get the current position of the bullet
        Vector2 bulletPosition = transform.position;

        // Find all objects within the tether radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(bulletPosition, tetherRadius);

        foreach (var hit in hits) {
            // Check if the object is an enemy (you can use tags or layers for this)
            if (hit.CompareTag("Enemy")) {
                // Get the Rigidbody2D component of the enemy
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                GhoulComponent ghoulComponent = hit.GetComponent<GhoulComponent>();
                if (enemyRb != null) {
                    ParticleManager.Instance.spawnParticle(ParticleManager.Instance.tetherParticle, transform.position, Quaternion.identity);

                    // Calculate the direction to pull the enemy
                    Vector2 pullDirection = (bulletPosition - (Vector2)enemyRb.transform.position).normalized;

                    ghoulComponent.addForce(pullDirection, tetherPullForce);
                }
            }
        }

        // Optionally destroy the bullet after applying the tether
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        // Visualize the tether radius in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, tetherRadius);
    }
}
