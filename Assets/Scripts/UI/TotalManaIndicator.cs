using System.Globalization;
using Game;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class TotalManaIndicator : MonoBehaviour
{
    [SerializeField]
    private Text label;

	void Start ()
	{
        Instance.ProgressionManager.ManaChanged += OnMoneyChanged;
        OnMoneyChanged(Instance.ProgressionManager.Mana);
	}

    void OnDestroy()
    {
        Instance.ProgressionManager.ManaChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int money)
    {
        label.text = money.ToString(CultureInfo.InvariantCulture);
    }
}
