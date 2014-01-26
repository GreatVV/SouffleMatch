using System.Collections.Generic;
using System.Linq;

public class HorizontalLineChuzzle : Chuzzle, IPowerUp
{             
    protected override void OnAwake()
    {
        
    }

    public IEnumerable<Chuzzle> ToDestroy
    {
        get { return PowerUpDestroyManager.GetRow(Current.y); }
    }
}