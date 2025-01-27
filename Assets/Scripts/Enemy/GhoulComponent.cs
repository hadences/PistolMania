using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GhoulComponent : MonoBehaviour
{
    public UnityEvent onDeathEvent;

    [Header("Ghoul Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public GameObject target;
    [SerializeField] public float movSpeed = 8.0f;
    [SerializeField] private int maxHealth = 1;

    [Header("Dash Settings")]
    [SerializeField] private float dashCooldownValue = 4.0f;
    [SerializeField] private float moveSpeed = 2.0f;

    public bool canMove = true;
    private bool isDashing = false;
    private float dashCooldown = 0.0f;

    public int health;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onDeathEvent.AddListener(GameManager.Instance.onGhoulDeathEvent);
        dashCooldown = dashCooldownValue;
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (dashCooldown <= 0 && !isDashing) {
            dashCooldown = dashCooldownValue + 2;
            StartCoroutine(dash());
        }

        if (dashCooldown > 0) {
            dashCooldown -= Time.deltaTime;
            dashCooldown = Mathf.Max(dashCooldown, 0);
        }
    }

    private void FixedUpdate() {
        if (!canMove) {
            return;
        }

        Vector3 moveDirection = (target.transform.position - transform.position).normalized;

        if (moveDirection.x > 0) {
            spriteRenderer.flipX = true;
        }else {
            spriteRenderer.flipX = false;
        }

        rb.linearVelocity = moveDirection * movSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
            damage(1);
        }
    }

    public void addForce(Vector3 motion, float speed) {
        StartCoroutine(applyMotion(motion, speed));
    }

    IEnumerator applyMotion(Vector3 motion, float speed) {
        canMove = false;
        yield return new WaitForSeconds(0.1f);
        rb.AddForce(motion * speed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.6f);
        canMove = true;

    }

    IEnumerator dash() {
        canMove = false;
        isDashing = true;

        rb.linearVelocity = Vector2.zero;

        // Wait for 2 seconds before dashing
        yield return new WaitForSeconds(2);

        // Compute dash direction
        Vector3 dashDir = (target.transform.position - transform.position).normalized;

        // Apply force to dash
        rb.AddForce(dashDir * moveSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.6f);
        isDashing = false;
        canMove = true;
    }

    public void damage(int damage) {
        health -= damage;
        if (health <= 0) {
            onDeath();
        }
    }

    private void onDeath() {
        onDeathEvent.Invoke();
        ParticleManager.Instance.spawnParticle(ParticleManager.Instance.dustParticle, transform.position, Quaternion.identity);

        ParticleManager.Instance.spawnParticle(ParticleManager.Instance.impactParticle, transform.position, Quaternion.identity);
        SoundManager.Instance.playSound(SoundManager.ghostDeathSound, 0.25f, Random.Range(1.0f, 1.2f));
        Destroy(gameObject);
    }
}
