using System.Collections.Generic;

namespace Game.Gameplay.Chuzzles.PowerUps
{
    public interface IPowerUp
    {
        IEnumerable<Chuzzle> ToDestroy { get; }
    }
}