using UnityEngine;
using Utils;

namespace UI
{
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
}
