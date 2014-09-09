using System;

namespace Game.GameMode
{
    [Serializable]
    public abstract class GameMode
    {
        public int Turns;
        public bool IsGameOver;
        public bool IsWin;
        public ManaManager ManaManagerSystem;

        public GameModeDescription Description;

        protected GameMode(GameModeDescription description)
        {
            Description = description;
            Turns = StartTurns = description.Turns;
        }

        public int StartTurns;
        public virtual int TargetPoints { get; protected set; }

        public abstract void HumanTurn();                              

        public virtual void Reset()
        {
            TargetPoints = 1;
            Turns = StartTurns;
            IsWin = IsGameOver = false;
            OnReset();
        }

        public abstract void OnReset();

        public event Action GameOver;
        public event Action Win;
        public event Action<int, int> TurnsChanged;

        public void InvokeWin()
        {
            if (Win != null)
            {
                Win();
            }
        }

        public void InvokeGameOver()
        {
            if (GameOver != null)
            {
                GameOver();
            }
        }

        public bool Check()
        {
            ProgressionManager.Instance.RegisterLevelFinish(SessionRestorer.Instance.lastPlayedPack, SessionRestorer.Instance.lastPlayedLevel, ManaManagerSystem.CurrentPoints, Turns, IsWin);
            if (IsGameOver)
            {
                InvokeGameOver();
                return true;
            }

            if (IsWin)
            {
                InvokeWin();
                return true;
            }

            return false;
        }

        public void SpendTurn()
        {
            Turns--;
            InvokeTurnsChanged();
            if (Turns == 0 && !IsWin)
            {
                IsGameOver = true;
            }
        }

        public virtual void OnDestroy()
        {
        
        }

        public void InvokeTurnsChanged()
        {
            if (TurnsChanged != null)
            {
                TurnsChanged(Turns, StartTurns);
            }
        }

        public void AddTurns(int additionalTurns)
        {
            Turns += additionalTurns;
            InvokeTurnsChanged();
        }
    
        public Gamefield Gamefield;

        public void Init(Gamefield gamefield)
        {   
            this.Gamefield = gamefield;
            ManaManagerSystem = Gamefield.ManaManagerSystem;
            Reset();
            OnDestroy();
            OnInit();
            InvokeTurnsChanged();
        }

        protected abstract void OnInit();
    }
}