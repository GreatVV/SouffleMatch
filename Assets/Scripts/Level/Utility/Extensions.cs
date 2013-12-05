using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void AddUniqRange(this List<Chuzzle> list, IEnumerable<Chuzzle> range)
    {
        foreach (var item in range)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }

    public static Transform Search(this Transform target, string name, GameObject original = null)
    {
        if (target.name == name && target.gameObject != original) return target;

        for (int i = 0; i < target.childCount; ++i)
        {
            var result = Search(target.GetChild(i), name);

            if (result != null) return result;
        }

        return null;
    }
}