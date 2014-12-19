using System.Collections.Generic;
using System.Linq;
using Assets.Game.Visual;
using UnityEngine;

namespace Assets.Game.Gameplay.Conditions
{
    [RequireComponent(typeof(Gamefield))]
    public class StageManager : MonoBehaviour
    {
        public List<Stage> Stages = new List<Stage>();

        public Stage CurrentStage;

        [HideInInspector]
        public Gamefield Gamefield;

        public GameObject Camera;

        public void Awake()
        {
            Gamefield = GetComponent<Gamefield>();
        }

        public void Init(List<Stage> stages)
        {   
            if (stages == null || stages.Count < 2)
            {
                if (CurrentStage != null)
                {
                    Gamefield.ManaManagerSystem.PointChanged -= CurrentStage.OnManaManagersChanged;
                    CurrentStage.StageComplete -= OnStageComplete;
                    CurrentStage = null;
                }
                Stages.Clear();
                //Gamefield.Level.UpdateActive();
                CenterCameraOnField.Instance.CenterCameraOnChuzzles(Gamefield.Level.Chuzzles.GetTiles(), true);
                return;
            }

            Stages = stages;
            ChangeStageTo(0);
        }

  
        private void OnStageComplete()
        {
            Gamefield.ManaManagerSystem.PointChanged -= CurrentStage.OnManaManagersChanged;
            CurrentStage.StageComplete -= OnStageComplete;
            if (CurrentStage.NextStage == -1 )
            {
                if (CurrentStage.WinOnComplete)
                {
                    Gamefield.GameMode.IsWin = true;
                }
            }
            else
            {
                ChangeStageTo(CurrentStage.NextStage);
            }
        }

        public void ChangeStageTo(int id)
        {
            CurrentStage = Stages.First(x => x.Id == id);

            CurrentStage.StageComplete += OnStageComplete;
            Gamefield.ManaManagerSystem.PointChanged += CurrentStage.OnManaManagersChanged;

            //    Gamefield.Level.ChoseFor(CurrentStage.MinY, CurrentStage.MaxY);
        
            CenterCameraOnField.Instance.CenterCameraOnChuzzles(Gamefield.Level.Chuzzles.GetTiles(), false);

        }
    }
}
