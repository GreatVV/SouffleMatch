using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteScale : MonoBehaviour
{

    public Vector2 Scale;

    public void Awake()
    {
#if !UNITY_EDITOR
        var spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(Scale.x/spriteRenderer.sprite.bounds.size.x,
            Scale.y/spriteRenderer.sprite.bounds.size.y, 1);
#endif
    }

    public void Update()
    {
#if UNITY_EDITOR

        var spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(Scale.x/spriteRenderer.sprite.bounds.size.x,
            Scale.y/spriteRenderer.sprite.bounds.size.y, 1);

#endif
    }
}
