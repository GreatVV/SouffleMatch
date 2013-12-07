using System.Collections.Generic;
using UnityEngine;

public class TwoTimeChuzzle : Chuzzle
{
    public int TimesDestroyed;

    public SpriteRenderer TwoTimeSprite;

    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        NeedCreateNew = needCreateNew;
        TimesDestroyed++;

        if (TimesDestroyed >= 2)
        {
            Die(withAnimation);
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