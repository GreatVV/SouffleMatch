using System.Collections.Generic;

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
}