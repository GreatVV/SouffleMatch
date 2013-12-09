using System.Collections.Generic;
using System.Linq;

public class VerticalLineChuzzle : Chuzzle
{
    protected override void OnAwake()
    {
        
    }

    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        base.Destroy(needCreateNew, withAnimation);
        var vertical = Gamefield.Chuzzles.Where(x => x.Current.x == Current.x && !GamefieldUtility.IsPowerUp(x)).ToArray();
        foreach (var chuzzle in vertical)
        {
            chuzzle.Destroy(true);
        }
    }
}

