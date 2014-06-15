#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion

public class GamefieldUtility
{
    #region Find Combination

    public static List<List<Chuzzle>> FindCombinations(IEnumerable<Chuzzle> chuzzles, int combinationSize = 3)
    {            
        var combinations = new List<List<Chuzzle>>();

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }
        
        //find combination
        foreach (var c in chuzzles)
        {
            if (c.IsCheckedForSearch) continue;

            var combination = RecursiveFind(c, new List<Chuzzle>(), chuzzles);

            if (combination.Count() >= combinationSize)
            {
                combinations.Add(combination);
            }
        }

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        return combinations;
    }

    /// <summary>
    /// Try to find any combination of chuzzles from chuzzles with length more then combinationSize
    /// </summary>
    /// <param name="chuzzles">list of chuzzle to find in</param>
    /// <param name="combinationSize">required combination size</param>
    /// <returns></returns>
    public static List<Chuzzle> FindOnlyOneCombination(TilesCollection chuzzles, int combinationSize = 3)
    {
        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        //find combination
        foreach (var c in chuzzles)
        {
            if (c.IsCheckedForSearch) continue;

            var combination = RecursiveFind(c, new List<Chuzzle>(), chuzzles);

            if (combination.Count() >= combinationSize)
            {
                return combination;
            }
        }

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        return new List<Chuzzle>();
    }

    public static List<Chuzzle> FindOnlyOneCombinationWithCondition(IEnumerable<Chuzzle> chuzzles, Func<Chuzzle, bool> condition, int combinationSize = 3)
    {
        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        //find combination
        foreach (var c in chuzzles)
        {
            if (c.IsCheckedForSearch) continue;

            var combination = RecursiveFind(c, new List<Chuzzle>(), chuzzles);

            if (combination.Count() >= combinationSize && combination.Any(condition))
            {
                return combination;
            }
        }

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        return new List<Chuzzle>();
    }


    public static List<Chuzzle> RecursiveFind(Chuzzle chuzzle, List<Chuzzle> combination, IEnumerable<Chuzzle> chuzzles)
    {
        if (chuzzle == null || combination.Contains(chuzzle) || chuzzle.IsCheckedForSearch)
        {
            return new List<Chuzzle>();
        }
        combination.Add(chuzzle);
        chuzzle.IsCheckedForSearch = true;

        var left = GetLeftFor(chuzzle, chuzzles);
        if (left != null && IsSameColor(chuzzle, left))
        {
            var answer = RecursiveFind(left, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var right = GetRightFor(chuzzle, chuzzles);
        if (right != null && IsSameColor(chuzzle, right))
        {
            var answer = RecursiveFind(right, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var top = GetTopFor(chuzzle, chuzzles);
        if (top != null && IsSameColor(chuzzle,top))
        {
            var answer = RecursiveFind(top, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        var bottom = GetBottomFor(chuzzle, chuzzles);
        if (bottom != null && IsSameColor(chuzzle, bottom))
        {
            var answer = RecursiveFind(bottom, combination, chuzzles);
            foreach (var a in answer)
            {
                if (combination.Contains(a) == false)
                {
                    combination.Add(a);
                }
            }
        }

        return combination;
    }

    public static bool IsSameColor(Chuzzle a, Chuzzle b)
    {
        if (a == null || b == null)
        {
            //Debug.LogError(String.Format("A or b is NULL. a: {0} b: {1}", a, b));
            return false;
        }

        if (a is InvaderChuzzle || b is InvaderChuzzle)
        {
            return false;
        }

        return a.Color == b.Color;
    }

    public static Chuzzle GetLeftFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Left);
    }

    public static Chuzzle GetRightFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Right);
    }

    public static Chuzzle GetTopFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Top);
    }

    public static Chuzzle GetBottomFor(Chuzzle c, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Real == c.Real.Bottom);
    }

    #endregion

    #region Tips

    public static int CompareByX(Chuzzle first, Chuzzle second)
    {
        if (first.Current.x == second.Current.x)
        {
            return 0;
        }

        return first.Current.x > second.Current.x ? 1 : -1;
    }

    public static int CompareByY(Chuzzle first, Chuzzle second)
    {
        if (first.Current.y == second.Current.y)
        {
            return 0;
        }

        return first.Current.y > second.Current.y ? 1 : -1;
    }

    /// <summary>
    ///     Находит любую возможную комбинацию
    /// </summary>
    /// <param name="chuzzles">Список элементов в котором надо найти комбинацию</param>
    /// <returns>Список элементов которые составляют эту комбинацию</returns>
    public static List<Chuzzle> Tip(TilesCollection chuzzles, out IntVector2 isHorizontalMove, out Chuzzle chuzzleToMove)
    {
        var bottom =
            chuzzles.FirstOrDefault(
                x => BetweenYCheck(x, chuzzles));

        if (bottom != null && bottom.Current.Top != null && bottom.Current.Top.Type != CellTypes.Block)
        {
            var top = chuzzles.First(ch => ch.Current == bottom.Current.Top.Top);

            var bottomPart = RecursiveFind(bottom, new List<Chuzzle>(), chuzzles);
            var middlePart = GetHorizontalLineChuzzles(bottom.Current.y + 1, bottom.Color, chuzzles);
            var topPart = RecursiveFind(top, new List<Chuzzle>(), chuzzles);

            var possibleCombination = new List<Chuzzle>();
            possibleCombination.AddRange(bottomPart);
            possibleCombination.AddRange(middlePart);
            possibleCombination.AddRange(topPart);

            //Debug.Log("Combination 1");
            isHorizontalMove = new IntVector2(bottom.Current.x, bottom.Current.y + 1);
            chuzzleToMove = middlePart.First();
            //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
            //Debug.Log(target);
            return possibleCombination;
        }

        var left = chuzzles.FirstOrDefault(x => BetweenXCheck(x, chuzzles));

        if (left != null && left.Current.Right != null && left.Current.Right.Type != CellTypes.Block)
        {
            var right = chuzzles.First(ch => ch.Current == left.Current.Right.Right);

            var leftPart = RecursiveFind(left, new List<Chuzzle>(), chuzzles);
            var middlePart = GetVerticalLineChuzzles(left.Current.Right.x, left.Color, chuzzles);
            var rightPart = RecursiveFind(right, new List<Chuzzle>(), chuzzles);

            var possibleCombination = new List<Chuzzle>();
            possibleCombination.AddRange(leftPart);
            possibleCombination.AddRange(middlePart);
            possibleCombination.AddRange(rightPart);

            isHorizontalMove = new IntVector2(left.Current.x + 1, left.Current.y);
            chuzzleToMove = middlePart.First();
            //Debug.Log("Combination 2: " + chuzzleToMove);
            //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
            //Debug.Log(target);
            return possibleCombination;
        }

        var combinations = FindCombinations(chuzzles, 2);

        foreach (var combination in combinations)
        {         
            var first = combination[0];
            var second = combination[1];

            //vertical combination
            if (first.Current.x == second.Current.x)
            {
                combination.Sort(CompareByY);
                var topChuzzle = combination[0];
                var bottomChuzzle = combination[1];
                //try left             
                if ((topChuzzle.Current.Left != null && topChuzzle.Current.Left.Type != CellTypes.Block) ||
                    (bottomChuzzle.Current.Left != null && bottomChuzzle.Current.Left.Type != CellTypes.Block))
                {
                    var leftPart = GetVerticalLineChuzzles(topChuzzle.Current.x - 1, topChuzzle.Color, chuzzles).ToList();
                    if (leftPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(leftPart);

                        //Debug.Log("Combination 3");
                        //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(topChuzzle.Current.x - 1, topChuzzle.Current.y);
                        chuzzleToMove = leftPart.First();
                        return possibleCombination;
                    }
                }

                //try right
                if ((topChuzzle.Current.Right != null && topChuzzle.Current.Right.Type != CellTypes.Block) ||
                    (bottomChuzzle.Current.Right != null && bottomChuzzle.Current.Right.Type != CellTypes.Block))
                {
                    var rightPart = GetVerticalLineChuzzles(topChuzzle.Current.x + 1, topChuzzle.Color, chuzzles).ToList();
                    if (rightPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(rightPart);

                        //Debug.Log("Combination 4");
                        //Svar target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(topChuzzle.Current.x + 1, topChuzzle.Current.y);
                        chuzzleToMove = rightPart.First();
                        return possibleCombination;
                    }
                }

                //try top    
                if (bottomChuzzle.Current.Top != null && bottomChuzzle.Current.Top.Type != CellTypes.Block &&
                    chuzzles.Any(x => x.Current == bottomChuzzle.Current.Top))
                {
                    var topPart = GetHorizontalLineChuzzles(bottomChuzzle.Current.Top.y, bottomChuzzle.Color, chuzzles).ToList();
                    if (topPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(topPart);

                        //Debug.Log("Combination 5");
                        //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(bottomChuzzle.Current.x, bottomChuzzle.Current.Top.y);
                        chuzzleToMove = topPart.First();
                        return possibleCombination;
                    }
                }

                //try bottom    
                if (topChuzzle.Current.Bottom != null && topChuzzle.Current.Bottom.Type != CellTypes.Block &&
                    chuzzles.Any(x => x.Current == topChuzzle.Current.Bottom))
                {
                    var bottomPart = GetHorizontalLineChuzzles(topChuzzle.Current.Bottom.y, topChuzzle.Color, chuzzles).ToList();
                    if (bottomPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(bottomPart);

                        //Debug.Log("Combination 6");
                        //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(bottomChuzzle.Current.x, bottomChuzzle.Current.Bottom.y);
                        chuzzleToMove = bottomPart.First();
                        return possibleCombination;
                    }
                }
            }
            else
            {
                combination.Sort(CompareByX);
                var leftChuzzle = combination[0];
                var rightChuzzle = combination[1];
                //horizontal combinations
               
                //try left             
                if (leftChuzzle.Current.Left != null && leftChuzzle.Current.Left.Type != CellTypes.Block)
                {
                    var leftPart = GetVerticalLineChuzzles(leftChuzzle.Current.Left.x, leftChuzzle.Color, chuzzles).ToList();
                    if (leftPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(leftPart);

                      //  //Debug.Log("Left:"+leftChuzzle);
                      //  //Debug.Log("Right:"+rightChuzzle.ToString());

                        //Debug.Log("Combination 7");
                        //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(leftChuzzle.Current.Left.x, leftChuzzle.Current.y);
                        chuzzleToMove = leftPart.First();
                        return possibleCombination;
                    }
                }

                //try right
                if (rightChuzzle.Current.Right != null && rightChuzzle.Current.Right.Type != CellTypes.Block)
                {
                    var rightPart = GetVerticalLineChuzzles(rightChuzzle.Current.x + 1, rightChuzzle.Color, chuzzles).ToList();
                    if (rightPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(rightPart);

                        //Debug.Log("Combination 8");
                        //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(rightChuzzle.Current.x + 1, rightChuzzle.Current.y);
                        chuzzleToMove = rightPart.First();
                        return possibleCombination;
                    }
                }

                //try top    
                if (
                    (leftChuzzle.Current.Top != null && leftChuzzle.Current.Top.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == leftChuzzle.Current.Top)) ||
                    (rightChuzzle.Current.Top != null && rightChuzzle.Current.Top.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == rightChuzzle.Current.Top))
                    )
                {
                    var topPart = GetHorizontalLineChuzzles(rightChuzzle.Current.y + 1, rightChuzzle.Color, chuzzles).ToList();
                    if (topPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(topPart);

                        //Debug.Log("Combination 9");
                        //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(rightChuzzle.Current.x, rightChuzzle.Current.y + 1);
                        chuzzleToMove = topPart.First();
                        return possibleCombination;
                    }
                }

                //try bottom    
                if (
                    (leftChuzzle.Current.Bottom != null && leftChuzzle.Current.Bottom.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == leftChuzzle.Current.Bottom)) ||
                    (rightChuzzle.Current.Bottom != null && rightChuzzle.Current.Bottom.Type != CellTypes.Block &&
                     chuzzles.Any(x => x.Current == rightChuzzle.Current.Bottom))
                    )
                {
                    var bottomPart = GetHorizontalLineChuzzles(leftChuzzle.Current.y - 1, leftChuzzle.Color, chuzzles).ToList();
                    if (bottomPart.Any())
                    {
                        var possibleCombination = new List<Chuzzle>();
                        possibleCombination.AddRange(combination);
                        possibleCombination.AddRange(bottomPart);

                        //Debug.Log("Combination 10");
                        //var target = possibleCombination.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
                        //Debug.Log(target);
                        isHorizontalMove = new IntVector2(leftChuzzle.Current.x, leftChuzzle.Current.y - 1);
                        chuzzleToMove = bottomPart.First();
                        return possibleCombination;
                    }
                }
            }
        }
        //Debug.Log("Combination NOOOOOOOOOO 11");
        Repaint(chuzzles,100);
        Tip(chuzzles, out isHorizontalMove, out chuzzleToMove);
            
        isHorizontalMove = new IntVector2();
        chuzzleToMove = null;
        return new List<Chuzzle>();
    }

    public static bool BetweenYCheck(Chuzzle chuzzle, IEnumerable<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle = allChuzzles.FirstOrDefault( ch => 
            ch.Current.x == firstChuzzle.Current.x && 
            ch.Current.y == firstChuzzle.Current.y + 2 && 
            IsSameColor(ch, firstChuzzle));

        if (secondChuzzle == null || allChuzzles.Any(x => x.Current.y == firstChuzzle.Current.y + 1 && IsLock(x)))
            return false;

        return allChuzzles.Any(x => x.Current.y == firstChuzzle.Current.y + 1 && IsSameColor(x, firstChuzzle) );
    }

    public static bool BetweenXCheck(Chuzzle chuzzle, IEnumerable<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                ch =>
                    ch.Current.y == firstChuzzle.Current.y && ch.Current.x == firstChuzzle.Current.x + 2 &&
                    IsSameColor(ch, firstChuzzle));

        if (secondChuzzle == null || allChuzzles.Any(x => x.Current.x == firstChuzzle.Current.x + 1 && IsLock(x)))
            return false;

        return allChuzzles.Any(x => x.Current.x == firstChuzzle.Current.x + 1 && IsSameColor(x, firstChuzzle) && !IsLock(x));
    }

    // вертикальная и горизонтальная проверка для второго случая
    public static bool AnotherVerticalCheck(Chuzzle chuzzle, IEnumerable
        <Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                ch =>
                    ch.Current.x == firstChuzzle.Current.x && ch.Current.y == firstChuzzle.Current.y + 1 &&
                    IsSameColor(ch, firstChuzzle));

        if (secondChuzzle == null) return false;

        return
            allChuzzles.Where(
                ch =>
                    Math.Abs(ch.Current.x - firstChuzzle.Current.x) == 1 || ch.Current.y == firstChuzzle.Current.y - 1 ||
                    ch.Current.y == firstChuzzle.Current.y + 2).Any(ch => IsSameColor(ch, firstChuzzle));
    }

    public static bool AnotherHorizontalCheck(Chuzzle chuzzle, IEnumerable<Chuzzle> allChuzzles)
    {
        var firstChuzzle = chuzzle;
        var secondChuzzle =
            allChuzzles.FirstOrDefault(
                ch =>
                    ch.Current.y == firstChuzzle.Current.y && ch.Current.x == firstChuzzle.Current.x + 1 && IsSameColor(ch, firstChuzzle));

        if (secondChuzzle == null) return false;

        return
            allChuzzles.Where(
                ch =>
                    Math.Abs(ch.Current.y - firstChuzzle.Current.y) == 1 || ch.Current.x == firstChuzzle.Current.x - 1 ||
                    ch.Current.x == firstChuzzle.Current.x + 2).Any(ch => IsSameColor(ch, firstChuzzle));

        //return false;
    }

    #endregion

    #region New Tips

    public static IEnumerable<Chuzzle> GetHorizontalLineChuzzles(int y, ChuzzleColor chuzzleColor,
        IEnumerable<Chuzzle> chuzzles)
    {
        var enumerable = chuzzles as IList<Chuzzle> ?? chuzzles.ToList();
        var firstChuzzle = enumerable.FirstOrDefault(x => x.Real.y == y && x.Color == chuzzleColor && IsOrdinaryDestroyable(x));
        if (firstChuzzle != null && !enumerable.Any(c => c is LockChuzzle && c.Current.y == y))
        {
            var secondChuzzle =
                enumerable.FirstOrDefault(
                    c => IsSameColor(c, firstChuzzle) &&
                        (c.Current == firstChuzzle.Current.Left || c.Current == firstChuzzle.Current.Right));
            if (secondChuzzle != null)
            {
                return new List<Chuzzle> {firstChuzzle, secondChuzzle};
            }
            return new List<Chuzzle> {firstChuzzle};
        }
        return new List<Chuzzle>();
    }

    public static IEnumerable<Chuzzle> GetVerticalLineChuzzles(int x, ChuzzleColor chuzzleColor,
        IEnumerable<Chuzzle> chuzzles)
    {
        var firstChuzzle = chuzzles.FirstOrDefault(c => c.Real.x == x && c.Color == chuzzleColor && !(c is InvaderChuzzle));
        ////Debug.Log(string.Format("fc:{0} |x: {1} Color: {2}", firstChuzzle, x, chuzzleColor));
       // //Debug.Log("Any is lock: "+chuzzles.FirstOrDefault(c=>c is LockChuzzle));
        if (firstChuzzle != null && !chuzzles.Any(c => c is LockChuzzle && c.Current.x == x))
        {
            var secondChuzzle = chuzzles.FirstOrDefault(c => IsSameColor(c, firstChuzzle) && (c.Current == firstChuzzle.Current.Top || c.Current == firstChuzzle.Current.Bottom));
            if (secondChuzzle != null)
            {
                return new List<Chuzzle> {firstChuzzle, secondChuzzle};
            }
            return new List<Chuzzle> {firstChuzzle};
        }
        return new List<Chuzzle>();
    }

    #endregion

    public static IntVector2 ToRealCoordinates(Chuzzle chuzzle)
    {
        return new IntVector2(Mathf.RoundToInt(chuzzle.transform.position.x/Chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.position.y/Chuzzle.Scale.y));
    }

    public static Cell CellAt(IEnumerable<Cell> cells, int x, int y)
    {
        return cells.FirstOrDefault(c => c.x == x && c.y == y);
    }

    public static Vector3 ConvertXYToPosition(int x, int y, Vector3 scale)
    {
        return new Vector3(x*scale.x, y*scale.y, 0);
    }

    public static void ShowArrow(Chuzzle from, IntVector2 to, TipArrow tipArrow)
    {
        //Debug.Log(string.Format("Arrow. From:{0} To:{1} ", from, to));
        if (@from.Current.x == to.x)
        {
            //vertical
            if (@from.Current.y >= to.y)
            {
                //to down
                //do nothing
                tipArrow.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                //to up
                //mirror vertical
                tipArrow.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
        else
        {
            //horizontal
            if (@from.Current.x < to.x)
            {
                //to right
                tipArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                //to left
                //to right
                tipArrow.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        }

        tipArrow.Chuzzle = @from;
    }

    public static Cell MaxColumnAvailiablePosition(int column, IEnumerable<Cell> cells)
    {
        var enumerable = cells as Cell[] ?? cells.ToArray();
        //Debug.Log("Column: "+column);
        //Debug.Log("Cells NUmber: "+cells.Count());
        var maxCell =  enumerable.First(cell => cell.x == column && !cell.IsTemporary && cell.y == enumerable.Where(c=>!c.IsTemporary).Max(y => y.y));
        if (maxCell.Type != CellTypes.Usual)
        {
            maxCell = maxCell.GetBottomWithType();
        }
        return maxCell;
    }

    public static Cell MinColumnAvailiablePosition(int column, IEnumerable<Cell> cells)
    {
        var enumerable = cells as Cell[] ?? cells.ToArray();

        var minCell =
            enumerable.First(
                x => x.x == column && !x.IsTemporary && x.y == enumerable.Where(c => !c.IsTemporary).Min(y => y.y));
        if (minCell.Type != CellTypes.Usual)
        {
            minCell = minCell.GetTopWithType();
        }
        return minCell;
    }

    public static Cell MinRowAvailiablePosition(int row, IEnumerable<Cell> cells)
    {
        var enumerable = cells as Cell[] ?? cells.ToArray();
        var minCell = enumerable.First(x => x.y == row && !x.IsTemporary && x.x == enumerable.Where(c => !c.IsTemporary).Min(y => y.x));
        if (minCell.Type != CellTypes.Usual)
        {
            minCell = minCell.GetRightWithType();
        }
        return minCell;
    }

    public static Cell MaxRowAvailiablePosition(int row, IEnumerable<Cell> cells)
    {
        var enumerable = cells as Cell[] ?? cells.ToArray();
        var maxCell = enumerable.First(x => x.y == row && !x.IsTemporary && x.x == enumerable.Where(c => !c.IsTemporary).Max(y => y.x));
        if (maxCell.Type != CellTypes.Usual)
        {
            maxCell = maxCell.GetLeftWithType();
        }
        return maxCell;
    }

    public static bool IsPowerUp(Chuzzle chuzzle)
    {
        return chuzzle is HorizontalLineChuzzle || chuzzle is VerticalLineChuzzle || chuzzle is BombChuzzle;
    }

    public static bool IsUsual(Chuzzle chuzzle)
    {
        return chuzzle is ColorChuzzle;
    }

    public static bool IsLock(Chuzzle chuzzle)
    {
        return chuzzle is LockChuzzle;
    }

    public static bool IsCounter(Chuzzle chuzzle)
    {
        return chuzzle is CounterChuzzle;
    }

    public static bool IsOrdinaryDestroyable(Chuzzle chuzzle)
    {
        return !(chuzzle is InvaderChuzzle);
    }

    public static Chuzzle GetChuzzleInCell(Cell cell, IEnumerable<Chuzzle> chuzzles)
    {
        return chuzzles.FirstOrDefault(x => x.Current == cell);
    }

    public static bool Repaint(TilesCollection tiles,int numberOfTries)
    {
        if (numberOfTries == 0)
        {
            return false;
        }
        var possible = tiles.Where(IsUsual);
            //.GetTiles(IsUsual);
        //complex logic of repainting

        //check number of invaders
        //if more then third of maximum and possible less then 10
        if (InvaderChuzzle.AllInvaderChuzzles.Count > InvaderChuzzle.MaxInvadersOnLevel/3 && possible.Count() < 10)
        {
            ////Debug.Log("Repaint invaders");
            var invadersForRepaint =
                InvaderChuzzle.AllInvaderChuzzles.Where(
                    x =>
                        InvaderChuzzle.AllInvaderChuzzles.IndexOf(x) <
                        InvaderChuzzle.MaxInvadersOnLevel/3).ToArray();
            //repaint them to random color
            foreach (var invaderToReplace in invadersForRepaint)
            {
                TilesFactory.Instance.ReplaceWithRandom(invaderToReplace);
            }
            //TODO show message to player                               
        }
        else
        {
            //try to find pair and repaint only one
            var combinations = FindCombinations(tiles, 2);
            if (combinations.Any())
            {
                foreach (var comb in combinations)
                {
                    //if vertical 
                    if (comb[0].Current.x == comb[1].Current.x)
                    {
                        //try to find up and bottom
                        var top = comb[0].Current.y > comb[1].Current.y ? comb[0] : comb[1];
                        var bottom = top == comb[0] ? comb[1] : comb[0];

                        var repainted = false;
                        if (top.Current.Top != null && top.Current.Top.Type != CellTypes.Block)
                        {
                            var possibleAbove =
                                tiles.Where(
                                    x =>
                                        IsUsual(x) && x.Current.y == top.Current.Top.y &&
                                        x.Current != top.Current.Top)
                                    .ToArray();

                            if (possibleAbove.Any())
                            {
                                Chuzzle toReplace = possibleAbove[Random.Range(0, possibleAbove.Length)];
                                TilesFactory.Instance.ReplaceWithColor(toReplace, top.Color);
                                //    //Debug.Log("Repaint above pair");
                                repainted = true;
                            }
                        }

                        if (bottom.Current.Bottom != null &&
                            bottom.Current.Bottom.Type != CellTypes.Block && !repainted)
                        {
                            var possibleBellow =
                                tiles.Where(
                                    x =>
                                        IsUsual(x) &&
                                        x.Current.y == bottom.Current.Bottom.y &&
                                        x.Current != bottom.Current.Bottom)
                                    .ToArray();

                            if (possibleBellow.Any())
                            {
                                Chuzzle toReplace =
                                    possibleBellow[Random.Range(0, possibleBellow.Length)];
                                TilesFactory.Instance.ReplaceWithColor(
                                    toReplace, bottom.Color);
                                repainted = true;
                                //      //Debug.Log("Repaint bellow pair");
                            }
                        }

                        if (repainted)
                        {
                            break;
                        }
                    }
                    else
                    {
                        //horizontal pair

                        var left = comb[0].Current.x < comb[1].Current.x ? comb[0] : comb[1];
                        var right = comb[0] == left ? comb[1] : comb[0];

                        var repainted = false;

                        //left?
                        if (left.Current.Left != null && left.Current.Left.Type != CellTypes.Block)
                        {
                            var possibleLeft =
                                tiles.Where(
                                    x =>
                                        IsUsual(x) &&
                                        x.Current.x == left.Current.Left.x &&
                                        x.Current != left.Current.Left)
                                    .ToArray();

                            if (possibleLeft.Any())
                            {
                                Chuzzle toReplace = possibleLeft[Random.Range(0, possibleLeft.Length)];
                                TilesFactory.Instance.ReplaceWithColor(toReplace, left.Color);
                                //        //Debug.Log("Repaint left pair");
                                repainted = true;
                            }
                        }

                        //right?
                        if (right.Current.Right != null && right.Current.Right.Type != CellTypes.Block &&
                            !repainted)
                        {
                            var possibleRight =
                                tiles.Where(
                                    x =>
                                        IsUsual(x) &&
                                        x.Current.x == right.Current.Right.x &&
                                        x.Current != right.Current.Right)
                                    .ToArray();

                            if (possibleRight.Any())
                            {
                                Chuzzle toReplace = possibleRight[Random.Range(0, possibleRight.Length)];
                                TilesFactory.Instance.ReplaceWithColor(
                                    toReplace, right.Color);
                                repainted = true;
                                //          //Debug.Log("Repaint right pair");
                            }
                        }

                        if (repainted)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                //if now pairs
                if (numberOfTries > 5)
                {
                    //create combination guaranteed
                    //get random usual cell
                    var possibleCellsLeftLeft = TilesFactory.Instance.Gamefield.Level.Cells.GetCells().Where(
                        cell =>
                            //слева есть обычная клетка и слева слева тоже есть обычная клетка и в стобце левее левого есть еще одна обычная клетка и в ней находится обычный тайл
                            (cell.Left != null && cell.Left.Type == CellTypes.Usual &&
                             (cell.Left.Left != null && cell.Left.Left.Type == CellTypes.Usual &&
                              TilesFactory.Instance.Gamefield.Level.Cells.GetCells().Count(x => x.x == cell.Left.Left.x && x.Type == CellTypes.Usual) >
                              1) && tiles.FirstOrDefault(chuzzle => chuzzle.Current == cell) != null &&
                             IsUsual(tiles.FirstOrDefault(chuzzle => chuzzle.Current == cell)))
                        ).ToList();
                    if (possibleCellsLeftLeft.Any())
                    {
                        var randomLeftLeft = possibleCellsLeftLeft[Random.Range(0, possibleCellsLeftLeft.Count)];
                        var randomLeftLeftChuzzle = GetChuzzleInCell(randomLeftLeft,
                            tiles);
                        TilesFactory.Instance.ReplaceWithColor(
                            GetChuzzleInCell(randomLeftLeft.Left, tiles),
                            randomLeftLeftChuzzle.Color);
                        var possibleLeftLeftLeft =
                            tiles.Where(
                                x =>
                                    x.Current != randomLeftLeft.Left.Left &&
                                    x.Current.x == randomLeftLeft.Left.Left.x &&
                                    (IsUsual(x) || !IsOrdinaryDestroyable(x)))
                                .ToList();
                        TilesFactory.Instance.ReplaceWithColor(
                            possibleLeftLeftLeft[Random.Range(0, possibleLeftLeftLeft.Count)],
                            randomLeftLeftChuzzle.Color);
                        //Debug.Log("Random left left");
                    }
                    else
                    {
                        //Debug.LogWarning("All our life is a lie");
                        return true;
                    }
                    //check if has free neighbour
                    //repaint to same color
                    //repaint random tile in near row to same color
                }
                else
                {
                    var possibleChuzzles = tiles.Where(IsUsual).ToArray();

                    for (int index = 0; index < possibleChuzzles.Length; index++)
                    {
                        var possibleChuzzle = possibleChuzzles[index];
                        TilesFactory.Instance.ReplaceWithRandom(possibleChuzzle);
                    }
                }
            }
        }


        //if create combination - repaint random
        var combination = FindOnlyOneCombination(tiles);
        if (combination.Any())
        {
            foreach (var chuzzle in combination)
            {
                if (IsUsual(chuzzle))
                {
                    ////Debug.Log("Oops, combination. Repaint");
                    TilesFactory.Instance.ReplaceWithOtherColor(chuzzle);
                    break;
                }
            }
        }
        return false;
    }

    public static List<List<Chuzzle>> SelectedTips(TilesCollection chuzzles, List<Chuzzle> selectedChuzzles, int combinationSize)
    {
        var combinations = new List<List<Chuzzle>>();

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        //find combination
        foreach (var c in selectedChuzzles)
        {
            if (c.IsCheckedForSearch) continue;

            var combination = RecursiveFind(c, new List<Chuzzle>(), chuzzles);

            if (combination.Count() >= combinationSize)
            {
                combinations.Add(combination);
            }
        }

        foreach (var c in chuzzles)
        {
            c.IsCheckedForSearch = false;
        }

        return combinations;
    }
}