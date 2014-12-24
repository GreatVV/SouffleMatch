using System.Collections.Generic;
using System.Linq;
using Game.Gameplay.Chuzzles;
using UnityEngine;

namespace Game.Utility
{
    public static class Extensions
    {
        public static void AddUniqRange(this List<Chuzzle> list, IEnumerable<Chuzzle> range)
        {
            list.AddRange(range.Where(x=>!list.Contains(x)));
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

        public static bool IsPowerUp(this Chuzzle chuzzle)
        {
            return GamefieldUtility.IsPowerUp(chuzzle);
        }
    }
}