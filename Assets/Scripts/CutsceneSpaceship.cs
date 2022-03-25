using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneSpaceship : MonoBehaviour
{
    [SerializeField] float thrust = 60f;

    [SerializeField] GameObject malfunction = null;
    [SerializeField] GameObject engineFailures = null;

    [SerializeField] ParticleSystem mainThruster = null;
    [SerializeField] ParticleSystem healingEffect = null;

    [SerializeField] AudioClip healing = null;

    AudioManager _audioManager;
    Rigidbody _rigidBody;
    bool _canMove;
    bool _isTransitioning;

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _rigidBody = GetComponent<Rigidbody>();

        mainThruster.Play();
    }

    void FixedUpdate()
    {
        ProcessMovementValue();
    }

    void ProcessMovementValue()
    {
        if (_isTransitioning) { return; }

        if (_canMove)
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

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Heal":
                Destroy(other.gameObject);
                StartHealingSequence();
                ContactSpacechipsToLeave();
                break;

            case "Finish":
                StartFinishSequence();
                break;

            default:
                // do nothing
                break;
        }
    }

    void StartHealingSequence()
    {
        _audioManager.PlayClip(healing);
        _canMove = false;
        _rigidBody.velocity = Vector3.zero;
        healingEffect.transform.position = transform.position;
        healingEffect.Play();
        malfunction.SetActive(false);
        engineFailures.SetActive(false);
    }

    void ContactSpacechipsToLeave()
    {
        GameObject[] friendlyExplorers = GameObject.FindGameObjectsWithTag("Friendly");
        foreach (GameObject explorer in friendlyExplorers)
        {
            explorer.GetComponent<CutsceneFriendlyExplorerSpaceship>().PrepareDeparture();
        }
    }

    void StartFinishSequence()
    {
        _isTransitioning = true;
        _canMove = false;
        _audioManager.StopMainThruster();
        GameObject.Find("Canvas (End Cutscene)").GetComponent<CutsceneSceneTransition>().PrepareToEndCutscene();
    }

    public void CanLeave() // change the name of this method
    {
        _canMove = true;
    }
}
