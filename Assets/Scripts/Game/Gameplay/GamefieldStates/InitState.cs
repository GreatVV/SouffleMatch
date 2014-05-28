using System;
using Game;
using Game.Data;
using Game.GameMode;

namespace GamefieldStates
{
    [Serializable]
    public class InitState : GamefieldState
    {
        #region Event Handlers

        public override void OnEnter()
        {
            if (Gamefield.CheckSpecialState)
            {
                Destroy(Gamefield.CheckSpecialState);
            }
            Gamefield.CheckSpecialState = gameObject.AddComponent<CheckSpecialState>();

            if (Gamefield.CreateNewChuzzlesState)
            {
                Destroy(Gamefield.CreateNewChuzzlesState);
            }
            Gamefield.CreateNewChuzzlesState = gameObject.AddComponent<CreateNewChuzzlesState>();

            if (Gamefield.RemoveState)
            {
                Destroy(Gamefield.RemoveState);
            }
            Gamefield.RemoveState = gameObject.AddComponent<RemoveCombinationState>();

            if (Gamefield.GameOverState)
            {
                Destroy(Gamefield.GameOverState);
            }
            Gamefield.GameOverState = gameObject.AddComponent<GameOverState>();

            if (Gamefield.WinState)
            {
                Destroy(Gamefield.WinState);
            }
            Gamefield.WinState = gameObject.AddComponent<WinState>();

            if (Gamefield.FieldState)
            {
                Destroy(Gamefield.FieldState);
            }
            Gamefield.FieldState = gameObject.AddComponent<FieldState>();


            Gamefield.LevelDescription = Player.Instance.LastPlayedLevelDescription;

            Gamefield.PointSystem.Reset();
            Gamefield.Level.InitFromFile(Gamefield.LevelDescription.Field);

            Gamefield.GameMode = GameModeFactory.CreateGameMode(Gamefield.LevelDescription.Condition.GameMode);
            Gamefield.GameMode.Init(Gamefield);
            Gamefield.PointSystem.TargetPoints = Gamefield.GameMode.TargetPoints;

            Gamefield.AddEventHandlers();

            Gamefield.InvokeGameStarted();
            Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
        }

        public override void OnExit()
        {
        }

        #endregion

        public override void UpdateState()
        {
        }

        public override void LateUpdateState()
        {
        }
    }
}