using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public Text Points;

    public void Init(int points)
    {
        Points.text = points.ToString();
    }

    public void Show(bool state)
    {
        gameObject.SetActive(state);
    }
}