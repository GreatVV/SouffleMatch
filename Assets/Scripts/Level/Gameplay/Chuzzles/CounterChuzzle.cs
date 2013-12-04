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

    protected override void Die()
    {
        Debug.LogError("Counter is dead");
        base.Die();
    }
}