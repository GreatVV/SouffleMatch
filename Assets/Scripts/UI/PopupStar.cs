#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class PopupStar : MonoBehaviour
{
    public Image Active;
    public Image InActive;

    public bool IsActive;

    public float AnimationTime = 0.5f;

    public void Show(bool isStar, bool withAnimation = false)
    {
        Active.gameObject.SetActive(isStar);
        InActive.gameObject.SetActive(!isStar);
        IsActive = isStar;

        if (IsActive && withAnimation)
        {
            var scale = Active.transform.localScale;
            Active.transform.localScale = scale*10;
            iTween.ScaleTo(Active.gameObject, scale, AnimationTime);
        }
    }
}