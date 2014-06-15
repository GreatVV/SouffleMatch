using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using System.Collections;

public class FlyingPoints : MonoBehaviour
{

    [SerializeField] private UILabel amountLabel;

    public void Init(int points)
    {
        amountLabel.text = points.ToString(CultureInfo.InvariantCulture);
    }
}