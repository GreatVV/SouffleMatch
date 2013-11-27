using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

[Serializable]
public class TargetPlaceGameMode : GameMode
{
    public List<IntVector2> CurrentPlaceCoordinates = new List<IntVector2>();
    public List<IntVector2> PlaceCoordinates = new List<IntVector2>();

    public TargetPlaceGameMode(GameModeDescription description) : base(description)
    {
    }

    public override void OnDestroy()
    {
        Gamefield.TileDestroyed -= OnTileDestroyed;
    }

    private void OnTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (PlaceCoordinates.Count == 0)
        {
            return;
        }
       

        var place =
            CurrentPlaceCoordinates.FirstOrDefault(
                x => x.x == destroyedChuzzle.Current.x && x.y == destroyedChuzzle.Current.y);
        if (place != null)
        {
            NGUITools.Destroy(destroyedChuzzle.Current.PlaceSprite);
            CurrentPlaceCoordinates.Remove(place);
        }

        if (CurrentPlaceCoordinates.Count == 0)
        {
            IsWin = true;
        }
    }

    public override void HumanTurn()
    {
        SpendTurn();
    }

    protected override void OnInit()
    {
        Gamefield.TileDestroyed -= OnTileDestroyed;
        Gamefield.TileDestroyed += OnTileDestroyed;

        PlaceCoordinates.Clear();
        var placeCell = Gamefield.Level.Cells.Where(x => x.CreationType == CreationType.Place);
        var enumerable = placeCell as Cell[] ?? placeCell.ToArray();
        Debug.Log("Now of cells: "+enumerable.Count());
        foreach (var cell in enumerable)
        {
            PlaceCoordinates.Add(new IntVector2(cell.x, cell.y));
        }
        OnReset();
    }

    public override void OnReset()
    {
        CurrentPlaceCoordinates.Clear();
        CurrentPlaceCoordinates.AddRange(PlaceCoordinates);
    }

    public override string ToString()
    {
        return string.Format("You should clear all {0} cells", PlaceCoordinates.Count);
    }
}