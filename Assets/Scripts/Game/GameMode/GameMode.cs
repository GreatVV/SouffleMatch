using System;

namespace Game.GameMode
{
    [Serializable]
    public abstract class GameMode
    {
        public int Turns;
        public bool IsGameOver;
        public bool IsWin;
        public Points PointSystem;

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
            if (IsGameOver)
            {
                GA.API.Design.NewEvent(string.Format("Game:{0}:Lose:Time", Gamefield.LevelDescription.Name), (float)(DateTime.UtcNow - Gamefield.GameStartTime).TotalSeconds);
                GA.API.Design.NewEvent(string.Format("Game:{0}:Lose:Points", Gamefield.LevelDescription.Name), PointSystem.CurrentPoints);
                InvokeGameOver();
                return true;
            }

            if (IsWin)
            {
                GA.API.Design.NewEvent(string.Format("Game:{0}:Win:Time", Gamefield.LevelDescription.Name), (float)(DateTime.UtcNow - Gamefield.GameStartTime).TotalSeconds);
                GA.API.Design.NewEvent(string.Format("Game:{0}:Win:Points", Gamefield.LevelDescription.Name), PointSystem.CurrentPoints);
                GA.API.Design.NewEvent(string.Format("Game:{0}:Win:Turns", Gamefield.LevelDescription.Name), PointSystem.CurrentPoints);
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
            PointSystem = Gamefield.PointSystem;
            Reset();
            OnDestroy();
            OnInit();
            InvokeTurnsChanged();
        }

        protected abstract void OnInit();
    }
}