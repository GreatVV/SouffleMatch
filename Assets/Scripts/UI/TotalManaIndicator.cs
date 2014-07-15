using System.Globalization;
using Game;
using UnityEngine;

public class TotalManaIndicator : MonoBehaviour
{
    [SerializeField]
    private UILabel label;

	void Start ()
	{
	    ProgressionManager.ManaChanged += OnMoneyChanged;
        OnMoneyChanged(ProgressionManager.Mana);
	}

    void OnDestroy()
    {
        ProgressionManager.ManaChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int money)
    {
        label.text = money.ToString(CultureInfo.InvariantCulture);
    }
}
