﻿using System.Collections.Generic;

public class BombChuzzle : Chuzzle, IPowerUp
{      
    protected override void OnAwake()
    {
        
    }


    public IEnumerable<Chuzzle> ToDestroy
    {
        get
        {
            return new List<Chuzzle>();
            //return PowerUpDestroyManager.GetSquare(Current.x, Current.y);
        }
    }
}