#region

using System;

#endregion

[Serializable]
public abstract class GamefieldState
{      
    protected GamefieldState(Gamefield gamefield)
    {
        Gamefield = gamefield;
    }

    public Gamefield Gamefield { get; private set; }

    #region Event Handlers

    public abstract void OnEnter();
    public abstract void OnExit();

    #endregion

    public abstract void Update();
    public abstract void LateUpdate();
}