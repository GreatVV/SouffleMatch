using Assets.Utils;
using UnityEngine;

namespace Assets.UI
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
