using Cinemachine;
using System;
using System.Collections.Generic;
// using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using static System.Net.Mime.MediaTypeNames;

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
    public GameObject go_fpsScreen;

    //Credits Sequence Objects
    protected GameObject go_creditsScreensHolder;
    protected bool bl_creditsSequenceRunning = false;
    protected List<GameObject> li_creditScreens;

    protected int int_creditsScreenTracker = 0;
    protected int int_creditsScreenSequence = 0;

    protected bool bl_runCreditsTimer = false;
    protected float flt_creditsTimer = 3;

    protected Color co_transparent;
    protected Color co_opaque;

    // Screen Variables used in screen transitions, and the ability to go back and forth within Main Menu and Pause
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
    public Image img_crosshair;

    //Tooltip Elements
    protected Image img_tooltipBackground;
    protected TextMeshProUGUI tmp_tooltipText;

    //Chore Notification Elements
    protected bool bl_runNotificationTimer = false;
    protected GameObject go_choreNotificationHolder;
    protected Image img_notificationBackground;
    protected TextMeshProUGUI tmp_notificationText;
    protected float flt_notificationTimer = -1;

    //Chore List and Current Chore variables
    public bool bl_choreListUp = false;
    protected GameObject go_choreSheet;
    protected GameObject go_currentChoreLabel;
    protected TextMeshProUGUI tmp_currentChore;

    public Image Img_damageOverlay { get { return img_damageOverlay; } set { img_damageOverlay = value; } }

    // These ints keep track of the transitions between screens and scenes. For example, EnterGameSequence() uses it to iterate through its switch statement to perform transitions with LeanTween.
    protected int int_enterSequence = 0;
    protected int int_startSequence = 0;
    protected int int_deathSequence = 0;
    protected int int_endSequence = 0;
    protected int int_quitToMenuSequence = 0;
    protected int int_clearScreenSequence = 0;
    private bool bl_transitioningScene = false;

    // Volume and Look Sensativity slider references
    protected Slider sli_volume;
    protected Slider sli_musicVolume;
    protected Slider sli_lookSensitivity;

    // A few miscellaneous screen elements that are animated via LeanTween
    protected GameObject go_OrientationNote;
    protected GameObject go_quitButton;
    protected GameObject go_startButton;
    protected GameObject go_restartButton;

    // Bools related to the Pause system
    protected bool bl_paused = false;
    public bool Bl_paused { get { return bl_paused; } set { bl_paused = value; } }
    protected bool bl_allowPause = false;
    public bool Bl_allowPause { get { return bl_allowPause; } set { bl_allowPause = value; } }
    protected bool ready = true;

    public event EventHandler<EventArgs> GamePaused;
    public event EventHandler<EventArgs> GameUnpaused;
    public event EventHandler<EventArgs> GameStart;
    public event EventHandler<EventArgs> StartTransitionToGame;
    public event EventHandler<EventArgs> MenuEntered;
    public event EventHandler<EventArgs> CreditsEntered;
    public event EventHandler<EventArgs> DeathScreenEntered;
    public event EventHandler<EventArgs> SoundVolumeChanged;
    public event EventHandler<EventArgs> MusicVolumeChanged;
    public event EventHandler<EventArgs> StartScreenClosed;
    public event EventHandler<EventArgs> StartQuitingGame;

    protected bool bl_initialized = false;

    protected bool bl_gameStarted = false;

    // audio clip to play when SFX volume slider is updated
    [SerializeField] AudioClip ac_sliderUpdate;

    // This was once the Awake method, but it was found that GameManager's Awake method was running first, and FadeIn() method needed components that weren't ready yet
    public void InitializeMenuManager()
    {
        if(!bl_initialized)
        {
            // These are primary menu related image component references:
            img_damageOverlay = FindObjectOfType<DamageOverlay>(true).GetComponent<Image>();
            img_deathMessage = FindObjectOfType<DeathMessage>(true).GetComponent<Image>();
            img_deathScreen = GameObject.Find("DeathOverlay").GetComponent<Image>();
            img_fadeOverlay = GameObject.Find("FadeOverlay").GetComponent<Image>();

            // These are credits sequence related variables:
            go_creditsScreensHolder = GameObject.Find("CreditsScreens");

            li_creditScreens = new();

            foreach (Transform creditsScreen in go_creditsScreensHolder.transform)
            {
                li_creditScreens.Add(creditsScreen.gameObject);
            }

            co_opaque = new();
            co_opaque = Color.white;
            co_transparent = new();
            co_transparent = co_opaque;
            co_transparent.a = 0;

            foreach (GameObject creditsScreen in li_creditScreens)
            {
                creditsScreen.transform.Find("Picture").GetComponent<Image>().color = co_transparent;

                GameObject textObjectHolder = creditsScreen.transform.Find("TextObjects").gameObject;

                foreach (Transform textObject in textObjectHolder.transform)
                {
                    if (textObject.GetComponent<TextMeshProUGUI>() != null) textObject.GetComponent<TextMeshProUGUI>().color = co_transparent;
                }
            }

            // These components make up the player tooltip:
            img_tooltipBackground = GameObject.Find("ToolTipBackground").GetComponent<Image>();
            tmp_tooltipText = GameObject.Find("ToolTipText").GetComponent<TextMeshProUGUI>();

            // These components make up the player chore notifications:
            bl_runNotificationTimer = false;
            go_choreNotificationHolder = GameObject.Find("ChoreNotification");
            img_notificationBackground = GameObject.Find("ChoreNotificationBackground").GetComponent<Image>();
            tmp_notificationText = GameObject.Find("ChoreNotificationText").GetComponent<TextMeshProUGUI>();

            // This is the player's Current Chore info in the corner of their screen
            go_choreSheet = GameObject.Find("ChoreSheet");
            tmp_currentChore = GameObject.Find("CurrentChoreText").GetComponent<TextMeshProUGUI>();
            tmp_currentChore.gameObject.SetActive(false);

            // These components have references because they are animated into/out of view
            go_quitButton = GameObject.Find("DeathScreenQuit");
            go_startButton = GameObject.Find("StartScreenButton");
            go_OrientationNote = GameObject.Find("Note");
            go_restartButton = GameObject.Find("RestartButton");

            // These are the references to the Options screen slider components:
            sli_volume = GameObject.Find("VolumeSlider").GetComponent<Slider>();
            sli_musicVolume = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
            sli_lookSensitivity = GameObject.Find("LookSensitivitySlider").GetComponent<Slider>();

            // We set the volume and look sensitivity to the defaults defined in Settings
            sli_volume.value = Settings.flt_volume;
            sli_musicVolume.value = Settings.flt_musicVolume;
            sli_lookSensitivity.value = Settings.flt_lookSensitivity;

            // The debug screen is a special case screen that is not set active or deactivated elsewhere, so it is deactivated here
            go_debugScreen.SetActive(false);

            // The game should load into the Title Scene, and so we show the title scene object. Otherwise we just show a blank screen.
            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.name == "TitleScene") SwitchScreen(go_titleScreen);
            else ClearScreens();

            // The screen buffer is used in animation transitions, and we set it to TItleScreen so it is not null at start
            go_screenBuffer = go_titleScreen;

            bl_initialized = true;
        }
    }

    public void Start()
    {
        GameManager.taskManager.ChoreCompleted += ChoreCompleteNotification;
        GameManager.taskManager.ChoreUpdated += ChoreUpdatedNotification;
    }

    public void Update()
    {
        // when a chore is completed or updated, this counts down and then dismisses the chore notification
        if (bl_runNotificationTimer)
        {
            if (flt_notificationTimer > 0)
            {
                flt_notificationTimer -= Time.deltaTime;
            }
            else
            {
                bl_runNotificationTimer = false;
                HideChoreNotification();
            }
        }

        // this handles the timing between credits slides
        if (bl_runCreditsTimer)
        {
            if (flt_creditsTimer > 0)
            {
                flt_creditsTimer -= Time.deltaTime;
            }
            else
            {
                bl_runCreditsTimer = false;
                flt_creditsTimer = 3;
                CheckCreditSequenceDone();
                //CreditsHide();
            }
        }
    }

    // Hides the player's GUI
    public void ToggleGUI()
    {
        if (go_gameScreen.activeSelf)
        {
            go_gameScreen.SetActive(false);
            tmp_currentChore.gameObject.SetActive(false);
        }
        else
        {
            go_gameScreen.SetActive(true);
            tmp_currentChore.gameObject.SetActive(true);
        }
    }

    // Brings up needed Menu screen with a LeanTween transition
    public void CallScreenWithTransition()
    {
        go_lastScreen.transform.localPosition = new Vector3(-750, 0f, 0f);

        if (go_lastScreen == go_optionsScreen || go_lastScreen == go_controlsScreen) LeanTween.moveLocal(go_screenBuffer, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);

        if (go_nextScreen == go_pauseScreen) LeanTween.moveLocal(go_nextScreen, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine).setOnComplete(AllowPause).setIgnoreTimeScale(true);
        else LeanTween.moveLocal(go_nextScreen, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setOnComplete(AllowPause).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);

        go_lastScreen = go_nextScreen;
    }

    // Dismisses Menu screen with a LeanTween transition
    public void DissmissScreenAndCallNext(GameObject screen)
    {
        if (ready)
        {
            bl_allowPause = false;

            go_nextScreen = screen;

            if (screen == go_gameScreen) LeanTween.moveLocal(go_lastScreen, new Vector3(750f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeInSine).setOnComplete(SwitchToGame).setIgnoreTimeScale(true);
            else LeanTween.moveLocal(go_lastScreen, new Vector3(750f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeInSine).setOnComplete(CallScreenWithTransition).setIgnoreTimeScale(true);

            if (go_nextScreen == go_optionsScreen || go_nextScreen == go_controlsScreen) go_screenBuffer = go_lastScreen;
        }
    }

    // Used to bring up the Game menu screen after LeanTween transition
    public void SwitchToGame()
    {
        go_gameScreen.SetActive(true);
    }

    //Switch the currently displayed screen to be the designated without LeanTween transition
    public void SwitchScreen(GameObject screen)
    {
        ClearScreens();

        if (screen != go_pauseScreen && screen != go_startScreen)
            Time.timeScale = 1;

        screen.SetActive(true);
        if (screen != go_optionsScreen && screen != go_controlsScreen) go_lastScreen = screen;
    }

    //Set all screens to inactive
    private void ClearScreens()
    {
        go_titleScreen.SetActive(false);
        go_gameScreen.SetActive(false);
        go_deathScreen.SetActive(false);
        go_startScreen.SetActive(false);
        go_endScreen.SetActive(false);
    }

    // Clears screens like above, but uses LeanTween transition
    private void ClearScreenWithTransition()
    {
        switch (int_clearScreenSequence)
        {
            case 0:
                int_clearScreenSequence++;
                LeanTween.moveLocal(go_lastScreen, new Vector3(750f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeInSine).setOnComplete(ClearScreenWithTransition).setIgnoreTimeScale(true);
                break;

            case 1:
                if (!bl_allowPause) bl_allowPause = true;
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
        LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 1f, 0.2f);
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

    //Shows death sequence with animation in steps
    public void ShowDeathSequence()
    {
        switch (int_deathSequence)
        {
            case 0:
                Time.timeScale = 0;
                Bl_allowPause = false;
                go_deathScreen.SetActive(true);
                if (DeathScreenEntered != null)
                    DeathScreenEntered(this, new EventArgs());
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
        img_deathScreen.color = tempcolor;
        img_deathMessage.color = tempcolor;

        go_quitButton.transform.localPosition = new Vector3(0f, -300, 0f);
        go_startButton.transform.localPosition = new Vector3(0f, -300, 0f);
        go_OrientationNote.transform.localPosition = new Vector3(0f, -500f, 0f);
        go_restartButton.transform.localPosition = new Vector3(0f, -500, 0f);
        bl_gameStarted = false;

        foreach (GameObject creditsScreen in li_creditScreens)
        {
            creditsScreen.transform.Find("Picture").GetComponent<Image>().color = co_transparent;

            GameObject textObjectHolder = creditsScreen.transform.Find("TextObjects").gameObject;

            foreach (Transform textObject in textObjectHolder.transform)
            {
                if (textObject.GetComponent<TextMeshProUGUI>() != null) textObject.GetComponent<TextMeshProUGUI>().color = co_transparent;
            }
        }

        tmp_currentChore.gameObject.SetActive(false);
    }

    // This handles the transition from the TitleScreen scene to the Game scene and brings up the Orientation Note and Start buttons with LeanTween with animation in steps
    public void EnterGameSequenceBtn()
    {
        if (bl_transitioningScene) return;
        EnterGameSequence();
    }
    public void EnterGameSequence()
    {
        switch (int_enterSequence)
        {
            case 0:
                bl_transitioningScene = true;
                if (StartTransitionToGame != null)
                    StartTransitionToGame(this, new EventArgs());
                ready = false;
                int_enterSequence++;
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 1, 1.5f).setOnComplete(EnterGameSequence).setIgnoreTimeScale(true);
                break;
            case 1:
                int_enterSequence++;
                SceneManager.LoadScene(1);
                ClearScreens();
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(EnterGameSequence);
                break;
            case 2:
                int_enterSequence++;
                SwitchScreen(go_startScreen);
                if (GameStart != null)
                    GameStart(this, new EventArgs());
                LeanTween.moveLocal(go_OrientationNote, new Vector3(0f, 25f, 0f), 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete(EnterGameSequence).setIgnoreTimeScale(true);
                break;
            case 3:
                LeanTween.moveLocal(go_startButton, new Vector3(0f, -135f, 0f), 0.5f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
                int_enterSequence = 0;
                Cursor.lockState = CursorLockMode.Confined;
                ready = true;
                bl_transitioningScene = false;
                break;
        }
    }

    // Once player clicks on the start button below the Orientation Note, this dismisses note and button, brings up the gameScreen, and enters play mode.
    public void StartGameSequence()
    {
        switch (int_startSequence)
        {
            case 0:
                if (bl_gameStarted) return;
                bl_gameStarted = true;
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
                if (StartScreenClosed != null)
                    StartScreenClosed(this, new EventArgs());
                break;
        }
    }

    //Go To Title Screen with fade transitions
    public void QuitToTitleSequenceBtn()
    {
        if (bl_transitioningScene) return;
        QuitToTitleSequence();
    }
    public void QuitToTitleSequence()
    {
        switch (int_quitToMenuSequence)
        {
            case 0:
                bl_transitioningScene = true;
                int_quitToMenuSequence++;
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 1, 1f).setOnComplete(QuitToTitleSequence).setIgnoreTimeScale(true);
                break;

            case 1:
                int_quitToMenuSequence++;
                GameManager.ResetGame();
                go_pauseScreen.transform.localPosition = new Vector3(-1000, 0, 0);
                SceneManager.LoadScene("TitleScene");
                SwitchScreen(go_titleScreen);
                if (MenuEntered != null)
                    MenuEntered(this, new EventArgs());
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(QuitToTitleSequence).setIgnoreTimeScale(true);
                break;
            case 2:
                Cursor.lockState = CursorLockMode.Confined;
                int_quitToMenuSequence = 0;
                if (bl_paused) bl_paused = false;
                bl_transitioningScene = false;
                break;
        }
    }

    // Go to previous screen, used with Options and Controls screens to go back to TitleScreen or Pause depending on context
    public void BackButton()
    {
        DissmissScreenAndCallNext(go_screenBuffer);
    }

    //Go to End Scene with LeanTween transitions
    public void ToEnd()
    {
        tmp_currentChore.gameObject.SetActive(false);

        switch (int_endSequence)
        {
            case 0:
                int_endSequence++;
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 1, 1f).setOnComplete(ToEnd).setIgnoreTimeScale(true);
                break;
            case 1:
                int_endSequence++;
                ClearScreens();
                SceneManager.LoadScene("EndScreen");
                SwitchScreen(go_endScreen);
                if (CreditsEntered != null)
                    CreditsEntered(this, new EventArgs());
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(ToEnd).setIgnoreTimeScale(true);
                break;
            case 2:
                Cursor.lockState = CursorLockMode.Confined;
                int_endSequence = 0;
                CreditsShow();
                break;
        }
    }

    // this tweens in the image and text for a credits slide
    private void CreditsShow()
    {
        switch(int_creditsScreenSequence)
        {
            case 0:
                int_creditsScreenSequence++;
                LeanTween.alpha(li_creditScreens[int_creditsScreenTracker].transform.Find("Picture").GetComponent<RectTransform>(), 1, 1f).setOnComplete(CreditsShow).setIgnoreTimeScale(true);
                break;
            case 1:
                int_creditsScreenSequence++;
                LeanTween.value(li_creditScreens[int_creditsScreenTracker].transform.Find("Picture").gameObject, 0, 1, 1f).setOnUpdate(UpdateTextAlpha).setOnComplete(CreditsShow).setIgnoreTimeScale(true);
                break;
            case 2:
                int_creditsScreenSequence = 0;
                bl_runCreditsTimer = true;
                break;
        }
    }

    // this tweens out the image and text for a credits slide

    private void CreditsHide()
    {
        var color = Color.white;
        var fadeoutcolor = color;
        fadeoutcolor.a = 0;

        switch (int_creditsScreenSequence)
        {
            case 0:
                int_creditsScreenSequence++;
                // LeanTween.textAlpha(li_creditScreens[int_creditsScreenTracker].transform.Find("Role").GetComponent<RectTransform>(), 0, 1f).setOnUpdate().setOnComplete(CreditsHide).setIgnoreTimeScale(true);
                LeanTween.value(li_creditScreens[int_creditsScreenTracker].transform.Find("Picture").gameObject, 1, 0, 1f).setOnUpdate(UpdateTextAlpha).setOnComplete(CreditsHide).setIgnoreTimeScale(true);
                break;
            case 1:
                int_creditsScreenSequence++;
                LeanTween.alpha(li_creditScreens[int_creditsScreenTracker].transform.Find("Picture").GetComponent<RectTransform>(), 0, 1f).setOnComplete(CreditsHide).setIgnoreTimeScale(true);
                break;
            case 2:
                int_creditsScreenSequence = 0;
                int_creditsScreenTracker++;
                CreditsShow();
                break;
        }
    }

    // this is part of the tweening of text, this applies the tweened value to the text's alpha
    private void UpdateTextAlpha(float alpha)
    {
        Color color = Color.white;
        color.a = alpha;

        GameObject textObjectHolder = li_creditScreens[int_creditsScreenTracker].transform.Find("TextObjects").gameObject;

        foreach(Transform textObject in textObjectHolder.transform)
        {
            if(textObject.GetComponent<TextMeshProUGUI>() != null) textObject.GetComponent<TextMeshProUGUI>().color = color;
        }
    }

    // if there are no more slides, this shows the last message and the back button
    private void CheckCreditSequenceDone()
    {
        if (int_creditsScreenTracker != li_creditScreens.Count - 1)
        {
            CreditsHide();
        }
        else
        {
            int_creditsScreenTracker = 0;
            LeanTween.moveLocal(go_restartButton, new Vector3(0f, -110f, 0f), 1f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);

        }
    }

    //Close the game
    public void Quit()
    {
        Application.Quit();
    }

    //Go to Gameplay from Pause. Used by below TogglePause method, and to Pause Menu's 'continue' button
    public void Unpause()
    {
        if (bl_allowPause)
        {
            bl_allowPause = false;

            ClearScreenWithTransition();
            Time.timeScale = 1;
            if (GameManager.ghost != null)
            {
                GameManager.ghost.bl_frozen = false;
            }
            bl_paused = false;

            if (GameUnpaused != null)
                GameUnpaused(this, new EventArgs());
        }
    }

    // Toggles the pause state
    public void TogglePause()
    {
        if (!bl_allowPause) return;

        if (!bl_paused)
        {
            if (bl_choreListUp) ToggleChoreSheet();

            bl_allowPause = false;
            bl_paused = true;

            if (GamePaused != null)
                GamePaused(this, new EventArgs());

            go_nextScreen = go_pauseScreen;
            CallScreenWithTransition();

            Time.timeScale = 0;
            if (GameManager.ghost != null)
            {
                GameManager.ghost.bl_frozen = true;
            }
        }
        else if (bl_paused)
        {
            Unpause();
        }
    }

    public void AllowPause()
    {
        bl_allowPause = true;
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

    public void EnterFPS()
    {
        go_fpsScreen.SetActive(true);
    }

    public void ExitFPS()
    {
        go_fpsScreen.SetActive(false);
    }

    //Volume management
    public void UpdateEffectsVolume()
    {
        Settings.flt_volume = sli_volume.value;

        if (SoundVolumeChanged != null)
            SoundVolumeChanged(this, new EventArgs());
    }

    // play slider update SFX when player ends drag on SFX slider
    public void PlaySFXUpdateClip()
    {
        GetComponent<AudioSource>().PlayOneShot(ac_sliderUpdate, Settings.flt_volume);
    }

    //Music Volume management
    public void UpdateMusicVolume()
    {
        Settings.flt_musicVolume = sli_musicVolume.value;

        if (MusicVolumeChanged != null)
            MusicVolumeChanged(this, new EventArgs());
    }

    //Mouse sensitivity management
    public void UpdateLookSensitivity()
    {
        Settings.flt_lookSensitivity = sli_lookSensitivity.value;
    }

    //Updating the player's tooltip:
    public void UpdateTooltip(GameObject go_lookingAtObject, GameObject go_heldObject)
    {
        bool bl_lookingAtObjectIsInteractable = false;
        string st_tooltipMessage = null;

        if (go_lookingAtObject != null)
        {
            if (go_lookingAtObject.GetComponent<Interactable>() != null || go_lookingAtObject.GetComponent<Cobweb>() != null) bl_lookingAtObjectIsInteractable = true;
        }

        img_tooltipBackground.gameObject.SetActive(true);
        tmp_tooltipText.gameObject.SetActive(true);

        // If the object is interactable and pickupable, this checks if it is a doorknob. If not, propts player to pick up the object.

        if (bl_lookingAtObjectIsInteractable && go_lookingAtObject.GetComponent<Pickupable>() != null)
        {
            if (go_lookingAtObject.GetComponent<Pickupable>().bl_doorKnob) st_tooltipMessage = "Press \"E\" to grab handle";
            else st_tooltipMessage = "Press \"E\" to pick up " + go_lookingAtObject.name;
        }

        //If the player is holding an object..
        if (go_heldObject != null)
        {
            //..and the object is a dish:
            if (go_heldObject.GetComponent<Dish>() != null)
            {
                bool bl_goesInCupboard = FindObjectOfType<CupboardTrigger>().li_dishes.Contains(go_heldObject.GetComponent<Dish>());

                if (go_heldObject.GetComponent<Dish>().bl_dirtyDish) st_tooltipMessage = "Dunk this in the sink to clean";
                if (go_heldObject.GetComponent<Dish>().bl_broken) st_tooltipMessage = "Put this in the trash";
                if (go_heldObject.GetComponent<Dish>().bl_dirtyDish == false && bl_goesInCupboard) st_tooltipMessage = "Put this in the cupboard";
            }

            //..and the object is a book:
            else if (go_heldObject.GetComponent<Book>() != null)
            {
                bool bl_goesOnShelf = FindObjectOfType<LibraryManager>().l_books.Contains(go_heldObject.GetComponent<Book>());

                if (bl_goesOnShelf) st_tooltipMessage = "Put this on the bookshelf";
            }

            //..and the object is a toy:
            else if (go_heldObject.GetComponent<Toy>() != null)
            {
                bool bl_goesInToybox = FindObjectOfType<ToyChestTrigger>().li_toys.Contains(go_heldObject.GetComponent<Toy>());

                if (bl_goesInToybox) st_tooltipMessage = "Put this in the toy chest";
            }

            //..and the object is the TV remote:
            else if (go_heldObject.GetComponent<Pickupable>().bl_remote)
            {
                bool bl_tvOn = FindObjectOfType<TVStatic>().bl_on;

                if (bl_tvOn) st_tooltipMessage = "Turn off the TV";
            }

            //..and the object is the lighter, while looking at a candle:
            else if (go_heldObject.GetComponent<Pickupable>().bl_lighter && go_lookingAtObject)
            {
                if(go_lookingAtObject.GetComponent<Candle>() && !go_lookingAtObject.GetComponent<Candle>().bl_lit) st_tooltipMessage = "Press \"E\" to light" + go_lookingAtObject.name;
                else if(go_lookingAtObject.GetComponent<Candle>() && go_lookingAtObject.GetComponent<Candle>().bl_lit)st_tooltipMessage = "Press \"E\" to pick up " + go_lookingAtObject.name;
            }
        }

        // If the object the player is looking at is interactable, but not pickupable..
        if (bl_lookingAtObjectIsInteractable && go_lookingAtObject.GetComponent<Pickupable>() == null)
        {
            //..and the object is a cupboard:
            if (go_lookingAtObject.GetComponent<CabinetDoorController>() && go_lookingAtObject.GetComponent<CabinetDoorController>().Bl_ready) st_tooltipMessage = "Press \"E\" to use cupboard door";

            //..and the obejct is a light switch:
            else if (go_lookingAtObject.GetComponent<LightSwitch>() != null) st_tooltipMessage = "Press \"E\" to toggle switch";

            //..and the object is the breaker box:
            else if (go_lookingAtObject.GetComponent<FuseBox>() != null) st_tooltipMessage = "Press \"E\" to toggle breaker";

            //..and the object is a cobweb:
            else if (go_lookingAtObject.GetComponent<Cobweb>() != null)
            {
                if (go_lookingAtObject.GetComponent<Cobweb>().bl_cleaned == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_duster == false) st_tooltipMessage = "Duster required to clean";
                    else st_tooltipMessage = "Touch with duster";
                }
            }

            //..and the object is the radio:
            else if (go_lookingAtObject.GetComponent<Radio>() != null)
            {
                if (go_lookingAtObject.GetComponent<Radio>().bl_playing == true) st_tooltipMessage = "Press \"E\" to turn radio off";
                else st_tooltipMessage = "Press \"E\" to turn radio on";
            }

            //..and the object is the mirror chore:
            else if (go_lookingAtObject.GetComponent<Mirror>() != null && go_lookingAtObject.GetComponent<Mirror>().bl_gameActive == false)
            {
                if (go_lookingAtObject.GetComponent<Mirror>().bl_clean == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_soapBar == false) st_tooltipMessage = "Soap required to clean";
                    else st_tooltipMessage = "Press \"E\" to clean";
                }
            }

            //..and the object is the floor mess chore:
            else if (go_lookingAtObject.GetComponent<FloorMess>() != null)
            {
                if (go_lookingAtObject.GetComponent<FloorMess>().bl_clean == false && go_lookingAtObject.GetComponent<FloorMess>().bl_gameActive == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_mop == false) st_tooltipMessage = "Broom required to clean";
                    else st_tooltipMessage = "Press \"E\" to clean";
                }
            }

            //..and the object is the fireplace:
            else if (go_lookingAtObject.GetComponent<Fireplace>() != null)
            {
                if (go_lookingAtObject.GetComponent<Fireplace>().bl_lit == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_lighter == false) st_tooltipMessage = "Lighter required to light";
                    else st_tooltipMessage = "Press \"E\" to light fireplace";
                }
            }

            //..and the object is the TV:
            else if (go_lookingAtObject.GetComponent<TVStatic>() != null)
            {
                if (go_lookingAtObject.GetComponent<TVStatic>().bl_on == true)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_remote == false) st_tooltipMessage = "Remote required to turn off";
                    else st_tooltipMessage = "Press \"E\" to turn off TV";
                }
            }

            //..and the object is a drawer:
            else if (go_lookingAtObject.GetComponent<DrawerOpen>() != null)
            {
                if (go_lookingAtObject.GetComponent<DrawerOpen>().Bl_ready == true && go_lookingAtObject.GetComponent<DrawerOpen>().Bl_open == false) st_tooltipMessage = "Press \"E\" to open drawer";
            }

            //..and the object is the exit door:
            else if (go_lookingAtObject.GetComponent<Exit>() != null)
            {
                if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_frontDoorKey == false) st_tooltipMessage = "Find the key to leave";
                    else st_tooltipMessage = "Press \"E\" to leave house";
                }
                else st_tooltipMessage = "Complete your chore list before you leave";
            }
        }

        // If the player is in inactive state, it'll reset the tooltip to null, resulting in it getting dismissed from the screen
        if (GameManager.playerController.En_state == PlayerController.State.inactive) st_tooltipMessage = null;

        // The tooltip message is set here, and its background adjusted to its contents:
        if (st_tooltipMessage != null)
        {
            tmp_tooltipText.text = st_tooltipMessage;
            img_tooltipBackground.rectTransform.sizeDelta = new Vector2(tmp_tooltipText.text.Length * 8, img_tooltipBackground.rectTransform.sizeDelta.y);
        }

        // If the tooltip message is never set, or is set back to null at the end, then this deactivates message and background components
        else
        {
            img_tooltipBackground.gameObject.SetActive(false);
            tmp_tooltipText.gameObject.SetActive(false);
        }
    }

    // This is attached to a listener that fires when a chore is complete
    public void ChoreCompleteNotification(object source, EventArgs e)
    {
        tmp_notificationText.text = "Chore Completed!";
        img_notificationBackground.rectTransform.sizeDelta = new Vector2(tmp_notificationText.text.Length * 8, img_notificationBackground.rectTransform.sizeDelta.y);

        ShowChoreNotification();
    }

    // This is attached to a listener that fires when a chore is updated
    public void ChoreUpdatedNotification(object source, EventArgs e)
    {
        tmp_notificationText.text = "Chore Sheet Updated!";
        img_notificationBackground.rectTransform.sizeDelta = new Vector2(tmp_notificationText.text.Length * 8, img_notificationBackground.rectTransform.sizeDelta.y);

        ShowChoreNotification();
    }

    // This handles the tweening the chore notification on screen
    protected void ShowChoreNotification()
    {
        flt_notificationTimer = Settings.flt_notificationTimer;

        LeanTween.moveLocal(go_choreNotificationHolder, new Vector3(250f, 160f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine);

        bl_runNotificationTimer = true;
    }

    // This handles tweening the chore notification away
    protected void HideChoreNotification()
    {
        LeanTween.moveLocal(go_choreNotificationHolder, new Vector3(500, 160f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine);
    }

    // this brings the chore sheet up and down with a tween
    public void ToggleChoreSheet()
    {
        if (go_choreSheet.transform.localPosition.y > -5)
        {
            bl_choreListUp = false;
            GameManager.playerController.TogglePlayerControl();
            LeanTween.moveLocal(go_choreSheet, new Vector3(0f, -500f, 0f), 0.5f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
        }
        else if (go_choreSheet.transform.localPosition.y < -495)
        {
            bl_choreListUp = true;
            GameManager.playerController.TogglePlayerControl();
            LeanTween.moveLocal(go_choreSheet, new Vector3(0f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
        }
    }

    // This updates the Current Chore text in the top right
    public void UpdateCurrentChore(string choreString)
    {
        if (tmp_currentChore.gameObject.activeSelf == false) tmp_currentChore.gameObject.SetActive(true);
        tmp_currentChore.text = choreString;
    }
}