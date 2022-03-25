using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreBehaviour : MonoBehaviour
{
    [SerializeField] float detonationForce = 10f;
    [SerializeField] float detonationLimitScale = 400f;

    SceneTransition _sceneTransition;

    bool _canSpreadDetonation;

    void Start()
    {
        _sceneTransition = GameObject.Find("Canvas").GetComponent<SceneTransition>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canSpreadDetonation)
        {
            SpreadDetonation();
        }
    }

    void SpreadDetonation()
    {
        float spreadThisFrame = Time.deltaTime * detonationForce;
        transform.localScale += Vector3.one * spreadThisFrame;

        if (transform.localScale.x > detonationLimitScale)
        {
            transform.localScale = Vector3.one * detonationLimitScale;
            _canSpreadDetonation = false;
            _sceneTransition.PrepareToLoadNextLevel();
        }

    }

    public void StartCoreDetonation()
    {
        _canSpreadDetonation = true;
    }
}
