using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    public UnityEvent onLevelUpEvent;
    
    public int level = 0;
    public int currentEXP = 0;
    public int baseEXP = 5;
    public float multiplier = 1.5f;

    public Slider expBar;
    public TextMeshProUGUI levelText;

    public int getEXPForNextLevel() {
        return Mathf.CeilToInt(baseEXP * Mathf.Pow(multiplier, level-1));
    }

    private void levelUp() {
        level++;
        //Debug.Log($"Leveled up! Current Level: {level}");
        //Debug.Log($"EXP Needed for Next Level: {getEXPForNextLevel()}");
        onLevelUpEvent.Invoke();
    }

    public void resetLevel() {
        level = 0;
        currentEXP = 0;
        updateEXPBar();
    }

    public void addEXP(int amount) {
        currentEXP += amount;

        while (currentEXP >= getEXPForNextLevel()) {
            currentEXP -= getEXPForNextLevel();
            levelUp();
        }

        updateEXPBar();
    }

    private void updateEXPBar() {
        expBar.maxValue = getEXPForNextLevel();
        expBar.value = currentEXP;
        
        levelText.text = "Level: " + level;
    }
}
