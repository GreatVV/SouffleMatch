using System.Collections.Generic;

public interface IPowerUp
{
    IEnumerable<Chuzzle> ToDestroy { get; }
}