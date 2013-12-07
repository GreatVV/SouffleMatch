#region

using System.Linq;

#endregion

public class VerticalLineChuzzle : Chuzzle
{
    #region Event Handlers

    protected override void OnAwake()
    {
    }

    #endregion

    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        base.Destroy(needCreateNew, withAnimation);
        var vertical =
            Gamefield.Chuzzles.Where(
                x => x.Current.x == Current.x && x.IsDiying == false && !GamefieldUtility.IsPowerUp(x));
        UI.Instance.Gamefield.RemoveState.Combinations.Add(vertical.ToList());
    }
}