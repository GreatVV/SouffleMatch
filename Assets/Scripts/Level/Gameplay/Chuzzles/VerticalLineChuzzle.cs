using System.Collections.Generic;
using System.Linq;

public class VerticalLineChuzzle : Chuzzle, IPowerUp
{
    protected override void OnAwake()
    {

    }

    public override void Destroy(bool needCreateNew, bool withAnimation = true, bool isReplacingOnDeath = false)
    {
        if (IsDead)
        {
            return;
        }

        base.Destroy(needCreateNew, withAnimation, isReplacingOnDeath);
        StartCoroutine(PowerUpDestroyManager.Instance.Destroy(this));
    }

    public IEnumerable<Chuzzle> ToDestroy
    {
        get { return PowerUpDestroyManager.GetColumn(Current.x); }
    }
}

