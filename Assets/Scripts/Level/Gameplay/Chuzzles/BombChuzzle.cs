using System.Collections.Generic;

public class BombChuzzle : Chuzzle, IPowerUp
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
        PowerUpDestroyManager.Instance.Destroy(this);
    }


    public IEnumerable<Chuzzle> ToDestroy
    {
        get { return PowerUpDestroyManager.GetSquare(Current.x, Current.y); }
    }
}