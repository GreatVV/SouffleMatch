using UnityEngine;

public class CounterChuzzle : Chuzzle
{   
    public int Counter;

    public TextMesh TextMesh;
  
    public override void Destroy()
    {
        if (Counter <= 0)
        {   
            Die();
        }
    }

    protected override void OnAwake()
    {
        
    }

    protected override void Die()
    {
        Debug.LogWarning("Counter is dead");
        base.Die();
    }
}