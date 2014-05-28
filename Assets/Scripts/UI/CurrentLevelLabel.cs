using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
public class CurrentLevelLabel : MonoBehaviour
{

    private UILabel label;
    [SerializeField]
    private Gamefield gamefield;

    void Awake()
    {
        label = GetComponent<UILabel>();
        gamefield.GameStarted += OnGameStarted;
    }

    void OnDestroy()
    {
        gamefield.GameStarted -= OnGameStarted;
    }

    private void OnGameStarted(Gamefield gamefield)
    {
        label.text = gamefield.LevelName;
    }
}
