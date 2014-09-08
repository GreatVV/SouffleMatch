using UnityEngine;

public abstract class Window : MonoBehaviour
{
    private Collider[] _colliders;

    private RectTransform _panel;

    protected void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>();
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }

    public void Activate()
    {
       // Debug.Log("Activate: "+gameObject.name);
        foreach (var currentCollider in _colliders)
        {
            currentCollider.enabled = true;
        }
        OnShowWindow();
    }

    protected virtual void OnShowWindow()
    {
       // Debug.Log("On Activate");
    }

    public void Deactivate()
    {
        foreach (var currentCollider in _colliders)
        {
            currentCollider.enabled = false;
        }
        OnHideWindow();
    }

    protected virtual void OnHideWindow()
    {
        
    }
}