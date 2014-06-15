using System.Globalization;
using Game;
using UnityEngine;

public class TotalManaIndicator : MonoBehaviour
{
    [SerializeField]
    private UILabel label;

	void Start ()
	{
	    Economy.Instance.MoneyChanged += OnMoneyChanged;
	}

    void OnDestroy()
    {
        if (Economy.Instance)
        {
            Economy.Instance.MoneyChanged -= OnMoneyChanged;
        }
    }

    private void OnMoneyChanged(int money)
    {
        label.text = money.ToString(CultureInfo.InvariantCulture);
    }
}
