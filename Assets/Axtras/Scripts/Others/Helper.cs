using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
        for (int i = 0; i < steps; i++) {
            Physics.simulationMode = SimulationMode.Script;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
    #endregion

    #region UI Related
    public bool IsPointerOverUIElement() {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new(EventSystem.current) {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new ();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    #endregion
}