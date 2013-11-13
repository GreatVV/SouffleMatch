#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class Gamefield : MonoBehaviour
{
   
    public LayerMask ChuzzleMask;

    #region State

    private GamefieldState _currentState = null;

    public CheckSpecialState CheckSpecial = null;
    public CreateNewState CreateNew = null;
    public GameOverState GameOverState = null;
    public WinState WinState = null;
    public Field FieldState = null;
    public RemoveCombinationState RemoveState = null;

    #endregion                                  

    public int[] NewTilesInColumns = new int[0];
    public GameMode GameMode = GameModeFactory.CreateGameMode(GameModeDescription.CreateFromJson(null));

    public SerializedLevel LastLoadedLevel = null;
    public Level Level = new Level();

    public Points PointSystem = new Points();

    public List<Pair> PowerTypePrefabs = new List<Pair>();

    public StageManager StageManager = new StageManager();
    public float TimeFromTip = 0;

    public bool IsPause;
    public GameObject DownArrow;

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action<Gamefield> GameStarted;

    public event Action<Chuzzle> TileDestroyed;


    private void OnGameOver()
    {
        SwitchStateTo(GameOverState);
        Player.Instance.Lifes.SpentLife();
    }

    private void OnWin()
    {
        SwitchStateTo(WinState);
    }

    public virtual void InvokeCombinationDestroyed(List<Chuzzle> combination)
    {
        var handler = CombinationDestroyed;
        if (handler != null) handler(combination);
    }

    public virtual void InvokeGameStarted()
    {
        var handler = GameStarted;
        if (handler != null) handler(this);
    }

    private void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
    }


    public void Awake()
    {
        CheckSpecial = new CheckSpecialState(this);
        CreateNew = new CreateNewState(this);
        RemoveState = new RemoveCombinationState(this);
        GameOverState = new GameOverState(this);
        WinState = new WinState(this);
        FieldState = new Field(this);
        //PauseState = new PauseState(this);
    }


    private void LateUpdate()
    {
        if (_currentState != null && !IsPause)
        {
            _currentState.LateUpdate();
        }
    }


    /// <summary>
    ///     Remove chuzzle from game logic and add new tiles in column
    /// </summary>
    /// <param name="chuzzle">Chuzzle to remove</param>
    /// <param name="invokeEvent">Need to invoke event or not</param>
    public void RemoveChuzzle(Chuzzle chuzzle, bool invokeEvent = true)
    {
        Level.Chuzzles.Remove(chuzzle);
        if (Level.ActiveChuzzles.Contains(chuzzle))
        {
            Level.ActiveChuzzles.Remove(chuzzle);
        }
        NewTilesInColumns[chuzzle.Current.x]++;
        if (invokeEvent)
        {
            InvokeTileDestroyed(chuzzle);
        }
    }

    public void StartGame(SerializedLevel level = null)
    {
        LastLoadedLevel = level;
        _currentState = new InitState(this);
        _currentState.OnEnter();
    }

    public void AddEventHandlers()
    {
        RemoveEventHandlers();
        GameMode.Win += OnWin;
        GameMode.GameOver += OnGameOver;
    }

    private void RemoveEventHandlers()
    {
        GameMode.Win -= OnWin;
        GameMode.GameOver -= OnGameOver;
    }

    private void Update()
    {
        if (_currentState != null && !IsPause)
        {
            _currentState.Update();
        }
    }

    public void SwitchStateTo(GamefieldState newState)
    {
        _currentState.OnExit();
        _currentState = newState;
        Debug.Log("Switch to: " + _currentState);
        _currentState.OnEnter();
    }
}