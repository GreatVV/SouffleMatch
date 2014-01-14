using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpDestroyManager : MonoBehaviour
{
    private readonly List<IPowerUp> _powerUpDestroySequince = new List<IPowerUp>();
    public bool IsInDestroyState;
    public static PowerUpDestroyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    public IEnumerator Destroy(IPowerUp powerUpChuzzle)
    {
        if (IsInDestroyState)
        {
            IPowerUp previousChuzzle = _powerUpDestroySequince.LastOrDefault();
            _powerUpDestroySequince.Add(powerUpChuzzle);

            if (previousChuzzle == null)
            {
                yield return StartCoroutine(DestroyCollection(powerUpChuzzle.ToDestroy));
            }
            else
            {
                if (previousChuzzle is HorizontalLineChuzzle)
                {
                    if (powerUpChuzzle is HorizontalLineChuzzle)
                    {
                        int column = (powerUpChuzzle as HorizontalLineChuzzle).Current.x;
                        yield return StartCoroutine(DestroyCollection(GetColumn(column)));
                    }
                    else
                    {
                        yield return StartCoroutine(DestroyCollection(powerUpChuzzle.ToDestroy));
                    }
                }
                else
                {
                    if (previousChuzzle is VerticalLineChuzzle)
                    {
                        if (powerUpChuzzle is VerticalLineChuzzle)
                        {
                            int row = (powerUpChuzzle as VerticalLineChuzzle).Current.y;
                            yield return StartCoroutine(DestroyCollection(GetRow(row)));
                        }
                        else
                        {
                            yield return StartCoroutine(DestroyCollection(powerUpChuzzle.ToDestroy));
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(DestroyCollection(powerUpChuzzle.ToDestroy));
                    }
                }
            }
        }
        else
        {
            IsInDestroyState = true;
            _powerUpDestroySequince.Clear();
            StartCoroutine(Destroy(powerUpChuzzle));
        }
        yield return new WaitForEndOfFrame();
    }

    private static IEnumerator DestroyCollection(IEnumerable<Chuzzle> chuzzleToDestroy)
    {
        foreach (Chuzzle chuzzle in chuzzleToDestroy.ToArray())
        {   
            chuzzle.Destroy(true);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForEndOfFrame();
    }

    public static IEnumerable<Chuzzle> GetColumn(int column)
    {
        return Gamefield.Chuzzles.Where(x => x.Real.x == column);
    }

    public static IEnumerable<Chuzzle> GetRow(int row)
    {
        return Gamefield.Chuzzles.Where(x => x.Real.y == row);
    }

    public static IEnumerable<Chuzzle> GetSquare(int posX, int posY)
    {
        return Gamefield.Chuzzles.Where(
            x =>
                (x.Real.x == posX - 1 || x.Real.x == posX + 1 ||
                 x.Real.x == posX) &&
                (x.Real.y == posY - 1 || x.Real.y == posY ||
                 x.Real.y == posY + 1));
    }
}