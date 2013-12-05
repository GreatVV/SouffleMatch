using System;
using System.Globalization;
using UnityEngine;

public class GuiLifes : MonoBehaviour
{
    public LifeSystem LifeSystem;

    public UILabel Amount;
    public UILabel Timer;

    #region Event Handlers

    private void OnAddButtonClick()
    {
        UI.Instance.BuyLivesPopup.Show();
    }

    private void OnDestroy()
    {
        LifeSystem.LifesChanged -= OnLifeChanged;
    }

    private void OnLifeChanged(int liveNumber)
    {
        Amount.text = liveNumber.ToString(CultureInfo.InvariantCulture);
        Timer.gameObject.SetActive(liveNumber < LifeSystem.MaxLifes);
    }

    #endregion

    private void Awake()
    {
        LifeSystem.LifesChanged += OnLifeChanged;
    }

    private void Update()
    {
        if (LifeSystem.IsRegenerating && LifeSystem.LifeSpentDate.HasValue)
        {
            var timespan = LifeSystem.LifeSpentDate.Value + TimeSpan.FromSeconds(LifeSystem.RegenarationTime) -
                           DateTime.UtcNow;
            if (timespan.TotalSeconds > 0)
            {
                Timer.text = string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds);
            }
        }
    }
}