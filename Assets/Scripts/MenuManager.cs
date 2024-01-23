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

    //Singleton
    public static MenuManager instance;

    //Image Assets
    protected Image img_deathMessage;
    protected Image img_deathScreen;
    protected Image img_damageOverlay;
    protected Image img_fadeOverlay;

    public Image Img_damageOverlay { get { return img_damageOverlay; } set { img_damageOverlay = value; } }

    protected int int_enterSequence = 0;
    protected int int_deathSequence = 0;
    protected int int_quitToMenuSequence = 0;

    protected Slider sli_volume;
    protected Slider sli_lookSensitivity;

    protected GameObject go_OrientationNote;
    protected GameObject go_quitButton;
    private void Awake()
    {
        img_damageOverlay = FindObjectOfType<DamageOverlay>(true).GetComponent<Image>();
        img_deathMessage = FindObjectOfType<DeathMessage>(true).GetComponent<Image>();
        go_quitButton = GameObject.Find("DeathScreenQuit");
        img_deathScreen = GameObject.Find("DeathOverlay").GetComponent<Image>();
        img_fadeOverlay = GameObject.Find("FadeOverlay").GetComponent<Image>();
        go_OrientationNote = GameObject.Find("Note");
        sli_volume = GameObject.Find("VolumeSlider").GetComponent<Slider>();
        sli_lookSensitivity = GameObject.Find("LookSensitivitySlider").GetComponent<Slider>();

        sli_volume.value = Settings.flt_volume;
        sli_lookSensitivity.value = Settings.flt_lookSensitivity;

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
    }

    //Switch the currently displayed screen to be the designated screen
    public void SwitchScreen(GameObject screen)
    {
        ClearScreens();

        if (screen != go_pauseScreen && screen != go_startScreen)
            Time.timeScale = 1;

        screen.SetActive(true);
    }

    private void ClearScreens()
    {
        go_titleScreen.SetActive(false);
        go_optionsScreen.SetActive(false);
        go_gameScreen.SetActive(false);
        go_pauseScreen.SetActive(false);
        go_deathScreen.SetActive(false);
        go_startScreen.SetActive(false);
        go_endScreen.SetActive(false);
    }

    public void FadeIn()
    {
        LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0f, 0.2f);
    }

    public void FadeOut()
    {
        LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0f, 0.2f);
    }

    public void IncreaseDamageOverlay()
    {
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a + 0.33f, 0.2f);
    }

    public void IncreaseDamageOverlayTemporarily()
    {
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a + 0.33f, 0.2f).setOnComplete(ReduceDamageOverlay);
    }

    private void ReduceDamageOverlay()
    {
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a - 0.33f, 1f);
    }

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

    public void ResetMenus()
    {
        Color tempcolor = Img_damageOverlay.color;
        tempcolor.a = 0;
        Img_damageOverlay.color = tempcolor;

        go_quitButton.transform.position = new Vector3(0f, -300, 0f);
        go_OrientationNote.transform.position = new Vector3(0f, -500f, 0f);
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
                // Time.timeScale = 1;
                SwitchScreen(go_startScreen);
                LeanTween.moveLocal(go_OrientationNote, new Vector3(0f, 25f, 0f), 1f).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);
                Cursor.lockState = CursorLockMode.Confined;
                int_enterSequence = 0;
                break;
        }
    }

    //Start Game
    public void StartGame()
    {
        SwitchScreen(go_gameScreen);

        Cursor.lockState = CursorLockMode.Locked;

        GameManager.playerController.TogglePlayerControl();
    }

    //Go To Title Screen

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
                ClearScreens();
                SceneManager.LoadScene("TitleScene");
                LeanTween.alpha(img_fadeOverlay.GetComponent<RectTransform>(), 0, 1f).setOnComplete(QuitToTitleSequence).setIgnoreTimeScale(true);
                break;
            case 2:
                // Time.timeScale = 1;
                SwitchScreen(go_titleScreen);
                Cursor.lockState = CursorLockMode.Confined;
                int_quitToMenuSequence = 0;
                break;
        }
    }
    // public void ToTitle()
    // {
    //     SceneManager.LoadScene(0);
    //     SwitchScreen(go_titleScreen);
    //     GameManager.ResetGame();
    // }

    //Go to End Scene
    public void ToEnd()
    {
        SceneManager.LoadScene(2);
        SwitchScreen(go_endScreen);
        Cursor.lockState = CursorLockMode.Confined;
    }

    //Close the game
    public void Quit()
    {
        Application.Quit();
    }

    //Go to Gameplay from Pause
    public void Unpause()
    {
        SwitchScreen(go_gameScreen);
        GameManager.playerController.TogglePlayerControl();
    }

    private void Update()
    {
        //Toggle Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (go_gameScreen.activeSelf)
            {
                SwitchScreen(go_pauseScreen);
                GameManager.playerController.TogglePlayerControl();
                Time.timeScale = 0;
            }
            else if (go_pauseScreen.activeSelf)
            {
                SwitchScreen(go_gameScreen);
                GameManager.playerController.TogglePlayerControl();
                //Time.timeScale = 1;
            }
        }
    }

    public void UpdateVolume()
    {
        Settings.flt_volume = sli_volume.value;
    }

    public void UpdateLookSensitivity()
    {
        Settings.flt_lookSensitivity = sli_volume.value;
    }
}