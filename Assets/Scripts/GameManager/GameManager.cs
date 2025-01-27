using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public enum ScreenType {
        MAIN,
        HUD,
        END
    }

    public enum UpgradeType {
        BOUNCING_BULLETS,
        TETHER,
        RELOADER,
        SCAVENGER,
        FIRE_POWER,
        INFERNO,
        TERMINATOR,
        SCATTER
    }

    [Header("Upgrade System")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private List<UpgradeType> currentUpgrades;
    [SerializeField] private List<Transform> cardSpawnPoints = new List<Transform>();
    [SerializeField] private Card bouncingBulletsCard;
    [SerializeField] private Card tetherCard;
    [SerializeField] private Card reloaderCard;
    [SerializeField] private Card scavengerCard;
    [SerializeField] private Card firePowerCard;
    [SerializeField] private Card infernoCard;
    [SerializeField] private Card terminatorCard;
    [SerializeField] private Card scatterCard;

    [Header("BG Music References")]
    [SerializeField] private AudioSource mainBGMusic;
    [SerializeField] private AudioSource inGameBGMusic;


    [Header("Canvas References/Settings")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject htpScreen;
    [SerializeField] private TextMeshProUGUI hudScoreText;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject highScoreText;

    [Header("Game System References")]
    [SerializeField] public GameObject entities;
    [SerializeField] public GameObject player;
    [SerializeField] private GameObject spawnerPrefab;
    [SerializeField] public Camera cam;
    [SerializeField] public LevelSystem levelSystem;
    private GameObject spawnerObj;
    private GameSpawner spawner;

    [Header("Prefab References")]
    [SerializeField] private GameObject ghoulPrefab;
    
    [Header("Ammo References")]
    [SerializeField] private TextMeshProUGUI currentAmmoTextMesh;
    [SerializeField] private TextMeshProUGUI maxAmmoTextMesh;

    public bool inGame = false;

    private float currentScore = 0;
    private float highScore = 0;

    public PlayerComponent playerComponent;

    private List<GameObject> screens = new List<GameObject>();

    void Start()
    {
        upgradePanel.SetActive(false);
        loadHighScore();

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

    void OnApplicationQuit() {
        saveHighScore();
    }

    private void saveHighScore() {
        PlayerPrefs.SetFloat("highScore", highScore);
    }

    private void loadHighScore() {
        highScore = PlayerPrefs.GetFloat("highScore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        updateHUD();

        currentScore += Time.deltaTime;
        hudScoreText.text = currentScore.ToString("F2");
    }

    public void startGame() {
        mainBGMusic.Stop();
        inGameBGMusic.Play();
        inGame = true;
        Time.timeScale = 1;

        levelSystem.resetLevel();

        currentScore = 0;

        if(spawnerObj != null) {
            Destroy(spawnerObj);
        }
        spawnerObj = Instantiate(spawnerPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
        spawner = spawnerObj.GetComponent<GameSpawner>();

        showScreen(ScreenType.HUD);
        playerComponent.resetUpgrades();
        currentUpgrades.Clear();
        playerComponent.initPlayer();

        player.gameObject.SetActive(true);
        player.transform.position = Vector3.zero;

        HUD hud = hudScreen.GetComponent<HUD>();
        hud.updateHealth(player);
    }

    public void endGame() {
        inGameBGMusic.Stop();
        inGame = false;
        TextMeshProUGUI scoreTextMesh = scoreText.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI highScoreTextMesh = highScoreText.GetComponent<TextMeshProUGUI>();

        if (currentScore > highScore) {
            highScore = currentScore;
        }

        scoreTextMesh.text = "Score: " + currentScore.ToString("F2");
        highScoreTextMesh.text = "High Score: " + highScore.ToString("F2");

        player.gameObject.SetActive(false);
        spawner.reset();
        Destroy(spawner);
        showScreen(ScreenType.END);
        Time.timeScale = 0;
    }

    public void returnToMainScreen() {
        mainBGMusic.Play();

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

        htpScreen.SetActive(false);

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

    public void showHTPScreen() {
        htpScreen.SetActive(true);
    }

    public void hideHTPScreen() {
        htpScreen.SetActive(false);
    }

    public void shakeCamera(float duration, float intensityMultiplier) {
        CameraShake camComp = cam.GetComponent<CameraShake>();
        camComp.shakeCamera(duration, intensityMultiplier);
    }

    public void onPlayerDeathEvent() {
        endGame();
    }

    public void onPlayerHurtEvent() {
        HUD hud = hudScreen.GetComponent<HUD>();
        hud.updateHealth(player);
    }

    public void onGhoulDeathEvent() {
        levelSystem.addEXP(1);
    }

    public void onPlayerLevelUp() {
        playerComponent.maxAmmo += 12;
        SoundManager.Instance.playSound(SoundManager.levelUpSound, 0.25f, Random.Range(1.0f, 1.0f));

        if (levelSystem.level % 1 == 0) {
            showUpgrades();
        }
    }

    /**
     * Main logic for the upgrade of the player based on upgrade type
     */
    public void upgrade(UpgradeType upgradeType) {
        SoundManager.Instance.playSound(SoundManager.happySound, 0.25f, Random.Range(1.0f, 1.0f));

        switch (upgradeType) {
            case UpgradeType.BOUNCING_BULLETS: {
                    playerComponent.bouncingBullets = true;
                    break;
                }
            case UpgradeType.TETHER: {
                    playerComponent.tetherBullets = true;
                    break;
                }
            case UpgradeType.RELOADER: {
                    // increases reload speed by 5 %
                    playerComponent.currentReloadTime -= playerComponent.currentReloadTime * 0.05f;
                    break;
                }
            case UpgradeType.FIRE_POWER: {
                    // increases fire power by 2%
                    playerComponent.currentRecoilForce += playerComponent.currentRecoilForce * 0.02f;
                    break;
                }
            case UpgradeType.SCAVENGER: {
                    spawner.ammoSupply += 4;
                    break;
                }
            case UpgradeType.INFERNO: {
                    playerComponent.infernoBullets = true;
                    break;
                }
            case UpgradeType.TERMINATOR: {
                    SoundManager.Instance.playSound(SoundManager.terminateSound, 0.5f, Random.Range(1.25f, 1.5f));
                    ParticleManager.Instance.spawnParticle(ParticleManager.Instance.terminatorParticle, transform.position, Quaternion.identity);
                    spawner.killAllMobs();
                    shakeCamera(0.5f, 2);
                    break;
                }
            case UpgradeType.SCATTER: {
                    playerComponent.scatterBullets = true;
                    break;
                }
        }

        Time.timeScale = 1.0f;
        upgradePanel.SetActive(false);
        currentUpgrades.Add(upgradeType);
    }

    /**
     * pauses the game and shows the upgrades
     */
    public void showUpgrades() {
        Time.timeScale = 0.0f;
        upgradePanel.SetActive(true);

        // Clear spawn points
        foreach (Transform spawnPoint in cardSpawnPoints) {
            foreach (Transform child in spawnPoint) {
                Destroy(child.gameObject);
            }
        }

        // Create list of all cards
        List<Card> allCards = new List<Card>
        {
        bouncingBulletsCard, tetherCard, reloaderCard, scavengerCard,
        firePowerCard, infernoCard, terminatorCard, scatterCard
    };

        // Filter available cards
        List<Card> availableCards = allCards.FindAll(card =>
            card.resuable || !currentUpgrades.Contains(card.upgradeType)
        );

        Debug.Log($"Available Cards Before Selection: {availableCards.Count}");

        // Select up to 3 cards
        List<Card> selectedCards = new List<Card>();
        int maxIterations = 100; // Failsafe limit
        int iterations = 0;

        while (selectedCards.Count < 3 && availableCards.Count > 0) {
            Card fallbackCard = availableCards[Random.Range(0, availableCards.Count)];
            if (!selectedCards.Contains(fallbackCard)) {
                selectedCards.Add(fallbackCard);
                availableCards.Remove(fallbackCard); // Prevent selecting the same card
            }

            iterations++;
            if (iterations >= maxIterations) {
                Debug.LogWarning("Max iterations reached in showUpgrades while loop. Breaking out to prevent infinite loop.");
                break;
            }
        }

        Debug.Log($"Selected Cards Count: {selectedCards.Count}");

        // Spawn cards
        for (int i = 0; i < selectedCards.Count; i++) {
            if (i < cardSpawnPoints.Count) {
                Instantiate(selectedCards[i].gameObject, cardSpawnPoints[i].position, Quaternion.identity, cardSpawnPoints[i]);
            }
        }
    }

    public void playOnClickSound() {
        SoundManager.Instance.playSound(SoundManager.clickSound);
    }
}
