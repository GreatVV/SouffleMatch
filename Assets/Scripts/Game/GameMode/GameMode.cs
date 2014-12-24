using System;
using Game.Gameplay;
using Game.Player;

namespace Game.GameMode
{
    [Serializable]
    public abstract class GameMode
    {
        public GameModeDescription Description;
        public Gamefield Gamefield;
        public bool IsGameOver;
        public bool IsWin;
        public ManaManager ManaManagerSystem;

        public int StartTurns;
        public int Turns;

        #region Events

        public event Action GameOver;
        public event Action<int, int> TurnsChanged;
        public event Action Win;

        #endregion

        protected GameMode(GameModeDescription description)
        {
            Description = description;
            Turns = StartTurns = description.Turns;
        }

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
        {}

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

        public void Init(Gamefield gamefield)
        {
            Gamefield = gamefield;
            ManaManagerSystem = Gamefield.ManaManagerSystem;
            Reset();
            OnDestroy();
            OnInit();
            InvokeTurnsChanged();
        }

        protected abstract void OnInit();
    }
}