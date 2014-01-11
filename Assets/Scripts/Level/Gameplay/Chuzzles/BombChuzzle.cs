using System.Linq;

public class BombChuzzle : Chuzzle
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

        var square =
    Gamefield.Chuzzles.Where(
        x =>
            (x.Current.x == Current.x - 1 || x.Current.x == Current.x + 1 ||
             x.Current.x == Current.x) &&
            (x.Current.y == Current.y - 1 || x.Current.y == Current.y ||
             x.Current.y == Current.y + 1)).ToArray();
        foreach (var chuzzle in square)
        {
            chuzzle.Destroy(true);
        }
    }
}