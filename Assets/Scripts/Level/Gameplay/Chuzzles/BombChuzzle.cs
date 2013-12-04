using System.Linq;

public class BombChuzzle : Chuzzle, IPowerUp
{
    public override void Destroy()
    {
        base.Destroy();
        Bang();        
    }
    
    public void Bang(PreviousBang previous = PreviousBang.No, IPowerUp prevPowerUp = null)
    {
        switch (previous)
        {
            case PreviousBang.No:
            case PreviousBang.Bomb:
                {
                    var square =
                Gamefield.Chuzzles.Where(
                    x =>
                        (x.Current.x == Current.x - 1 || x.Current.x == Current.x + 1 ||
                         x.Current.x == Current.x) &&
                        (x.Current.y == Current.y - 1 || x.Current.y == Current.y ||
                         x.Current.y == Current.y + 1) &&
                        x.IsDiying == false);

                    foreach (var chuzzle in square)
                    {
                        chuzzle.Destroy();
                        if (GamefieldUtility.IsPowerUp(chuzzle))
                        {
                            ((IPowerUp)chuzzle).Bang(PreviousBang.Vertical, this);
                        }
                    }

                }
                break;
            case PreviousBang.Vertical:
                var horizontal = Gamefield.Chuzzles.Where(x => x.Current.y == Current.y && x.IsDiying == false);
                foreach (var chuzzle in horizontal)
                {
                    chuzzle.Destroy();
                    if (GamefieldUtility.IsPowerUp(chuzzle))
                    {
                        ((IPowerUp)chuzzle).Bang(PreviousBang.Horizontal, this);
                    }
                }
                break;
            case PreviousBang.Horizontal:
                var vertical3 = Gamefield.Chuzzles.Where(x => (x.Current.x == Current.x || x.Current.x == Current.x - 1 || x.Current.x == Current.x + 1) && x.IsDiying == false);
                foreach (var chuzzle in vertical3)
                {
                    chuzzle.Destroy();
                    if (GamefieldUtility.IsPowerUp(chuzzle))
                    {
                        ((IPowerUp)chuzzle).Bang(PreviousBang.Horizontal, this);
                    }
                }
                break;
        }
    }
}