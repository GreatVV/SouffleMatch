using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class PowerUpBehaviour {

    //TODO check for if start chuzzle contains in tilesToKill
	public static void ApplyPowerUp(this Gamefield gamefield, List<Chuzzle> tilesToKill,  Chuzzle chuzzle)
    {
        if (chuzzle.PowerType == PowerType.Usual)
        {
            Debug.LogError("Try apply power up for non power type chuzzle: "+chuzzle.ToString());
            return;
        }        

        if (chuzzle.PowerType == PowerType.HorizontalLine)
        {
            var horizontalChuzzles = gamefield.Level.ActiveChuzzles.Where(x => x.Current.y == chuzzle.Current.y && x.PowerType == PowerType.Usual);            
            tilesToKill.AddUniqRange(horizontalChuzzles);             
        }

        if (chuzzle.PowerType == PowerType.VerticalLine)
        {
            var vertical = gamefield.Level.ActiveChuzzles.Where(x => x.Current.x == chuzzle.Current.x && x.PowerType == PowerType.Usual);
            tilesToKill.AddUniqRange(vertical);    
        }

        if (chuzzle.PowerType == PowerType.Bomb)
        {
            var square = gamefield.Level.ActiveChuzzles.Where(x => (x.Current.x == chuzzle.Current.x - 1 || x.Current.x == chuzzle.Current.x + 1 || x.Current.x == chuzzle.Current.x) && (x.Current.y == chuzzle.Current.y - 1 || x.Current.y == chuzzle.Current.y || x.Current.y == chuzzle.Current.y + 1) && x.PowerType == PowerType.Usual);
            tilesToKill.AddUniqRange(square);                    
        }
    }
}
