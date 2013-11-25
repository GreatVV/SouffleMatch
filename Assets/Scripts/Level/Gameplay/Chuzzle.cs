#region

using System;
using System.Globalization;
using UnityEngine;

#endregion

[RequireComponent(typeof(TeleportableEntity))]
public class Chuzzle : MonoBehaviour
{
    public Cell Current;

    public Cell MoveTo;

    public Cell Real;

    public ChuzzleType Type;

    public bool IsCheckedForSearch;

    public PowerType PowerType;

    public int Counter
    {
        get { return _counter; }
        set
        {
            _counter = value;
            var counter = GetComponentInChildren<TextMesh>();
            if (counter != null)
            {
                counter.text = Counter.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    public SpriteRenderer Sprite;

    public bool _shine;

    public float Alpha;
    private bool _frozen;

    public GameObject Arrow;
    public Vector3 Velocity;
    private int _counter;

    public bool Shine
    {
        get { return _shine; }
        set
        {
            _shine = value;
            Alpha = 1;
            if (!_shine && Arrow != null)
            {
                Destroy(Arrow);
            }
        }
    }   

    public Vector3 Scale
    {
        get { return transform.localScale; }
    }

    public override string ToString()
    {
        return "" + Type + " (" + Current.x + "," + Current.y + ")";
    }                       

    private void Update()
    {
        /*
        if (Shine && !Frozen)
        {
            Alpha = Alpha + Time.deltaTime*3;
            Sprite.color = new Color(Sprite.color.r, Sprite.color.g, Sprite.color.b, Mathf.Sin(Alpha)/2f + 0.5f);
        }                           */
    }

    public bool Frozen
    {
        get { return _frozen; }
        set
        {
            _frozen = value;
            //todo
            //Sprite.color = new Color(Sprite.color.r, Sprite.color.g, Sprite.color.b, _frozen ? 0.1f : 1f);
        }
    }

    public int TimesDestroyed;

    void OnDisable()
    {
        //drop all event handlers
        Died = null;
    }

    public void Die()
    {   
        //TODO Do Explosion
        if (transform.localScale != Vector3.zero)
        {
            iTween.ScaleTo(gameObject,
                iTween.Hash(
                    "x", 0,
                    "y", 0,
                    "z", 0,
                    "time", 0.5f,
                    "oncomplete", new Action<object>(OnDeathAnimationEnd),
                    "oncompleteparams", this));
        }
        else
        {
            OnDeathAnimationEnd(this);
        }
    }

    private void OnDeathAnimationEnd(object chuzzle)
    {
        InvokeDied();
    }

    public event Action<Chuzzle> Died;

    protected virtual void InvokeDied()
    {
        var handler = Died;
        if (handler != null) handler(this);
    }
}