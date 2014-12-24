using System;
using Game.Data;

namespace Game.Gameplay.Conditions
{
    [Serializable]
    public class Stage
    {
        public int Id;
        public int NextStage = -1;

        public int MinY;
        public int MaxY;
    
        public Condition Condition;
        public event Action StageComplete;
        public bool WinOnComplete;

        protected virtual void InvokeStageComplete()
        {
            Action handler = StageComplete;
            if (handler != null) handler();
        }

        public void OnManaManagersChanged(int points, int i)
        {
            if (Condition.IsScore)
            {
                if (Condition.Target < points)
                {
                    InvokeStageComplete();
                }
            }
        }

    }
}