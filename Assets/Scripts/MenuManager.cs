using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    //Game Screens
    public GameObject go_titleScreen;
    public GameObject go_optionsScreen;
    public GameObject go_gameScreen;
    public GameObject go_pauseScreen;
    public GameObject go_deathScreen;
    public GameObject go_startScreen;
    public GameObject go_endScreen;
    public GameObject go_debugScreen;
    public GameObject go_controlsScreen;

    protected GameObject go_nextScreen;
    protected GameObject go_lastScreen;
    protected GameObject go_screenBuffer;

    //Singleton
    public static MenuManager instance;

    //Image Assets
    protected Image img_deathMessage;
    protected Image img_deathScreen;
    protected Image img_damageOverlay;
    protected Image img_fadeOverlay;

    public Image Img_damageOverlay { get { return img_damageOverlay; } set { img_damageOverlay = value; } }

    protected int int_enterSequence = 0;
    protected int int_startSequence = 0;
    protected int int_deathSequence = 0;
    protected int int_endSequence = 0;
    protected int int_quitToMenuSequence = 0;
    protected int int_clearScreenSequence = 0;

    protected Slider sli_volume;
    protected Slider sli_lookSensitivity;

    protected GameObject go_OrientationNote;
    protected GameObject go_quitButton;
    protected GameObject go_startButton;

    protected bool bl_paused = false;
    protected bool bl_allowPause = false;
    private void Awake()
    {
        img_damageOverlay = FindObjectOfType<DamageOverlay>(true).GetComponent<Image>();
        img_deathMessage = FindObjectOfType<DeathMessage>(true).GetComponent<Image>();
        go_quitButton = GameObject.Find("DeathScreenQuit");
        go_startButton = GameObject.Find("StartScreenButton");
        img_deathScreen = GameObject.Find("DeathOverlay").GetComponent<Image>();
        img_fadeOverlay = GameObject.Find("FadeOverlay").GetComponent<Image>();
        go_OrientationNote = GameObject.Find("Note");
        sli_volume = GameObject.Find("VolumeSlider").GetComponent<Slider>();
        sli_lookSensitivity = GameObject.Find("LookSensitivitySlider").GetComponent<Slider>();

        sli_volume.value = Settings.flt_volume;
        sli_lookSensitivity.value = Settings.flt_lookSensitivity;

        go_debugScreen.SetActive(false);

        //Singleton
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            FadeIn();
        }
        else
        {
            Destroy(gameObject);
        }

        SwitchScreen(go_titleScreen);

        go_screenBuffer = go_titleScreen;
    }
    
    public void SwitchScreenFancy(GameObject screen)
    {
        go_nextScreen = screen;

        if(screen == go_gameScreen) LeanTween.moveLocal(go_lastScreen, new Vector3(750f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeInSine).setOnComplete(SwitchToGame).setIgnoreTimeScale(true);
        else LeanTween.moveLocal(go_lastScreen, new Vector3(750f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeInSine).setOnComplete(SwitchScreenTransition).setIgnoreTimeScale(true);
        if (go_nextScreen == go_optionsScreen || go_nextScreen == go_controlsScreen) go_screenBuffer = go_lastScreen;
    }

    public void SwitchScreenTransition()
    {
        go_lastScreen.transform.localPosition = new Vector3(-750, 0f, 0f);

        if (go_lastScreen == go_optionsScreen || go_lastScreen == go_controlsScreen) LeanTween.moveLocal(go_screenBuffer, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);
        else LeanTween.moveLocal(go_nextScreen, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);

        go_lastScreen = go_nextScreen;
    }

    public void SwitchToGame()
    {
        go_gameScreen.SetActive(true);
    }

    //Switch the currently displayed screen to be the designated screen
    public void SwitchScreen(GameObject screen)
    {
        ClearScreens();

        if (screen != go_pauseScreen && screen != go_startScreen)
            Time.timeScale = 1;

        screen.SetActive(true);
        if(screen != go_optionsScreen && screen != go_controlsScreen ) go_lastScreen = screen;
    }

    //Set all screens to inactive
    private void ClearScreens()
    {
        go_titleScreen.SetActive(false);
        // go_optionsScreen.SetActive(false);
        go_gameScreen.SetActive(false);
        // go_pauseScreen.SetActive(false);
        go_deathScreen.SetActive(false);
        go_startScreen.SetActive(false);
        go_endScreen.SetActive(false);
        // go_controlsScreen.SetActive(false);
    }

    private void ClearScreenFancy()
    {
        switch(int_clearScreenSequence)
        {
            case 0:
                int_clearScreenSequence++;
                LeanTween.moveLocal(go_lastScreen, new Vector3(750f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeInSine).setOnComplete(ClearScreenFancy).setIgnoreTimeScale(true);
                break;

                case 1:
                GameManager.playerController.TogglePlayerControl();
                int_clearScreenSequence = 0;

                go_optionsScreen.transform.localPosition = new Vector3(-750, 0f, 0f);
                go_pauseScreen.transform.localPosition = new Vector3(-750, 0f, 0f);
                go_controlsScreen.transform.localPosition = new Vector3(-750, 0f, 0f);
                break;
        }
    }

    //Fade effect
    public void FadeIn()
    {
        LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0f, 0.2f);
    }

    //Fade effect
    public void FadeOut()
    {
        LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0f, 0.2f);
    }

    //Damage effect
    public void IncreaseDamageOverlay()
    {
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a + 0.33f, 0.2f).setIgnoreTimeScale(true);
    }

    //Temp damage effect
    public void IncreaseDamageOverlayTemporarily()
    {
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a + 0.33f, 0.2f).setOnComplete(ReduceDamageOverlay);
    }

    //Lower damage effect
    private void ReduceDamageOverlay()
    {
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a - 0.33f, 1f);
    }

    //Shows death sequence
    public void ShowDeathSequence()
    {
        switch(int_deathSequence)
        {
            case 0:
                Time.timeScale = 0;
                go_deathScreen.SetActive(true);
                int_deathSequence++;
                LeanTween.alpha(img_deathScreen.GetComponent<RectTransform>(), 1, 1f).setOnComplete(ShowDeathSequence).setIgnoreTimeScale(true);
                break;

            case 1:
                int_deathSequence++;
                LeanTween.alpha(img_deathMessage.GetComponent<RectTransform>(), 1, 1f).setOnComplete(ShowDeathSequence).setIgnoreTimeScale(true);
                break;
            case 2:
                int_deathSequence++;
                LeanTween.moveLocal(go_quitButton, new Vector3(0f, -110f, 0f), 1f).setEase(LeanTweenType.easeInSine).setOnComplete(ShowDeathSequence).setIgnoreTimeScale(true);
                break;
            case 3:
                Cursor.lockState = CursorLockMode.None;
                int_deathSequence = 0;
                break;
        }
    }

    // When the player dies or quits to title, this will reset any menu-related objects to their original positions, and reset the alpha of overlays.
    public void ResetMenus()
    {
        bl_allowPause = false;

        Color tempcolor = Img_damageOverlay.color;
        tempcolor.a = 0;
        Img_damageOverlay.color = tempcolor;

        go_quitButton.transform.localPosition = new Vector3(0f, -300, 0f);
        go_startButton.transform.localPosition = new Vector3(0f, -300, 0f);
        go_OrientationNote.transform.localPosition = new Vector3(0f, -500f, 0f);
    }

    //Enter the Start Scene
    // public void EnterScene(int index)
    // {
    //     SceneManager.LoadScene(index);
    // 
    //     SwitchScreen(go_startScreen);
    // 
    //     Cursor.lockState = CursorLockMode.Confined;
    //     Time.timeScale = 0;
    // }

    //Plays between Menu and Game scene?
    public void EnterGameSequence()
    {
        switch (int_enterSequence)
        {
            case 0:
                int_enterSequence++;
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 1, 1f).setOnComplete(EnterGameSequence).setIgnoreTimeScale(true);
                break;
            case 1:
                int_enterSequence++;
                ClearScreens();
                SceneManager.LoadScene(1);
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(EnterGameSequence).setIgnoreTimeScale(true);
                break;
            case 2:
                int_enterSequence++;                
                SwitchScreen(go_startScreen);
                LeanTween.moveLocal(go_OrientationNote, new Vector3(0f, 25f, 0f), 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete(EnterGameSequence).setIgnoreTimeScale(true);
                break;
            case 3:
                LeanTween.moveLocal(go_startButton, new Vector3(0f, -135f, 0f), 0.5f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
                int_enterSequence = 0;
                Cursor.lockState = CursorLockMode.Confined;
                break;
        }
    }

    //Start Game
    public void StartGameSequence()
    {
        switch (int_startSequence)
        {
            case 0:
                int_startSequence++;
                LeanTween.moveLocal(go_startButton, new Vector3(0f, -300f, 0f), 0.5f).setEase(LeanTweenType.easeInSine).setOnComplete(StartGameSequence).setIgnoreTimeScale(true);
                break;
            case 1:
                int_startSequence++;
                LeanTween.moveLocal(go_OrientationNote, new Vector3(0f, -500f, 0f), 0.5f).setEase(LeanTweenType.easeInSine).setOnComplete(StartGameSequence).setIgnoreTimeScale(true);
                break;
            case 2:
                SwitchScreen(go_gameScreen);
                Cursor.lockState = CursorLockMode.Locked;
                GameManager.playerController.TogglePlayerControl();
                bl_allowPause = true;
                int_startSequence = 0;
                break;
        }
    }

    //Go To Title Screen with fade transitions
    public void QuitToTitleSequence()
    {
        switch (int_quitToMenuSequence)
        {
            case 0:
                int_quitToMenuSequence++;
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 1, 1f).setOnComplete(QuitToTitleSequence).setIgnoreTimeScale(true);
                break;

            case 1:
                int_quitToMenuSequence++;
                GameManager.ResetGame();
                go_pauseScreen.transform.localPosition = new Vector3(-1000, 0, 0);
                SwitchScreen(go_titleScreen);
                SceneManager.LoadScene("TitleScene");
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(QuitToTitleSequence).setIgnoreTimeScale(true);
                break;
            case 2:
                // Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Confined;
                int_quitToMenuSequence = 0;
                if (bl_paused) bl_paused = false;
                break;
        }
    }

    //Go to previous screen
    public void BackButton()
    {
        SwitchScreenFancy(go_screenBuffer);
    }

    //Go to End Scene
    public void ToEnd()
    {
        switch (int_endSequence)
        {
            case 0:
                GameManager.playerController.TogglePlayerControl();
                Time.timeScale = 0;

                int_endSequence++;
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 1, 1f).setOnComplete(ToEnd).setIgnoreTimeScale(true);
                break;
            case 1:
                int_endSequence++;
                ClearScreens();
                SceneManager.LoadScene("EndScreen");
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(ToEnd).setIgnoreTimeScale(true);
                break;
            case 2:
                SwitchScreen(go_endScreen);
                // Cursor.lockState = CursorLockMode.Confined;
                int_endSequence = 0;
                break;
        }
    }

    //Close the game
    public void Quit()
    {
        Application.Quit();
    }

    //Go to Gameplay from Pause
    public void Unpause()
    {
        ClearScreenFancy();
        Time.timeScale = 1;
        if (GameManager.ghost != null)
        {
            GameManager.ghost.bl_frozen = false;
        }
        bl_paused = false;
    }

    public void TogglePause()
    {
        //Toggle Pause
        
        if (!bl_paused && bl_allowPause)
        {
            GameManager.playerController.TogglePlayerControl();
            //ClearScreens();
            go_nextScreen = go_pauseScreen;
            SwitchScreenTransition();

            // SwitchScreenFancy(go_pauseScreen);
            Time.timeScale = 0;
            if (GameManager.ghost != null)
            {
                GameManager.ghost.bl_frozen = true;
            }
            bl_paused = true;
        }
        else if (bl_paused)
        {
            Unpause();
        }
    }

    //Debug screen management
    public void EnterDebug()
    {
        go_debugScreen.SetActive(true);
    }

    public void ExitDebug()
    {
        go_debugScreen.SetActive(false);
    }

    //Volume management
    public void UpdateVolume()
    {
        Settings.flt_volume = sli_volume.value;
    }

    //Mouse sensitivity management
    public void UpdateLookSensitivity()
    {
        Settings.flt_lookSensitivity = sli_lookSensitivity.value;
    }
}