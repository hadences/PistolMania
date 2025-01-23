using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public enum ScreenType {
        MAIN,
        HUD,
        END
    }

    [Header("Canvas References/Settings")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private GameObject endScreen;

    [Header("Game System References")]
    [SerializeField] public GameObject entities;
    [SerializeField] public GameObject player;
    [SerializeField] private GameObject spawnerPrefab;
    private GameSpawner spawner;

    [Header("Prefab References")]
    [SerializeField] private GameObject ghoulPrefab;
    
    [Header("Ammo References")]
    [SerializeField] private TextMeshProUGUI currentAmmoTextMesh;
    [SerializeField] private TextMeshProUGUI maxAmmoTextMesh;

    private PlayerComponent playerComponent;


    private List<GameObject> screens = new List<GameObject>();

    void Start()
    {
        Time.timeScale = 0;

        // create instance
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        playerComponent = player.GetComponent<PlayerComponent>();
        if(playerComponent == null) {
            Debug.Log("the player object does not have the player component!");
            Application.Quit();
        }

        // add the screens into the screens list
        screens.Add(mainScreen);
        screens.Add(endScreen);
        screens.Add (hudScreen);
        
        showScreen(ScreenType.MAIN); // show main by default

        HUD hud = hudScreen.GetComponent<HUD>();
        hud.updateHealth(player);
    }

    // Update is called once per frame
    void Update()
    {
        updateHUD();
    }

    public void startGame() {
        Time.timeScale = 1;

        var spawnerObj = Instantiate(spawnerPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
        spawner = spawnerObj.GetComponent<GameSpawner>();

        showScreen(ScreenType.HUD);
        playerComponent.initPlayer();
        player.gameObject.SetActive(true);
        player.transform.position = Vector3.zero;

        HUD hud = hudScreen.GetComponent<HUD>();
        hud.updateHealth(player);
    }

    public void endGame() {
        player.gameObject.SetActive(false);
        spawner.killAll();
        Destroy(spawner);
        Time.timeScale = 0;
        showScreen(ScreenType.END);
    }

    public void returnToMainScreen() {
        showScreen(ScreenType.MAIN);
    }

    private void updateHUD() {
        // update the ammo
        currentAmmoTextMesh.text = playerComponent.currentAmmo + "";
        maxAmmoTextMesh.text = playerComponent.maxAmmo + "";
    }

    /*
     Shows the screen of a given type
     */
    public void showScreen(ScreenType type) {
        foreach (GameObject screen in screens) {
            screen.SetActive(false);
        }

        switch (type) {
            case ScreenType.MAIN: {
                    mainScreen.SetActive(true);
                    break;
                }
            case ScreenType.HUD: {
                    hudScreen.SetActive(true);
                    break;
                }
            case ScreenType.END: {
                    endScreen.SetActive(true);
                    break;
                }
        }
    }

    public void onPlayerDeathEvent() {
        endGame();
    }

    public void onPlayerHurtEvent() {
        HUD hud = hudScreen.GetComponent<HUD>();
        hud.updateHealth(player);
    }
}
