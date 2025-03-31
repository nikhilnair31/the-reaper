using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

public class Helper : MonoBehaviour 
{
    public static Helper Instance { get; private set; }
    
    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #region Physics Related
    public void SimulatePhysicsSteps(int steps = 100) {
        for (int i = 0; i < 100; i++) {
            Physics.simulationMode = SimulationMode.Script;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
    #endregion
}