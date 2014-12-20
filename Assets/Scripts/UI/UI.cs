using Assets.Game.Gameplay;
using Assets.Game.Gameplay.GamefieldStates;
using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{

    public Gamefield Gamefield;

    void Start()
    {
        Gamefield.StateChanged += OnStateChanged;
    }

    public GameObject WinScreen;
    public MissionProgressUI MissionProgressUI;

    private void OnStateChanged(GameState oldState, GameState newState)
    {
        WinScreen.SetActive(newState is WinState || newState is GameOverState);

        if (oldState == null)
        {
            MissionProgressUI.Init(Gamefield.GameMode);
        }
        
    }
}