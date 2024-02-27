using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.tvOS;
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

    //Tooltip Elements
    protected Image img_tooltipBackground;
    protected TextMeshProUGUI tmp_tooltipText;
    public Image Img_damageOverlay { get { return img_damageOverlay; } set { img_damageOverlay = value; } }

    // These ints keep track of the transitions between screens and scenes. For example, EnterGameSequence() uses it to iterate through its switch statement to perform transitions with LeanTween.
    protected int int_enterSequence = 0;
    protected int int_startSequence = 0;
    protected int int_deathSequence = 0;
    protected int int_endSequence = 0;
    protected int int_quitToMenuSequence = 0;
    protected int int_clearScreenSequence = 0;

    // Volume and Look Sensativity slider references
    protected Slider sli_volume;
    protected Slider sli_lookSensitivity;

    // A few miscellaneous screen elements that are animated via LeanTween
    protected GameObject go_OrientationNote;
    protected GameObject go_quitButton;
    protected GameObject go_startButton;

    // Bools related to the Pause system
    protected bool bl_paused = false;
    protected bool bl_allowPause = false;
    public bool Bl_allowPause { get { return bl_allowPause; } set { bl_allowPause = value; } }
    protected bool ready = true;

    private void Awake()
    {
        img_damageOverlay = FindObjectOfType<DamageOverlay>(true).GetComponent<Image>();
        img_deathMessage = FindObjectOfType<DeathMessage>(true).GetComponent<Image>();
        img_deathScreen = GameObject.Find("DeathOverlay").GetComponent<Image>();
        img_fadeOverlay = GameObject.Find("FadeOverlay").GetComponent<Image>();

        img_tooltipBackground = GameObject.Find("ToolTipBackground").GetComponent<Image>();
        tmp_tooltipText = GameObject.Find("ToolTipText").GetComponent<TextMeshProUGUI>();

        go_quitButton = GameObject.Find("DeathScreenQuit");
        go_startButton = GameObject.Find("StartScreenButton");
        go_OrientationNote = GameObject.Find("Note");
        
        sli_volume = GameObject.Find("VolumeSlider").GetComponent<Slider>();
        sli_lookSensitivity = GameObject.Find("LookSensitivitySlider").GetComponent<Slider>();

        sli_volume.value = Settings.flt_volume;
        sli_lookSensitivity.value = Settings.flt_lookSensitivity;

        go_debugScreen.SetActive(false);

        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.name == "TitleScene") SwitchScreen(go_titleScreen);
        else ClearScreens();

        go_screenBuffer = go_titleScreen;
    }

    // Brings up needed Menu screen with a LeanTween transition
    public void CallScreenWithTransition()
    {
        go_lastScreen.transform.localPosition = new Vector3(-750, 0f, 0f);

        if (go_lastScreen == go_optionsScreen || go_lastScreen == go_controlsScreen) LeanTween.moveLocal(go_screenBuffer, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);
        
        if(go_nextScreen == go_pauseScreen) LeanTween.moveLocal(go_nextScreen, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine).setOnComplete(AllowPause).setIgnoreTimeScale(true);
        else LeanTween.moveLocal(go_nextScreen, new Vector3(0f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);

        go_lastScreen = go_nextScreen;
    }

    // Dismisses Menu screen with a LeanTween transition
    public void DissmissScreenAndCallNext(GameObject screen)
    {
        if(ready)
        {
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
        if(screen != go_optionsScreen && screen != go_controlsScreen ) go_lastScreen = screen;
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
        switch(int_clearScreenSequence)
        {
            case 0:
                int_clearScreenSequence++;
                LeanTween.moveLocal(go_lastScreen, new Vector3(750f, 0f, 0f), Settings.flt_menuTransitionSpeed).setEase(LeanTweenType.easeInSine).setOnComplete(ClearScreenWithTransition).setIgnoreTimeScale(true);
                break;

            case 1:
                if (!bl_allowPause) bl_allowPause = true;
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

    //Shows death sequence with animation in steps
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
        img_deathScreen.color = tempcolor;
        img_deathMessage.color = tempcolor;

        go_quitButton.transform.localPosition = new Vector3(0f, -300, 0f);
        go_startButton.transform.localPosition = new Vector3(0f, -300, 0f);
        go_OrientationNote.transform.localPosition = new Vector3(0f, -500f, 0f);
    }

    // This handles the transition from the TitleScreen scene to the Game scene and brings up the Orientation Note and Start buttons with LeanTween with animation in steps
    public void EnterGameSequence()
    {
        switch (int_enterSequence)
        {
            case 0:
                ready = false;
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
                ready = true;
                break;
        }
    }

    // Once player clicks on the start button below the Orientation Note, this dismisses note and button, brings up the gameScreen, and enters play mode.
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

    // Go to previous screen, used with Options and Controls screens to go back to TitleScreen or Pause depending on context
    public void BackButton()
    {
        DissmissScreenAndCallNext(go_screenBuffer);
    }

    //Go to End Scene with LeanTween transitions
    public void ToEnd()
    {
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
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(ToEnd).setIgnoreTimeScale(true);
                break;
            case 2:
                Cursor.lockState = CursorLockMode.Confined;
                int_endSequence = 0;
                break;
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
        if(bl_allowPause)
        {
            bl_allowPause = false;

            ClearScreenWithTransition();
            Time.timeScale = 1;
            if (GameManager.ghost != null)
            {
                GameManager.ghost.bl_frozen = false;
            }
            bl_paused = false;
        }
    }

    // Toggles the pause state
    public void TogglePause()
    {
        if (!bl_allowPause) return;

        if (!bl_paused)
        {
            bl_allowPause = false;

            GameManager.playerController.TogglePlayerControl();
            //ClearScreens();
            go_nextScreen = go_pauseScreen;
            CallScreenWithTransition();

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

        if (bl_lookingAtObjectIsInteractable && go_lookingAtObject.GetComponent<Pickupable>() != null)
        {
            if (go_lookingAtObject.GetComponent<Pickupable>().bl_doorKnob) st_tooltipMessage = "Press \"E\" to grab handle";
            else st_tooltipMessage = "Press \"E\" to pick up " + go_lookingAtObject.name;
        }

        if (go_heldObject != null)
        {
            if (go_heldObject.GetComponent<Dish>() != null)
            {
                bool bl_goesInCupboard = FindObjectOfType<CupboardTrigger>().li_dishes.Contains(go_heldObject.GetComponent<Dish>());

                if (go_heldObject.GetComponent<Dish>().bl_dirtyDish) st_tooltipMessage = "Dunk this in the sink to clean";
                if (go_heldObject.GetComponent<Dish>().bl_broken) st_tooltipMessage = "Put this in the trash";
                if (go_heldObject.GetComponent<Dish>().bl_dirtyDish == false && bl_goesInCupboard) st_tooltipMessage = "Put this in the cupboard";
            }

            if (go_heldObject.GetComponent<Book>() != null)
            {
                bool bl_goesOnShelf = FindObjectOfType<LibraryManager>().l_books.Contains(go_heldObject.GetComponent<Book>());

                if (bl_goesOnShelf) st_tooltipMessage = "Put this on the bookshelf";
            }

            if (go_heldObject.GetComponent<Toy>() != null)
            {
                bool bl_goesInToybox = FindObjectOfType<ToyChestTrigger>().li_toys.Contains(go_heldObject.GetComponent<Toy>());

                if (bl_goesInToybox) st_tooltipMessage = "Put this in the toy chest";
            }

            if (go_heldObject.GetComponent<Pickupable>().bl_remote)
            {
                bool bl_tvOn = FindObjectOfType<TVStatic>().bl_on;

                if (bl_tvOn) st_tooltipMessage = "Turn off the TV";
            }
        }

        // If the object the player is looking at is interactable, but not pickupable:

        if (bl_lookingAtObjectIsInteractable && go_lookingAtObject.GetComponent<Pickupable>() == null)
        {
            if (go_lookingAtObject.GetComponent<LightSwitch>() != null) st_tooltipMessage = "Press \"E\" to toggle switch";

            if (go_lookingAtObject.GetComponent<FuseBox>() != null) st_tooltipMessage = "Press \"E\" to toggle breaker";

            if (go_lookingAtObject.GetComponent<Cobweb>() != null)
            {
                if(go_lookingAtObject.GetComponent<Cobweb>().bl_cleaned == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_duster == false) st_tooltipMessage = "Duster required to clean";
                    else st_tooltipMessage = "Touch with duster";
                }
            }

            if (go_lookingAtObject.GetComponent<Mirror>() != null && go_lookingAtObject.GetComponent<Mirror>().bl_gameActive == false)
            {
                if (go_lookingAtObject.GetComponent<Mirror>().bl_clean == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_duster == false) st_tooltipMessage = "Duster required to clean";
                    else st_tooltipMessage = "Press \"E\" to clean";
                }
            }

            if (go_lookingAtObject.GetComponent<FloorMess>() != null)
            {
                if (go_lookingAtObject.GetComponent<FloorMess>().bl_clean == false && go_lookingAtObject.GetComponent<FloorMess>().bl_gameActive == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_mop == false) st_tooltipMessage = "Broom required to clean";
                    else st_tooltipMessage = "Press \"E\" to clean";
                }
            }

            if (go_lookingAtObject.GetComponent<Fireplace>() != null)
            {
                if (go_lookingAtObject.GetComponent<Fireplace>().bl_lit == false)
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_lighter == false) st_tooltipMessage = "Lighter required to light";
                    else st_tooltipMessage = "Press \"E\" to light fireplace";
                }
            }

            if (go_lookingAtObject.GetComponent<DrawerOpen>() != null)
            {
                if (go_lookingAtObject.GetComponent<DrawerOpen>().Bl_ready == true && go_lookingAtObject.GetComponent<DrawerOpen>().Bl_open == false) st_tooltipMessage = "Press \"E\" to open drawer";
            }

            if (go_lookingAtObject.GetComponent<Exit>() != null)
            {
                if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
                {
                    if (go_heldObject == null || go_heldObject.GetComponent<Pickupable>().bl_frontDoorKey == false) st_tooltipMessage = "Find the key to leave";
                    else st_tooltipMessage = "Press \"E\" to leave house";
                }
                else st_tooltipMessage = "Complete your task list before you leave";
            }
        }

        if(st_tooltipMessage != null)
        {
            tmp_tooltipText.text = st_tooltipMessage;
            img_tooltipBackground.rectTransform.sizeDelta = new Vector2(tmp_tooltipText.text.Length * 8, img_tooltipBackground.rectTransform.sizeDelta.y);
        }
        else
        {
            img_tooltipBackground.gameObject.SetActive(false);
            tmp_tooltipText.gameObject.SetActive(false);
        }
    }
}