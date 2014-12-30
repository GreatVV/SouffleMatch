using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class FloorDescBuyButton : MonoBehaviour, IPointerClickHandler
{
    public Text FloorNameLabel;

    public string FloorName
    {
        get
        {
            return _floorName;
        }
        set
        {
            _floorName = value;
            FloorNameLabel.text = _floorName;
        }
    }

    public Tower.Tower Tower;
    [SerializeField]
    private string _floorName;

    public void OnPointerClick(PointerEventData eventData)
    {
        Tower.AddFloor(Instance.FloorFactory.CreateFloor(FloorName));
    }
}