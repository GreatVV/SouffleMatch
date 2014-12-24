#region

using System;
using System.Collections.Generic;
using Game.Data;
using Game.GameMode;
using Game.Gameplay.Chuzzles;
using Game.Gameplay.GamefieldStates;
using Game.Player;
using Game.Visual;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

#endregion

namespace Game.Gameplay
{
    [ExecuteInEditMode]
    public class Gamefield : MonoBehaviour
    {
        public LayerMask ChuzzleMask;

        public GameMode.GameMode GameMode;
        public DateTime GameStartTime;

        public Level Level = new Level();
        [SerializeField]
        public ManaManager ManaManagerSystem;

        #region State
        [NonSerialized]
        public PowerUpAnalyzeState PowerUpAnalyzeState = null;
        [NonSerialized]
        public CreateState CreateState = null;
        [NonSerialized]
        public FieldState FieldState = null;
        [NonSerialized]
        public GameOverState GameOverState = null;
        [NonSerialized]
        public DeleteState RemoveState = null;
        [NonSerialized]
        public WinState WinState = null;

        [SerializeField]
        private string _currentStateName;
        
        private GameState _currentState = null;
        private bool _isPause;


        public GameState CurrentState
        {
            get { return _currentState; }
        }
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

        public LevelDescription LevelDescription { get; set; }

        #region Events

        public event Action<List<Chuzzle>> CombinationDestroyed;

        public event Action<Gamefield> GameStarted;

        public event Action<bool> Paused;

        public event Action<GameState, GameState> StateChanged;

        protected virtual void FireStateChanged(GameState oldState, GameState newState)
        {
            var handler = StateChanged;
            if (handler != null)
            {
                handler(oldState, newState);
            }
        }

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
            Instance.ProgressionManager.Init();
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

        public void StartGame(LevelDescription levelDescription)
        {
            LevelDescription = Instance.Player.LastPlayedLevelDescription = levelDescription;
            Debug.Log("Start level: "+LevelDescription);
            GameStartTime = DateTime.UtcNow;
            Init();
        }


        public void SwitchStateTo(GameState newState)
        {
            var prevState = _currentState;
            //Debug.Log("Old state: "+_currentState);
            if (_currentState != null)
            {
                _currentState.OnExit();
            }
            _currentState = newState;
            _currentStateName = newState.ToString();
            //Debug.Log("Switch to: " + _currentState);
            _currentState.OnEnter();

            FireStateChanged(prevState, newState);
        }

        public void Init()
        {
            //Debug.Log("Init");
            _currentState = null;
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

        public void OnDrag(BaseEventData eventData)
        {
            var pointerEventData = eventData as PointerEventData;
            if (pointerEventData != null)
            {
                FieldState.OnDrag(pointerEventData.delta);
            }
        }

        public void OnPointerDown(Chuzzle chuzzle)
        {
            FieldState.OnPointerDown(chuzzle);
        }

        public void OnPointerUp(Chuzzle chuzzle)
        {
            FieldState.OnPointerUp(chuzzle);
        }
    }
}