#region

using UnityEngine;

#endregion

public class GuiBuyLivesPopup : Window
{
    public static Phrase MaxLifeString = new Phrase("У вас {0} жизней.\n И это максимум",
        "BuyLivePopup_MaxLifes");

    public static Phrase LifeString = new Phrase("У вас {0} жизней.", "BuyLivePopup_Lifes");
    public GameObject AddLifeButton;
    public UILabel LifeLabel;

    #region Event Handlers

    public void OnAddLifeClick()
    {
        if (Player.Instance.Lifes.IsRegenerating)
        {
            if (Economy.Instance.Spent(100))
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

    public void OnCloseButton()
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
        AddLifeButton.SetActive(Player.Instance.Lifes.IsRegenerating);
        OnLifesChanged(Player.Instance.Lifes.Lifes);
        transform.localPosition = new Vector3(0, -800, -5);
        iTween.MoveTo(gameObject, new Vector3(0, 0, -0.01f), 0.5f);
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");
        iTween.MoveTo(gameObject,
            iTween.Hash("x", 0, "y", 2, "z", -0.01f, "time", 0.5f,
                "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    private void OnLifesChanged(int lifes)
    {
        if (!Player.Instance.Lifes.IsRegenerating)
        {
            LifeLabel.text = LocalizationStrings.GetString(MaxLifeString, lifes);
        }
        else
        {
            LifeLabel.text = LocalizationStrings.GetString(LifeString, lifes);
        }
        AddLifeButton.SetActive(Player.Instance.Lifes.IsRegenerating);
    }

    #endregion

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