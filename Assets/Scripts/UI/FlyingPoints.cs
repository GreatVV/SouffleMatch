using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI
{
    public class FlyingPoints : MonoBehaviour
    {
        [SerializeField] private Text amountLabel;

        public void Init(int points)
        {
            amountLabel.text = points.ToString(CultureInfo.InvariantCulture);
        }
    }
}