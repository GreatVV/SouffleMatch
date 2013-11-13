using System;
using UnityEngine;

public class GuiLifes : MonoBehaviour
{
    public LifeSystem LifeSystem;

    public UISprite[] LifeSprites;
    public UILabel TimerLabel;

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
        for (var i = 0; i < LifeSprites.Length; i++)
        {
            LifeSprites[i].color = i < liveNumber ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.3f);
        }

        TimerLabel.gameObject.SetActive(liveNumber < LifeSystem.MaxLifes);
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
                TimerLabel.text = string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds);
            }
        }
    }
}