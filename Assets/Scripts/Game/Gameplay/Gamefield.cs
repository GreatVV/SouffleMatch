#region

using System;
using System.Collections.Generic;
using Game;
using Game.Data;
using Game.GameMode;
using GamefieldStates;
using UnityEngine;

#endregion

[ExecuteInEditMode]
public class Gamefield : MonoBehaviour
{
    public LayerMask ChuzzleMask;

    public GameMode GameMode;
    public DateTime GameStartTime;

    public Level Level = new Level();
    [SerializeField]
    public ManaManager ManaManagerSystem;

    #region State

    public PowerUpAnalyzeState PowerUpAnalyzeState = null;
    public CreateState CreateState = null;
    public FieldState FieldState = null;
    public GameOverState GameOverState = null;
    public DeleteState RemoveState = null;
    public WinState WinState = null;

    private GameState _currentState;
    private bool _isPause;
    public static Gamefield Instance;


    public GameState CurrentState
    {
        get { return _currentState; }
    }
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
        CombinationDestroyed += ManaManagerSystem.CountForCombinations;
        GameMode.Win += OnWin;
        GameMode.GameOver += OnGameOver;
    }

    private void RemoveEventHandlers()
    {
        CombinationDestroyed -= ManaManagerSystem.CountForCombinations;
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

        if (ProgressionManager.Instance)
        {
            ProgressionManager.Instance.Init();
        }
        else
        {
            Debug.LogWarning("Progress manager is null");
        }
    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        if (Instance == this)
        {
            Instance = null;
        }
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
        Debug.Log("Start level: "+LevelDescription);
        GameStartTime = DateTime.UtcNow;
        Init();
    }


    public void SwitchStateTo(GameState newState)
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
        //Debug.Log("Init");
        
        ChuzzleMover.Instance = new ChuzzleMover();

        PowerUpAnalyzeState = new PowerUpAnalyzeState(this);
        CreateState = new CreateState(this);
        RemoveState = new DeleteState(this);
        GameOverState = new GameOverState(this);
        WinState = new WinState(this);
        FieldState = new FieldState(this);
        GameMode = GameModeFactory.CreateGameMode(LevelDescription.Condition.GameMode);
        if (!ManaManagerSystem)
        {
            ManaManagerSystem = FindObjectOfType<ManaManager>();
        }
        ManaManagerSystem.Reset();
        ManaManagerSystem.TargetPoints = GameMode.TargetPoints;
        GameMode.Init(this);

        Level.Gamefield = this;
        Level.InitFromFile(LevelDescription.Field);
        //Level.Gamefield.transform.localScale = Vector3.zero;
        //iTween.ScaleTo(Level.Gamefield.gameObject, Vector3.one, 0.5f);

        AddEventHandlers();
        InvokeGameStarted();

        if (CenterCameraOnField.Instance)
        {
            CenterCameraOnField.Instance.CenterCameraOnChuzzles(Level.Chuzzles,true);
        }
        
        SwitchStateTo(PowerUpAnalyzeState);
    }
}