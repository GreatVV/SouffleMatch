using UnityEngine;

public class TwoTimeChuzzle : Chuzzle
{
    public override void Destroy(bool needCreateNew, bool withAnimation = true, bool isReplacingOnDeath = false)
    {
        if (IsDead)
        {
            Debug.Log("Is Dead "+GetInstanceID());
            return;
        }
        NeedCreateNew = false;
        TilesFactory.Instance.CreateChuzzle(Current, Color);
        Die(false);           
    }

    protected override void OnAwake()
    {
        
    }
}