using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class CropSprite : MonoBehaviour
{                                       
    public SpriteRenderer SpriteRenderer;
    public Rect OriginalRect;
    // Use this for initialization
	void Start ()
	{
	    OriginalRect = SpriteRenderer.sprite.rect;
	    //SpriteRenderer.sprite.rect = OriginalRect;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
