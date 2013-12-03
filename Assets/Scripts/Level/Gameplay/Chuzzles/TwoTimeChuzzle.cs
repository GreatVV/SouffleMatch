using System.Collections.Generic;
using UnityEngine;

public class TwoTimeChuzzle : Chuzzle
{
    public int TimesDestroyed;

    public SpriteRenderer TwoTimeSprite;

    private void Awake()
    {   
    }

    public override void Destroy(List<Chuzzle> combination)
    {
        TimesDestroyed++;

        if (TimesDestroyed >= 2)
        {
            Die();
        }
        else
        {   
            TwoTimeSprite.enabled = false;
            InvokeAnimationFinished();
        }
    }
}