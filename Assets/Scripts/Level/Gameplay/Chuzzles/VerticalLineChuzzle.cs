using System.Collections.Generic;
using System.Linq;

public class VerticalLineChuzzle : Chuzzle, IPowerUp
{
    public override void Destroy()
    {
        base.Destroy();
        Bang();
    }

    protected override void OnAwake()
    {
        
    }

    public void Bang(PreviousBang previous = PreviousBang.No, IPowerUp prevPowerUp = null)
    {
        switch (previous)
        {
            case PreviousBang.No:
            case PreviousBang.Horizontal:
            {
                var vertical = Gamefield.Chuzzles.Where(x => x.Current.x == Current.x && x.IsDiying == false);
                foreach (var chuzzle in vertical)
                {
                    chuzzle.Destroy();
                    if (GamefieldUtility.IsPowerUp(chuzzle))
                    {
                        ((IPowerUp) chuzzle).Bang(PreviousBang.Vertical, this);
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
                        ((IPowerUp) chuzzle).Bang(PreviousBang.Horizontal, this);
                    }
                }
                break;
            case PreviousBang.Bomb:
                var vertical3 = Gamefield.Chuzzles.Where(x => (x.Current.x == Current.x || x.Current.x == Current.x - 1 || x.Current.x == Current.x + 1) && x.IsDiying == false);
                foreach (var chuzzle in vertical3)
                {
                    chuzzle.Destroy();
                    if (GamefieldUtility.IsPowerUp(chuzzle))
                    {
                        ((IPowerUp) chuzzle).Bang(PreviousBang.Horizontal, this);
                    }
                }
                break;
        }
    }
}

public enum PreviousBang
{
    No,
    Horizontal,
    Vertical,
    Bomb
}

public interface IPowerUp
{
    void Bang(PreviousBang previous = PreviousBang.No, IPowerUp prevPowerUp = null);
}