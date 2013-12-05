using System.Collections.Generic;
using UnityEngine;

public class TwoTimeChuzzle : Chuzzle
{
    public int TimesDestroyed;

    public SpriteRenderer TwoTimeSprite;

    public override void Destroy()
    {
        TimesDestroyed++;

        if (TimesDestroyed >= 2)
        {
            Die();
        }
        else
        {   
            TwoTimeSprite.enabled = false;
        }
    }

    protected override void OnAwake()
    {
        
    }
}