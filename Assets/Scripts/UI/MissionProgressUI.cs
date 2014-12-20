using System;
using System.Globalization;
using Assets.Game.Data;
using Assets.Game.GameMode;
using Assets.Game.Player;
using UnityEngine;
using UnityEngine.UI;

public class MissionProgressUI : MonoBehaviour
{
    public Text MissionProgress;
    public Text MissionTarget;
    public Text Turns;
    
    private ManaManager _manaManager = null;

    private GameMode _gameMode;

    void OnDestroy()
    {
        if (_manaManager)
        {
            _manaManager.PointChanged -= OnPointsChanged;
        }
    }

    private void OnPointsChanged(int currentPoints, int targetPoints)
    {
        if (_gameMode is TargetScoreGameMode)
        {
            MissionProgress.text = currentPoints.ToString(CultureInfo.InvariantCulture);
        }
    }

    public void Init(GameMode gameMode)
    {
        _gameMode = gameMode;
        _gameMode.TurnsChanged += OnTurnsChanged;
        _manaManager = _gameMode.ManaManagerSystem;

        _manaManager.PointChanged -= OnPointsChanged;
        _manaManager.PointChanged += OnPointsChanged;

        if (_gameMode is TargetScoreGameMode)
        {
            MissionProgress.text = "0";
            var targetScoreGameMode = (_gameMode as TargetScoreGameMode);
            MissionTarget.text = targetScoreGameMode.TargetScore.ToString(CultureInfo.InvariantCulture);
        }

        OnTurnsChanged(_gameMode.Turns, _gameMode.StartTurns);
    }

    private void OnTurnsChanged(int turnsLeft, int startTurns)
    {
        Turns.text = turnsLeft.ToString(CultureInfo.InvariantCulture);
    }
}