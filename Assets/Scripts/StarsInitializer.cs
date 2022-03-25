using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsInitializer : MonoBehaviour
{
    [SerializeField] float timeAtMaxSpeed = 0.03f;
    ParticleSystem.MainModule emission;
    float particleSimulationSpeed;

    private void Awake()
    {
        emission = GetComponent<ParticleSystem>().main;
        particleSimulationSpeed = emission.simulationSpeed;
        emission.simulationSpeed = 100f;
        StartCoroutine(Algo());
    }

    IEnumerator Algo()
    {
        yield return new WaitForSeconds(timeAtMaxSpeed);
        emission.simulationSpeed = particleSimulationSpeed;

    }
}
