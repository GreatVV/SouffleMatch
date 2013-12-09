using System.Linq;

public class HorizontalLineChuzzle : Chuzzle
{             
    protected override void OnAwake()
    {
        
    }

    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {   
        base.Destroy(needCreateNew, withAnimation);
        var horizontal = Gamefield.Chuzzles.Where(x => x.Current.y == Current.y && !GamefieldUtility.IsPowerUp(x)).ToArray();
        foreach (var chuzzle in horizontal)
        {
            chuzzle.Destroy(true);
        }
    }
}