#region

using System;
using System.Collections.Generic;
using Game;
using Game.Data;
using Game.GameMode;
using GamefieldStates;
using UnityEngine;

#endregion

public class Gamefield : MonoBehaviour
{
    public LayerMask ChuzzleMask;

    public GameMode GameMode;
    public DateTime GameStartTime;

    public Level Level = new Level();
    public Points PointSystem;

    #region State

    [HideInInspector] public CheckSpecialState CheckSpecialState = null;
    [HideInInspector] public CreateNewChuzzlesState CreateNewChuzzlesState = null;
    [HideInInspector] public FieldState FieldState = null;
    [HideInInspector] public GameOverState GameOverState = null;
    [HideInInspector] public RemoveCombinationState RemoveState = null;
    [HideInInspector] public WinState WinState = null;

    private GamefieldState _currentState;
    private bool _isPause;

    #endregion

    public string LevelName
    {
        get { return string.Format(Localization.Get("LevelNumber"), LevelDescription.Name); }
    }

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

    public LevelDescription LevelDescription { get; set; }

    #region Events

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action<Gamefield> GameStarted;

    public event Action<bool> Paused;

    public void AddEventHandlers()
    {
        RemoveEventHandlers();
        CombinationDestroyed += PointSystem.CountForCombinations;
        GameMode.Win += OnWin;
        GameMode.GameOver += OnGameOver;
    }

    private void RemoveEventHandlers()
    {
        CombinationDestroyed -= PointSystem.CountForCombinations;
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
        SwitchStateTo(GameOverState);
        Player.Instance.Lifes.SpentLife();
        RemoveEventHandlers();
    }

    private void OnWin()
    {
        SwitchStateTo(WinState);
        RemoveEventHandlers();
    }

    #endregion

    #region Event Invokators

    protected virtual void FirePaused()
    {
        var handler = Paused;
        if (handler != null) handler(IsPause);
    }

    public virtual void InvokeCombinationDestroyed(List<Chuzzle> combination)
    {
        Action<List<Chuzzle>> handler = CombinationDestroyed;
        if (handler != null) handler(combination);
    }


    public virtual void InvokeGameStarted()
    {
        Action<Gamefield> handler = GameStarted;
        if (handler != null) handler(this);
    }

    #endregion

    #region Unity Methods

    public void Start()
    {
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

    #endregion

    public void StartGame(LevelDescription levelDescription = null)
    {
        LevelDescription = Player.Instance.LastPlayedLevelDescription = levelDescription;

        GameStartTime = DateTime.UtcNow;
        Init();
    }


    public void SwitchStateTo(GamefieldState newState)
    {
        //Debug.Log("Old state: "+_currentState);
        if (_currentState != null)
        {
            _currentState.OnExit();
        }
        _currentState = newState;
         //Debug.Log("Switch to: " + _currentState);
        _currentState.OnEnter();
    }

    public void Init()
    {
        if (CheckSpecialState)
        {
            Destroy(CheckSpecialState);
        }
        CheckSpecialState = gameObject.AddComponent<CheckSpecialState>();

        if (CreateNewChuzzlesState)
        {
            Destroy(CreateNewChuzzlesState);
        }
        CreateNewChuzzlesState = gameObject.AddComponent<CreateNewChuzzlesState>();

        if (RemoveState)
        {
            Destroy(RemoveState);
        }
        RemoveState = gameObject.AddComponent<RemoveCombinationState>();

        if (GameOverState)
        {
            Destroy(GameOverState); 
        }
        GameOverState = gameObject.AddComponent<GameOverState>();

        if (WinState)
        {
            Destroy(WinState);
        }
        WinState = gameObject.AddComponent<WinState>();

        if (FieldState)
        {
            Destroy(FieldState);
        }

        FieldState = gameObject.AddComponent<FieldState>();

        GameMode = GameModeFactory.CreateGameMode(LevelDescription.Condition.GameMode);

        PointSystem.Reset();
        PointSystem.TargetPoints = GameMode.TargetPoints;
       
        GameMode.Init(this);

        Level.Gamefield = this;
        Level.InitFromFile(LevelDescription.Field);

        AddEventHandlers();
        InvokeGameStarted();
        CenterCameraOnField.Instance.CenterCameraOnChuzzles(Level.Chuzzles,true);
        
        SwitchStateTo(CheckSpecialState);
    }
}