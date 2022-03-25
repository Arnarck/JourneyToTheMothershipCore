using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Image panel = null;
    [SerializeField] float fadeIntensity = 10f;

    int _nextLevel;
    int _fadeSense;

    float _newAlphaValue;

    bool _isFading;

    // Start is called before the first frame update
    void Start()
    {
        _isFading = true;
        _fadeSense = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFading)
        {
            ChangeAlphaChannelValue();
            ProcessAlphaChannelValue();
        }
    }

    void ChangeAlphaChannelValue()
    {
        float fadeIntensityThisFrame = Time.deltaTime * fadeIntensity * _fadeSense;
        _newAlphaValue = panel.color.a + fadeIntensityThisFrame;
        panel.color = new Color(0f, 0f, 0f, Mathf.Clamp(_newAlphaValue, 0f, 1f) );
    }

    void ProcessAlphaChannelValue()
    {
        if (_newAlphaValue < Mathf.Epsilon) // less or equals zero
        {
            ActivateLevel();
        }
        else if (_newAlphaValue > 1f)
        {
            LoadNextLevel();
        }
    }

    void ActivateLevel()
    {
        _isFading = false;
        panel.gameObject.SetActive(false);

        Spaceship spaceship = GameObject.FindGameObjectWithTag("Player").GetComponent<Spaceship>();
        if (spaceship)
        {
            spaceship.EnablePlayerControls();
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(_nextLevel);
    }

    public void PrepareToReloadLevel()
    {
        PrepareLevelTransition();
        _nextLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public void PrepareToLoadNextLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex + 1;
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (nextLevelIndex <= lastLevelIndex)
        {
            _nextLevel = nextLevelIndex;
        }
        PrepareLevelTransition();
    }

    void PrepareLevelTransition()
    {
        panel.gameObject.SetActive(true);
        _isFading = true;
        _fadeSense = 1;
    }
}