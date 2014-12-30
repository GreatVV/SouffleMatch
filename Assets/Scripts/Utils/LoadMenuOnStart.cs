using UI;
using UnityEngine;

namespace Utils
{
    public class LoadMenuOnStart : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            Application.LoadLevel(ScenesName.Menu);
        }
    }
}