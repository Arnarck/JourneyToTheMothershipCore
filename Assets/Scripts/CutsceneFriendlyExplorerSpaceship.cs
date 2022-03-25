using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CutsceneFriendlyExplorerSpaceship : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField] float thrust = 20f;
    [SerializeField] float angularThrust = 300f;
    [SerializeField] float rotationAmount = 180f;
    [Range(-1, 1)][SerializeField] int rotationDirection = 1;

    [Header("Particles")]
    [SerializeField] ParticleSystem mainThruster = null;
    [SerializeField] ParticleSystem leftSideThruster = null;
    [SerializeField] ParticleSystem rightSideThruster = null;

    CutsceneSpaceship _spaceship;
    Rigidbody _rigidBody;
    AudioManager _audioManager;

    float _currentRotationAmount;
    float _finalRotation;

    bool _canRotateToGoAway;
    bool _canGoAway;

    // Start is called before the first frame update
    void Start()
    {
        _spaceship = GameObject.FindGameObjectWithTag("Player").GetComponent<CutsceneSpaceship>();
        _rigidBody = GetComponent<Rigidbody>();
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        mainThruster.Play();
        _currentRotationAmount = rotationAmount;
        _finalRotation = transform.eulerAngles.y + rotationAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (_canRotateToGoAway)
        {
            RotateSpaceship();
        }
    }

    private void RotateSpaceship()
    {
        float rotationThisFrame = Time.deltaTime * angularThrust;
        _currentRotationAmount -= rotationThisFrame;
        if (_currentRotationAmount < Mathf.Epsilon)
        {
            _canRotateToGoAway = false;
            _canGoAway = true;
            transform.eulerAngles = Vector3.up * _finalRotation;
            DisableAngularThrustEffects();
        }
        else
        {
            transform.Rotate(Vector3.up * rotationThisFrame * rotationDirection);
            DefineWhichAngularThrusterWillBeEnable();
        }
    }

    void DefineWhichAngularThrusterWillBeEnable()
    {
        if (rotationDirection >= 0)
        {
            EnableAngularThrustEffects(rightSideThruster, leftSideThruster);
        }
        else
        {
            EnableAngularThrustEffects(leftSideThruster, rightSideThruster);
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

    void FixedUpdate() // put an "isTransitioning" here
    {
        if (_canGoAway)
        {
            EnableAccelerationEffects();
            _rigidBody.AddRelativeForce(Vector3.forward * thrust);
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
    }

    void DisableAccelerationEffects()
    {
        var main = mainThruster.main;
        var emission = mainThruster.emission;

        main.simulationSpeed = 1f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
        emission.rateOverTime = 50;
    }

    public void PrepareDeparture()
    {
        StartCoroutine(GetReadyToLeave());
    }

    IEnumerator GetReadyToLeave()
    {
        yield return new WaitForSeconds(1f);
        _canRotateToGoAway = true;
        yield return new WaitForSeconds(1f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<CutsceneSpaceship>().CanLeave();
    }
}
