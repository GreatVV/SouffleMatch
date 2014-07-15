using System.Globalization;
using Game;
using UnityEngine;

public class TotalManaIndicator : MonoBehaviour
{
    [SerializeField]
    private UILabel label;

	void Start ()
	{
	    ProgressionManager.Instance.ManaChanged += OnMoneyChanged;
        OnMoneyChanged(ProgressionManager.Instance.Mana);
	}

    void OnDestroy()
    {
        ProgressionManager.Instance.ManaChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int money)
    {
        label.text = money.ToString(CultureInfo.InvariantCulture);
    }
}
