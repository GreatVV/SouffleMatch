using System.Collections.Generic;
using System.Linq;

public class VerticalLineChuzzle : Chuzzle, IPowerUp
{
    protected override void OnAwake()
    {

    }

    public IEnumerable<Chuzzle> ToDestroy
    {
        get { return PowerUpDestroyManager.GetColumn(Current.x); }
    }
}

