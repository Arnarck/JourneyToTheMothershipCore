using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject PauseGameInterface = null;
    [SerializeField] GameObject gameStoryUpdateMessage = null;
    [SerializeField] GameObject finalStoryText = null;
    [SerializeField] AudioClip buttonSelected = null;

    AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        MakeDecisionBasedOnTheCurrentScene();
    }

    void MakeDecisionBasedOnTheCurrentScene()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int mainMenuIndex = 0;
        int endCutsceneIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (currentLevelIndex == endCutsceneIndex)
        {
            ChangeGameObjectsStateInEndGameMenu();
        }

        if (currentLevelIndex == mainMenuIndex)
        {
            ChangeGameObjectsStateInMainMenu();
        }
    }

    void ChangeGameObjectsStateInEndGameMenu()
    {
        if (!GetPlayerPrefsBool("isGameFinished"))
        {
            gameStoryUpdateMessage.SetActive(true);
            SetPlayerPrefsBool("isGameFinished", true);
        }
    }

    void ChangeGameObjectsStateInMainMenu()
    {
        if (GetPlayerPrefsBool("isGameFinished"))
        {
            finalStoryText.SetActive(true);
        }
    }

    public void ModifyMenuActiveState(GameObject menu)
    {
        _audioManager.PlayClip(buttonSelected);
        menu.SetActive(!menu.activeSelf);
    }

    public void RestartGame()
    {
        _audioManager.PlayClip(buttonSelected);
        SceneManager.LoadScene(1);
    }

    public void StartNormalMode()
    {
        _audioManager.PlayClip(buttonSelected);
        SetPlayerPrefsBool("isPlayingNormalMode", true);
        SceneManager.LoadScene(1);
    }

    public void StartOneBoostChallengeMode()
    {
        _audioManager.PlayClip(buttonSelected);
        SetPlayerPrefsBool("isPlayingNormalMode", false);
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        _audioManager.PlayClip(buttonSelected);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        _audioManager.PlayClip(buttonSelected);
        Application.Quit();
    }

    public void SetPauseMenuState(bool isPaused)
    {
        _audioManager.PlayClip(buttonSelected);
        PauseGameInterface.SetActive(isPaused);
    }

    public void UnpauseGame()
    {
        SetPauseMenuState(false);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Spaceship>().UnpauseGame();
    }

    public void SetPlayerPrefsBool(string fileName, bool value)
    {
        if (value == true)
        {
            PlayerPrefs.SetInt(fileName, 1);
        }
        else
        {
            PlayerPrefs.SetInt(fileName, 0);
        }
    }

    public bool GetPlayerPrefsBool(string fileName)
    {
        if (!PlayerPrefs.HasKey(fileName)) { return false; }
        
        int value = PlayerPrefs.GetInt(fileName);
        if (value == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
