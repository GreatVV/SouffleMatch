using UnityEngine;
using System.Collections;
using Utils;

public class PauseStateOnEnable : MonoBehaviour {

    void OnEnable()
    {
        Instance.Gamefield.FieldState.IsWorking = false;
    }

    void OnDisable()
    {
        Instance.Gamefield.FieldState.IsWorking = true;
    }
}
