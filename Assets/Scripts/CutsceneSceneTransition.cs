using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneSceneTransition : MonoBehaviour
{
    [SerializeField] GameObject endGameMenu = null;
    [SerializeField] Image panel = null;
    [SerializeField] float fadeIntensity = 10f;

    bool _isFading;

    float _newAlphaValue;

    int _fadeSense;

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
        panel.color = new Color(0f, 0f, 0f, Mathf.Clamp(_newAlphaValue, 0f, 1f));
    }

    void ProcessAlphaChannelValue()
    {
        if (_newAlphaValue < Mathf.Epsilon) // less or equals zero
        {
            ActivateCutscene();
        }
        else if (_newAlphaValue > 1f)
        {
            FinishTheGame();
        }
    }

    void ActivateCutscene()
    {
        panel.gameObject.SetActive(false);
        _isFading = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CutsceneSpaceship>().CanLeave();
    }

    void FinishTheGame()
    {
        _isFading = false;
        Time.timeScale = 0f;
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().StopAllThrusters();
        endGameMenu.SetActive(true);
    }

    public void PrepareToEndCutscene()
    {
        _isFading = true;
        _fadeSense = 1;
        panel.gameObject.SetActive(true);
    }
}
