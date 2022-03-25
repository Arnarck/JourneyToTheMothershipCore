using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spaceship : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField] float thrust = 10f;
    [SerializeField] float initialThrust = 100f;
    [SerializeField] float angularThrust = 10f;

    [Header("Particles")]
    [SerializeField] ParticleSystem mainThruster = null;
    [SerializeField] ParticleSystem leftSideThruster = null;
    [SerializeField] ParticleSystem rightSideThruster = null;
    [SerializeField] ParticleSystem sparks = null;
    [SerializeField] ParticleSystem coreCollision = null;

    [Header("Audio Clips")]
    [SerializeField] AudioClip collision = null;
    [SerializeField] AudioClip coreDetonation = null;

    [Header("GameObjects")]
    [SerializeField] GameObject engineFailure = null;
    [SerializeField] GameObject wingTrails = null;

    Rigidbody _rigidBody;
    AudioManager _audioManager;
    SceneTransition _sceneTransition;
    UIManager _uiManager;

    bool _isTransitioning = true;
    bool _isCollisionsDisabled;
    bool _isPlayingNormalMode;
    bool _isGamePaused;
    bool _canApplyThrust;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMemberVariables();
        mainThruster.Play();

        if (SceneManager.GetActiveScene().buildIndex != 1) // if the current scene isn't level one
        {
            _rigidBody.AddRelativeForce(Vector3.forward * initialThrust);
        }
    }

    void InitializeMemberVariables()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _sceneTransition = GameObject.Find("Canvas").GetComponent<SceneTransition>();
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _isPlayingNormalMode = _uiManager.GetPlayerPrefsBool("isPlayingNormalMode");
    }

    // Update is called once per frame
    void Update()
    {
        RespondToKeyboardInputs();
        ProcessGameStatus();
    }

    void FixedUpdate()
    {
        if (!_isTransitioning && !_isGamePaused)
        {
            ProcessThrustInput();
        }
    }

    void RespondToKeyboardInputs()
    {
        if (_isTransitioning) { return; }

        RespondToPauseInput();

        if (_isGamePaused) { return; }

        RespondToAngularThrustInput();
        RespondToThrustInput();

        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
    }

    void RespondToPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            _isGamePaused = !_isGamePaused;
            _uiManager.SetPauseMenuState(_isGamePaused);
        }
    }

    void ProcessGameStatus()
    {
        if (_isGamePaused)
        {
            Time.timeScale = 0f;
            _audioManager.StopAllThrusters();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void RespondToThrustInput()
    {
        if (_isPlayingNormalMode)
        {
            RespondToNormalModeInput();
        }
        else
        {
            RespondToOneBoostChallengeInput();
        }
    }

    void RespondToNormalModeInput()
    {
        if (Input.GetKey(KeyCode.W)) // enable to thrust at the FixedUpdate
        {
            _canApplyThrust = true;
        }
        else
        {
            _canApplyThrust = false;
        }
    }

    void RespondToOneBoostChallengeInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _canApplyThrust = true;
        }
        else if (_canApplyThrust)
        {
            _isTransitioning = true;
            DisableMainThrusterEffects();
            DisableAngularThrustEffects();
            StartCoroutine(DelayToReloadLevel(1f));
        }
    }

    void DisableMainThrusterEffects()
    {
        _canApplyThrust = false;
        mainThruster.Stop();
        _audioManager.StopMainThruster();
    }

    void RespondToAngularThrustInput()
    {
        float rotationThisFrame = Time.deltaTime * angularThrust;
        if (Input.GetKey(KeyCode.A))
        {
            EnableAngularThrustEffects(leftSideThruster, rightSideThruster);
            transform.Rotate(Vector3.down * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            EnableAngularThrustEffects(rightSideThruster, leftSideThruster);
            transform.Rotate(Vector3.up * rotationThisFrame);
        }
        else
        {
            DisableAngularThrustEffects();
        }
    }

    void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _isCollisionsDisabled = !_isCollisionsDisabled;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            _isTransitioning = true;
            _sceneTransition.PrepareToLoadNextLevel();
        }
    }

    void EnableAngularThrustEffects(ParticleSystem activeSideThruster, ParticleSystem disabledSideThruster)
    {
        if (!activeSideThruster.isEmitting)
        {
            activeSideThruster.Play();
        }
        disabledSideThruster.Stop();
        _audioManager.PlaySideThruster();
    }

    void DisableAngularThrustEffects()
    {
        if (leftSideThruster.isEmitting)
        {
            leftSideThruster.Stop();
        }
        if (rightSideThruster.isEmitting)
        {
            rightSideThruster.Stop();
        }
        _audioManager.StopSideThruster();
    }

    void ProcessThrustInput()
    {
        if (_canApplyThrust)
        {
            _rigidBody.AddRelativeForce(Vector3.forward * thrust);
            EnableAccelerationEffects();
        }
        else
        {
            DisableAccelerationEffects();
        }
    }

    void EnableAccelerationEffects()
    {
        var main = mainThruster.main;
        var emission = mainThruster.emission;

        main.simulationSpeed = 5f;
        main.startSize = new ParticleSystem.MinMaxCurve(1f, 3f);
        emission.rateOverTime = 100;

        _audioManager.ChangeMainThrusterVolume(1);
    }

    void DisableAccelerationEffects()
    {
        var main = mainThruster.main;
        var emission = mainThruster.emission;

        main.simulationSpeed = 1f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
        emission.rateOverTime = 50;

        _audioManager.ChangeMainThrusterVolume(-1);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_isTransitioning || _isCollisionsDisabled) { return; } // Guard Condition. Prevent the code to do some behaviour twice and break the game

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;

            default:
                _isTransitioning = true;
                StartDeathSequence();
                PlayDeathEffects(collision.GetContact(0).point);
                StartCoroutine(DelayToReloadLevel(2f));
                break;
        }
    }

    void StartDeathSequence()
    {
        wingTrails.SetActive(false);
        DisableMainThrusterEffects();
        DisableAngularThrustEffects();

        _rigidBody.constraints = RigidbodyConstraints.None;
        _rigidBody.useGravity = true;
        _rigidBody.AddForce(Vector3.down * 20f, ForceMode.Impulse);
    }

    void PlayDeathEffects(Vector3 contactPoint)
    {
        sparks.transform.position = contactPoint;
        sparks.Play();
        engineFailure.SetActive(true);
        _audioManager.PlayClip(collision);
    }

    IEnumerator DelayToReloadLevel(float time)
    {
        yield return new WaitForSeconds(time);
        _sceneTransition.PrepareToReloadLevel();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_isTransitioning) { return; }

        switch (other.tag)
        {
            case "Finish":
                _isTransitioning = true;
                StartSuccessSequence();
                break;

            case "Core":
                _isTransitioning = true;
                StartCoreDetonationSequence(other.GetComponent<CoreBehaviour>());
                break;

            default:
                // do nothing
                break;
        }
    }

    void StartCoreDetonationSequence(CoreBehaviour core)
    {
        _audioManager.StopMainThruster();
        _audioManager.StopSideThruster();
        _audioManager.PlayClip(coreDetonation);
        coreCollision.transform.position = transform.position;
        coreCollision.Play();
        core.StartCoreDetonation();
    }

    void StartSuccessSequence()
    {
        _sceneTransition.PrepareToLoadNextLevel();
        _audioManager.StopMainThruster();
        _audioManager.StopSideThruster();
    }

    public void EnablePlayerControls()
    {
        _isTransitioning = false;
    }

    public void UnpauseGame()
    {
        _isGamePaused = false;
        Time.timeScale = 1f;
    }
}