#region

using System.Linq;

#endregion

public class BombChuzzle : Chuzzle
{
    #region Event Handlers

    protected override void OnAwake()
    {
    }

    #endregion

    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        base.Destroy(needCreateNew, withAnimation);

        var square =
            Gamefield.Chuzzles.Where(
                x =>
                    (x.Current.x == Current.x - 1 || x.Current.x == Current.x + 1 ||
                     x.Current.x == Current.x) &&
                    (x.Current.y == Current.y - 1 || x.Current.y == Current.y ||
                     x.Current.y == Current.y + 1) &&
                    x.IsDiying == false && !GamefieldUtility.IsPowerUp(x));

        UI.Instance.Gamefield.RemoveState.Combinations.Add(square.ToList());
    }
}