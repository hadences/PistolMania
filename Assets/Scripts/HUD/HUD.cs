using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject healthDisplayPoint;
    [SerializeField] private GameObject heartSprite;

    [Header("Settings")]
    [SerializeField] private float spacing = 16;

    private List<GameObject> healthDisplay = new List<GameObject>();

    private void Start() {
        healthDisplay.Clear();
    }

    public void updateHealth(GameObject player) {
        PlayerComponent comp = player.GetComponent<PlayerComponent>();  
        if (comp == null) return;

        int maxHealth = comp.maxHealth;
        int health = comp.health;

        if (health < healthDisplay.Count) {
            // remove extras
            for (int start = healthDisplay.Count - 1; start >= health; start--) {
                GameObject obj = healthDisplay[start];
                healthDisplay.RemoveAt(start); 

                Heart heart = obj.GetComponent<Heart>();
                heart.destroyHeart();
            }

            return;
        }

        if (healthDisplay.Count != 0) return;

        for (int i = 0; i < maxHealth; i++) {
            Vector3 position = new Vector3(i * spacing, 0, 0); // Use local space offset
            GameObject heart = Instantiate(heartSprite, healthDisplayPoint.transform);

            heart.transform.localPosition = position; // Set local position
            healthDisplay.Add(heart);
        }
    }
}
