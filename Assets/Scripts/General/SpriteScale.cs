using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteScale : MonoBehaviour
{

    public Vector2 Scale;

    public void Start()
    {
#if !UNITY_EDITOR
        Zoom();
#endif
    }

    public void Update()
    {
#if UNITY_EDITOR
        Zoom(); 
#endif
    }

    private void Zoom()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(Scale.x / spriteRenderer.sprite.bounds.size.x * Mathf.Sign(transform.localScale.x), Scale.y / spriteRenderer.sprite.bounds.size.y * Mathf.Sign(transform.localScale.y), 1);
        if (GetComponent<BoxCollider2D>())
        {
            GetComponent<BoxCollider2D>().size = new Vector2(Mathf.Abs(Scale.x/transform.localScale.x), Mathf.Abs(Scale.y/transform.localScale.y));
        }
    }
}
