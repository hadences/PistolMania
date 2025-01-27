using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerComponent : MonoBehaviour
{
    public UnityEvent onDamageEvent;
    public UnityEvent onDeathEvent;

    [Header("Sprite Rotation")]
    [SerializeField] public GameObject sprite;
    [SerializeField] private float rotationSpeed;

    [Header("Upgrade Settings")]
    public bool bouncingBullets = false;
    public bool tetherBullets = false;
    public bool infernoBullets = false;
    public bool scatterBullets = false;


    [Header("Gun Settings")]
    [SerializeField] private ParticleSystem gunImpactParticle;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float recoilForce = 2.0f;
    [SerializeField] private float bulletSpeed = 8.0f;
    [SerializeField] private int magAmmoCount = 8;
    [SerializeField] private int startingMaxAmmo = 56;
    [SerializeField] private float reloadTime = 2;
    [SerializeField] private float maxReloadScale = 6;
    [SerializeField] private GameObject reloadIndicator;

    [Header("Player Settings")]
    [SerializeField] public int maxHealth = 5;
    [SerializeField] private float iFrameSeconds = 2;

    private float currentReloadIndicatorScale = 0;
    private float reloadIndicatorScaleDecrementVal = 0;

    public int health;
    private bool isInvincible = false;

    private bool isReloading = false;
    public float currentReloadTime = 2;
    public float currentRecoilForce = 2.0f;
    public int currentAmmo;
    public int maxAmmo;

    private InputAction attackInput;
    private InputAction reloadInput;

    private Vector3 playerDirection; // the direction the player is facing

    void Start()
    {
        reloadIndicatorScaleDecrementVal = maxReloadScale / (reloadTime / Time.fixedDeltaTime);

        attackInput = InputSystem.actions.FindAction("Attack");
        attackInput.performed += ctx => onShoot();

        reloadInput = InputSystem.actions.FindAction("Reload");
        reloadInput.performed += ctx => {
            if (isReloading) return;
            StartCoroutine(reload());
        };

        initPlayer();
    }

    public void initPlayer() {
        isReloading = false;
        currentRecoilForce = recoilForce;
        currentReloadTime = reloadTime;
        currentAmmo = magAmmoCount;
        maxAmmo = startingMaxAmmo;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        rotatePlayerToMouse();

        reloadIndicator.transform.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    private void FixedUpdate() {
        if (currentReloadIndicatorScale > 0) {
            currentReloadIndicatorScale -= reloadIndicatorScaleDecrementVal;
        }
        currentReloadIndicatorScale = Mathf.Max(currentReloadIndicatorScale, 0);
        reloadIndicator.transform.localScale = new Vector3(currentReloadIndicatorScale*80f, 50.0f, 1.0f);
    }

    IEnumerator iFrameCountdown() {
        isInvincible = true;
        yield return new WaitForSeconds(iFrameSeconds);
        isInvincible = false;
    }

    IEnumerator reload() {
        // reloads the ammo
        currentReloadIndicatorScale = maxReloadScale;
        isReloading = true;

        SoundManager.Instance.playSound(SoundManager.reloadSound, 0.25f, Random.Range(1f, 1.25f));

        // delay time 
        yield return new WaitForSeconds(currentReloadTime);
        // set the current ammo to mag ammo acount - (check if max ammo has enough - otherwise set current as remaining ammo)
        int ammoNeeded = magAmmoCount - currentAmmo;

        SoundManager.Instance.playSound(SoundManager.reloadSound, 0.25f, Random.Range(1.25f, 1.5f));

        int ammoRemains = maxAmmo - ammoNeeded;
        if(ammoRemains > 0) {
            // we set the current ammo to the magAmmoCount
            currentAmmo = magAmmoCount;
            maxAmmo -= ammoNeeded;
        }
        else {
            currentAmmo = maxAmmo;
            maxAmmo = 0;
        }
        isReloading = false;
    }

    public void resetUpgrades() {
        bouncingBullets = false;
        infernoBullets = false;
        tetherBullets = false;
        scatterBullets = false;
        
    }

    void onShoot() {
        if (isReloading || !GameManager.Instance.inGame) return; // preven the player from reloading

        if (currentAmmo <= 0) {
            // TODO reload
            if (!isReloading && gameObject.activeSelf) {
                StartCoroutine(reload());
            }

            return;
        }

        // shake camera
        GameManager.Instance.shakeCamera(0.25f, 2.0f);

        // spawn particle
        gunImpactParticle.Play();

        // play sound
        SoundManager.Instance.playSound(SoundManager.shootSound, 1.0f, Random.Range(1.0f, 1.2f));

        currentAmmo--;

        // spawn the bullet on the shoot point
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity, GameManager.Instance.entities.transform);        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        BulletComponent bulletComponent = bullet.GetComponent<BulletComponent>();

        if (bouncingBullets) {
            bulletComponent.bouncy = true;
        }

        if (tetherBullets) {
            bulletComponent.tether = true;
        }

        if (infernoBullets) {
            bulletComponent.inferno = true;
        }

        if(scatterBullets) {
            bulletComponent.scatter = true;
        }

        rb.AddForce(playerDirection * bulletSpeed, ForceMode2D.Impulse);

        // move the player opposite dir
        Rigidbody2D playerRB = GetComponent<Rigidbody2D>();

        Vector2 recoil = -playerDirection.normalized * currentRecoilForce; // Normalize for consistent magnitude
        playerRB.AddForce(recoil, ForceMode2D.Impulse);
    }

    void rotatePlayerToMouse() {
        // Get mouse position in world coordinates
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Compute direction and raw angle from the player to the mouse
        Vector3 direction = mousePosition - transform.position;

        direction.z = 0;

        playerDirection = direction.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust angle and sprite flip based on direction

        SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();

        if (angle > 90 || angle < -90) {
            spriteRenderer.flipX = true;
            sprite.transform.localRotation = Quaternion.Euler(0f, 0f, -135.0f);
        }
        else {
            spriteRenderer.flipX = false;
            sprite.transform.localRotation = Quaternion.Euler(0f, 0f, -45.0f);
        }

        // Get current Z-angle of the transform
        float currentAngle = transform.eulerAngles.z;
        // Compute the new target angle using LerpAngle for smooth interpolation
        float targetAngle = Mathf.LerpAngle(currentAngle, angle, rotationSpeed * Time.deltaTime);

        // Apply the rotation to the player
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    public void damage(int damage) {
        health -= damage;
        ParticleManager.Instance.spawnParticle(ParticleManager.Instance.dustParticle, transform.position, Quaternion.identity);
        SoundManager.Instance.playSound(SoundManager.hurtSound, 0.5f, Random.Range(1.0f, 1.2f));

        onDamageEvent.Invoke();
        if (health <= 0) {
            onDeathEvent.Invoke();
        }

        if (isInvincible) return;
        if (isActiveAndEnabled) {
            StartCoroutine(iFrameCountdown());
        }
    }
}
