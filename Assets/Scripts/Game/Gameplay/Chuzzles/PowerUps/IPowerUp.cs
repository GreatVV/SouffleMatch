using System.Collections.Generic;

namespace Assets.Game.Gameplay.Chuzzles.PowerUps
{
    public interface IPowerUp
    {
        IEnumerable<Chuzzle> ToDestroy { get; }
    }
}