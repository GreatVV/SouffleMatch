#region

using System;
using System.Globalization;
using UnityEngine;

#endregion

public class GuiBuyLivesPopup : Window
{
    public static Phrase MaxLifeString = new Phrase("У вас {0} жизней.\n И это максимум",
        "BuyLivePopup_MaxLifes");

    public static Phrase LifeString = new Phrase("У вас {0} жизней.", "BuyLivePopup_Lifes");
    public static Phrase PlusOneInString = new Phrase("+1       in {0}", "BuyLivePopup_PlusOne");


    public UIButton RefillButton;
    public UILabel LifeLabel;


    public GameObject RegeneratingTime;
    public UILabel MaximumLabel;
    public UILabel PlusOneIn;

    public UILabel PriceLabel;

    #region Event Handlers

    public void OnAddLifeClick()
    {
        if (Player.Instance.Lifes.IsRegenerating)
        {
            if (Economy.Instance.Spent(Player.Instance.LifePrice)) 
            {
                Player.Instance.Lifes.AddLife();
                //UI.Instance.ShowMap();
            }
            else
            {
                UI.Instance.ShowInAppPopup(this);
            }
        }
    }

    public override void OnCloseButton()
    {
        Close();
    }

    private void OnDisable()
    {
        RemoveEventHandlers();
    }

    private void OnEnable()
    {
        AddEventHandlers();
        OnLifesChanged(Player.Instance.Lifes.Lifes);
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    private void OnLifesChanged(int lifes)
    {
        LifeLabel.text = string.Format("{0}", lifes);

        RegeneratingTime.SetActive(Player.Instance.Lifes.IsRegenerating);
        MaximumLabel.gameObject.SetActive(!Player.Instance.Lifes.IsRegenerating);

        PlusOneIn.text = LocalizationStrings.GetString(PlusOneInString, Player.Instance.Lifes.RegenarationTime);
        PriceLabel.text = Player.Instance.LifePrice.ToString(CultureInfo.InvariantCulture);

        RefillButton.isEnabled = Player.Instance.Lifes.IsRegenerating;
    }

    #endregion

    protected override void OnUpdate()
    {
        if (Player.Instance.Lifes.IsRegenerating && Player.Instance.Lifes.LifeSpentDate.HasValue)
        {
            var timespan = Player.Instance.Lifes.LifeSpentDate.Value + TimeSpan.FromSeconds(Player.Instance.Lifes.RegenarationTime) -
                           DateTime.UtcNow;
            if (timespan.TotalSeconds > 0)
            {
                PlusOneIn.text = LocalizationStrings.GetString(PlusOneInString, string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds)); ;
            }
        }
    }

    private void AddEventHandlers()
    {
        RemoveEventHandlers();
        Player.Instance.Lifes.LifesChanged += OnLifesChanged;
    }

    private void RemoveEventHandlers()
    {
        Player.Instance.Lifes.LifesChanged -= OnLifesChanged;
    }
}