using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Text))]
public class CurrentLevelLabel : MonoBehaviour
{
    [SerializeField] private Gamefield gamefield;
    private Text label;

    private void Awake()
    {
        label = GetComponent<Text>();
        gamefield.GameStarted += OnGameStarted;
    }

    private void OnDestroy()
    {
        gamefield.GameStarted -= OnGameStarted;
    }

    private void OnGameStarted(Gamefield gamefield)
    {
        label.text = gamefield.LevelName;
    }
}