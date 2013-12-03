#region

using System.Collections.Generic;
using System.Linq;

#endregion

public static class PowerUpBehaviour
{   
    public static void ApplyPowerUp(this Gamefield gamefield, List<Chuzzle> tilesToKill, Chuzzle chuzzle)
    {
        if (chuzzle is HorizontalLineChuzzle)
        {
            var horizontalChuzzles =
                gamefield.Level.ActiveChuzzles.Where(
                    x =>
                        x.Current.y == chuzzle.Current.y &&
                        !(x is VerticalLineChuzzle || x is HorizontalLineChuzzle || x is BombChuzzle));
            tilesToKill.AddUniqRange(horizontalChuzzles);
        }

        if (chuzzle is VerticalLineChuzzle)
        {
            var vertical =
                gamefield.Level.ActiveChuzzles.Where(
                    x =>
                        x.Current.x == chuzzle.Current.x &&
                        !(x is VerticalLineChuzzle || x is HorizontalLineChuzzle || x is BombChuzzle));
            tilesToKill.AddUniqRange(vertical);
        }

        if (chuzzle is BombChuzzle)
        {
            var square =
                gamefield.Level.ActiveChuzzles.Where(
                    x =>
                        (x.Current.x == chuzzle.Current.x - 1 || x.Current.x == chuzzle.Current.x + 1 ||
                         x.Current.x == chuzzle.Current.x) &&
                        (x.Current.y == chuzzle.Current.y - 1 || x.Current.y == chuzzle.Current.y ||
                         x.Current.y == chuzzle.Current.y + 1) &&
                        !(x is VerticalLineChuzzle || x is HorizontalLineChuzzle || x is BombChuzzle));
            tilesToKill.AddUniqRange(square);
        }
    }
}