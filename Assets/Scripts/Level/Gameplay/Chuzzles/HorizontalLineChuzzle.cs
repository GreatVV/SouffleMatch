#region

using System.Linq;

#endregion

public class HorizontalLineChuzzle : Chuzzle
{
    #region Event Handlers

    protected override void OnAwake()
    {
    }

    #endregion

    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        base.Destroy(needCreateNew, withAnimation);
        var horizontal =
            Gamefield.Chuzzles.Where(
                x => x.Current.y == Current.y && x.IsDiying == false && !GamefieldUtility.IsPowerUp(x));
        UI.Instance.Gamefield.RemoveState.Combinations.Add(horizontal.ToList());
    }
}