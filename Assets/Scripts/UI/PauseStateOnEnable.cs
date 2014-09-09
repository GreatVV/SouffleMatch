using UnityEngine;
using System.Collections;

public class PauseStateOnEnable : MonoBehaviour {

    void OnEnable()
    {
        Gamefield.Instance.FieldState.IsWorking = false;
    }

    void OnDisable()
    {
        Gamefield.Instance.FieldState.IsWorking = true;
    }
}
