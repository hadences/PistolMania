using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public float showChance = 0.1f;
    public bool resuable = false;
    public GameManager.UpgradeType upgradeType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(onClickButton);
    }

    void onClickButton() {
        GameManager.Instance.upgrade(upgradeType);
    }
}
