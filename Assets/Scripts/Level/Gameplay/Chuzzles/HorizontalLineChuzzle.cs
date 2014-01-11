using System.Linq;

public class HorizontalLineChuzzle : Chuzzle
{             
    protected override void OnAwake()
    {
        
    }

    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        if (IsDead)
        {
            return;
        }

        base.Destroy(needCreateNew, withAnimation);
        var horizontal = Gamefield.Chuzzles.Where(x => x.Current.y == Current.y).ToArray();
        foreach (var chuzzle in horizontal)
        {
            chuzzle.Destroy(true);
        }
    }
}