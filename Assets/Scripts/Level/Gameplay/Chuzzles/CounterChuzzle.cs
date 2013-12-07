using UnityEngine;

public class CounterChuzzle : Chuzzle
{   
    public int Counter;

    public TextMesh TextMesh;
  
    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        NeedCreateNew = needCreateNew;
        if (Counter <= 0)
        {   
            Die(withAnimation);
        }
    }

    protected override void OnAwake()
    {
        
    }

    protected override void Die(bool withAnimation)
    {
        Debug.LogWarning("Counter is dead");
        base.Die(withAnimation);
    }
}