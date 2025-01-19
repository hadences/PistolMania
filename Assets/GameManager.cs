using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum ScreenType {
        MAIN,
        HUD,
        END
    }

    [Header("Canvas References/Settings")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private GameObject endScreen;
   
    private List<GameObject> screens = new List<GameObject>();

    void Start()
    {
        // add the screens into the screens list
        screens.Add(mainScreen);
        screens.Add(endScreen);
        screens.Add (hudScreen);
        
        showScreen(ScreenType.MAIN); // show main by default
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame() {
        showScreen(ScreenType.HUD);
    }

    public void endGame() {
        showScreen(ScreenType.END);
    }

    public void returnToMainScreen() {
        showScreen(ScreenType.MAIN);
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
}
