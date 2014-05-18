#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof (Level))]
[RequireComponent(typeof (StageManager))]
[RequireComponent(typeof (Points))]
[RequireComponent(typeof (CheckSpecialState))]
[RequireComponent(typeof (CreateNewChuzzlesState))]
[RequireComponent(typeof (GameOverState))]
[RequireComponent(typeof (WinState))]
[RequireComponent(typeof (FieldState))]
[RequireComponent(typeof (RemoveCombinationState))]
[RequireComponent(typeof (InitState))]
[RequireComponent(typeof (WinCheckSpecialState))]
[RequireComponent(typeof (WinCreateNewChuzzlesState))]
[RequireComponent(typeof (WinRemoveCombinationState))]
public class Gamefield : MonoBehaviour
{
    private static Gamefield Instance;
    public static bool InvaderWasDestroyed;
    public LayerMask ChuzzleMask;

    public GameMode GameMode;
    public DateTime GameStartTime;

    [HideInInspector] public Level Level;
    public int[] NewTilesInColumns;

    [HideInInspector] public Points PointSystem;

    [HideInInspector] public StageManager StageManager;

    #region State

    [HideInInspector] public CheckSpecialState CheckSpecialState = null;

    [HideInInspector] public CreateNewChuzzlesState CreateNewChuzzlesState = null;

    [HideInInspector] public FieldState FieldState = null;
    [HideInInspector] public GameOverState GameOverState = null;

    [HideInInspector] public InitState InitState = null;
    [HideInInspector] public RemoveCombinationState RemoveState = null;
    [HideInInspector] public WinCheckSpecialState WinCheckSpecialState = null;
    [HideInInspector] public WinCreateNewChuzzlesState WinCreateNewChuzzlesState = null;
    [HideInInspector] public WinRemoveCombinationState WinRemoveCombinationState = null;
    [HideInInspector] public WinState WinState = null;
    private GamefieldState _currentState;
    private bool _isPause;

    #endregion

    public bool IsPause
    {
        get { return _isPause; }
        set
        {
            if (_isPause != value)
            {
                _isPause = value;
                FirePaused();
            }
        }
    }

    public static List<Chuzzle> Chuzzles
    {
        get { return Instance.Level.ActiveChuzzles; }
    }

    #region Events

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action<Gamefield> GameStarted;

    public event Action<Chuzzle> TileDestroyed;

    public event Action<bool> Paused;

    public void AddEventHandlers()
    {
        RemoveEventHandlers();
        GameMode.Win += OnWin;
        GameMode.GameOver += OnGameOver;
    }

    private void RemoveEventHandlers()
    {
        if (GameMode != null)
        {
            GameMode.Win -= OnWin;
            GameMode.GameOver -= OnGameOver;
        }
    }

    #endregion

    #region Event Handlers

    private void OnGameOver()
    {
        CombinationDestroyed -= PointSystem.CountForCombinations;
        SwitchStateTo(GameOverState);
        Player.Instance.Lifes.SpentLife();
    }

    private void OnWin()
    {
        CombinationDestroyed -= PointSystem.CountForCombinations;
        SwitchStateTo(WinState);
    }

    #endregion

    #region Event Invokators

    protected virtual void FirePaused()
    {
        Action<bool> handler = Paused;
        if (handler != null) handler(IsPause);
    }

    public virtual void InvokeCombinationDestroyed(List<Chuzzle> combination)
    {
        Action<List<Chuzzle>> handler = CombinationDestroyed;
        if (handler != null) handler(combination);
    }

    public void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
    }

    public virtual void InvokeGameStarted()
    {
        Action<Gamefield> handler = GameStarted;
        if (handler != null) handler(this);
    }

    #endregion

    #region Unity Methods

    public void Awake()
    {
        Instance = this;
        InitState = GetComponent<InitState>();
        CheckSpecialState = GetComponent<CheckSpecialState>();
        CreateNewChuzzlesState = GetComponent<CreateNewChuzzlesState>();
        RemoveState = GetComponent<RemoveCombinationState>();
        GameOverState = GetComponent<GameOverState>();
        WinState = GetComponent<WinState>();
        FieldState = GetComponent<FieldState>();
        WinCheckSpecialState = GetComponent<WinCheckSpecialState>();
        WinCreateNewChuzzlesState = GetComponent<WinCreateNewChuzzlesState>();
        WinRemoveCombinationState = GetComponent<WinRemoveCombinationState>();


        Level = GetComponent<Level>();
        StageManager = GetComponent<StageManager>();
        PointSystem = GetComponent<Points>();

        if (!Application.isEditor)
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian:
                    Localization.language = "Russian";
                    break;
                default:
                    Localization.language = "English";
                    break;
            }
        }
        else
        {
            Localization.language = "Russian";
        }

        GA.API.Design.NewEvent("Game:Localization:" + Localization.language);
        GA.API.Design.NewEvent("Game:SystemLocalization:" + Application.systemLanguage);
    }

    private void LateUpdate()
    {
        if (_currentState != null && !IsPause)
        {
            _currentState.LateUpdateState();
        }
    }

    private void OnDestroy()
    {
        RemoveEventHandlers();
    }

    private void Update()
    {
        if (_currentState != null && !IsPause)
        {
            _currentState.UpdateState();
        }
    }

    public void Reset()
    {
        PointSystem.Reset();
        Level.Reset();

        CombinationDestroyed -= InvaderChuzzle.OnCombinationDestroyed;
        CombinationDestroyed += InvaderChuzzle.OnCombinationDestroyed;

        Level.InitFromFile(Player.Instance.LastPlayedLevel);
        StageManager.Init(Player.Instance.LastPlayedLevel.Stages);

        PointSystem.TargetPoints = GameMode.TargetPoints;

        NewTilesInColumns = new int[Level.Width];

        AddEventHandlers();
    }

    #endregion

    public void StartGame(SerializedLevel level = null)
    {
        Level.Reset();

        Player.Instance.LastPlayedLevel = level;
        CombinationDestroyed -= PointSystem.CountForCombinations;
        CombinationDestroyed += PointSystem.CountForCombinations;
        SwitchStateTo(InitState);

        GameStartTime = DateTime.UtcNow;
    }

    /// <summary>
    ///     Remove chuzzle from game logic and add new tiles in column
    /// </summary>
    /// <param name="chuzzle">Chuzzle to remove</param>
    /// <param name="invokeEvent">Need to invoke event or not</param>
    public void RemoveChuzzle(Chuzzle chuzzle, bool invokeEvent = true)
    {
        Level.Chuzzles.Remove(chuzzle);
        Level.ActiveChuzzles.Remove(chuzzle);

        if (chuzzle.NeedCreateNew)
        {
            if (chuzzle is TwoTimeChuzzle)
            {
                Debug.LogError("Error: Two time chuzzle creation!!");
            }
            NewTilesInColumns[chuzzle.Current.x]++;
        }
        if (invokeEvent)
        {
            InvokeTileDestroyed(chuzzle);
        }
    }

    public void SwitchStateTo(GamefieldState newState)
    {
        // Debug.Log("Old state: "+_currentState);
        if (_currentState != null)
        {
            _currentState.OnExit();
        }
        _currentState = newState;
        // Debug.Log("Switch to: " + _currentState);
        _currentState.OnEnter();
    }
}