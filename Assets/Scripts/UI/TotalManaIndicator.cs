using System.Globalization;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class TotalManaIndicator : MonoBehaviour
{
    [SerializeField]
    private Text label;

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
