﻿using System.Collections.Generic;

public class HorizontalLineChuzzle : Chuzzle, IPowerUp
{
    #region IPowerUp Members

    public IEnumerable<Chuzzle> ToDestroy
    {
        get
        {
            //return PowerUpDestroyManager.GetRow(Current.y);
            return new List<Chuzzle>();
        }
    }

    #endregion

    #region Events Subscribers

    protected override void OnAwake()
    {
    }

    #endregion
}