using System.Collections.Generic;
using UnityEngine;

public class CounterChuzzle : Chuzzle
{
    private int _counter;
    public int Counter;

    public TextMesh TextMesh;
    void Awake()
    {     
    }

    public override void Destroy(List<Chuzzle> combination)
    {
        if (Counter > 0)
        {
            InvokeAnimationFinished();
        }
        else
        {
            Die();
        }
    }
}