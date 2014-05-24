using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UIPanel))]
public abstract class Window : MonoBehaviour
{
    private Collider[] _colliders;

    private UIPanel _panel;

    public Animator Animator;
    public int Depth
    {
        get { return _panel.depth; }
        set
        {
            _panel.depth = value;
         //   Debug.Log("Set depth: "+value);
        }
    }

    protected void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>();
        _panel = GetComponent<UIPanel>();
        Animator = GetComponent<Animator>();
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }

    public void Activate()
    {
        Debug.Log("Activate: "+gameObject.name);
        foreach (var currentCollider in _colliders)
        {
            currentCollider.enabled = true;
        }
        OnShowWindow();
    }

    protected virtual void OnShowWindow()
    {
        Debug.Log("On Activate");
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