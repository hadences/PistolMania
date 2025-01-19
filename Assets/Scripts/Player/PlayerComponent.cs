using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponent : MonoBehaviour
{
    [Header("Sprite Rotation")]
    [SerializeField] public GameObject sprite;
    [SerializeField] private float rotationSpeed;

    [Header("Gun Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float recoilForce = 2.0f;
    [SerializeField] private float bulletSpeed = 8.0f;
    [SerializeField] private int magAmmoCount = 8;
    [SerializeField] private int startingMaxAmmo = 56;
    [SerializeField] private float reloadTime = 2;

    private bool isReloading = false;
    public int currentAmmo;
    public int maxAmmo;

    private InputAction attackInput;
    private InputAction reloadInput;

    private Vector3 playerDirection; // the direction the player is facing

    void Start()
    {
        currentAmmo = magAmmoCount;
        maxAmmo = startingMaxAmmo;

        attackInput = InputSystem.actions.FindAction("Attack");
        attackInput.performed += ctx => onShoot();

        reloadInput = InputSystem.actions.FindAction("Reload");
        reloadInput.performed += ctx => {
            if (isReloading) return;
            StartCoroutine(reload());
        };
    }

    // Update is called once per frame
    void Update()
    {
        rotatePlayerToMouse();
    }

    IEnumerator reload() {
        // reloads the ammo
        Debug.Log("reloading");
        isReloading = true;
        // delay time 
        yield return new WaitForSeconds(reloadTime);
        // set the current ammo to mag ammo acount - (check if max ammo has enough - otherwise set current as remaining ammo)
        int ammoNeeded = magAmmoCount - currentAmmo;
        
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

    void onShoot() {
        if (currentAmmo <= 0) {
            // TODO reload
            if (!isReloading) {
                StartCoroutine(reload());
            }

            return;
        }

        currentAmmo--;

        // spawn the bullet on the shoot point
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity, GameManager.Instance.entities.transform);        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(playerDirection * bulletSpeed, ForceMode2D.Impulse);

        // move the player opposite dir
        Rigidbody2D playerRB = GetComponent<Rigidbody2D>();
        playerRB.AddForce(-playerDirection * recoilForce, ForceMode2D.Impulse);
    }

    void rotatePlayerToMouse() {
        // Get mouse position in world coordinates
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Compute direction and raw angle from the player to the mouse
        Vector3 direction = mousePosition - transform.position;

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
}
