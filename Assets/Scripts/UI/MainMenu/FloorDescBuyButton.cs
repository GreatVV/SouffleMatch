using System;
using System.Collections.Generic;
using System.Linq;
using Tower;
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

    [Serializable]
    public class SpriteForFloor
    {
        public FloorType FloorType;
        public Sprite Sprite;
    }

    public List<SpriteForFloor> sprites = new List<SpriteForFloor>();

    public Image Image;

    public Tower.Tower Tower;
    [SerializeField]
    private string _floorName;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Instance.FloorFactory.CanBuildFloor(Tower, FloorName))
        {
            Tower.AddFloor(FloorName);
            Tower.Create();
        }
    }

    public void Init(FloorType type)
    {
        FloorName = type.ToString();
        Image.sprite = sprites.FirstOrDefault(x => x.FloorType == type).Sprite;
    }
}