using System;
using Game;
using UnityEngine;

[ExecuteInEditMode]
public class MapId : MonoBehaviour
{
    public int Index;
    public GameObject currentLevelParticle;
    private GameObject effect;

    #region Events Subscribers

    public void OnClick()
    {
        SessionRestorer.Instance.StartLevel(Index);
    }

    #endregion

    #region Unity Methods

    public void Awake()
    {
        name = "0";
        //UpdateName();
    }

    private void OnDisable()
    {
        if (effect)
        {
            Destroy(effect);
        }
    }

    #endregion

    public void UpdateName()
    {
        Index = Convert.ToInt32(name) - 1;
        transform.Search("Label", gameObject).GetComponent<UILabel>().text = name;
    }

    public void ShowCurrent()
    {
        effect = (GameObject) Instantiate(currentLevelParticle);
        effect.transform.parent = transform;
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = Vector3.one;
    }
}