using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

namespace Assets.Game.Gameplay.Chuzzles.Types
{
    public class InvaderChuzzle : Chuzzle
    {
        public static List<InvaderChuzzle> AllInvaderChuzzles = new List<InvaderChuzzle>();
        public static int MaxInvadersOnLevel = 20;

        protected override void OnAwake()
        {
            if (AllInvaderChuzzles.Contains(this))
            {
                Debug.LogError("Already contains this invader: " + this);
            }
            else
            {
                AllInvaderChuzzles.Add(this);
            }
        }

        public override string ToString()
        {
            return string.Format("Invader chuzzle: Current: ({0},{1}); Real: ({2},{3})", Current.X, Current.Y, Real.X, Real.Y);
        }

        public override void Destroy(bool needCreateNew, bool withAnimation = true, bool isReplacingOnDeath = false)
        {
        
            AllInvaderChuzzles.Remove(this);
        
            base.Destroy(needCreateNew, withAnimation, isReplacingOnDeath);
        }

        void OnDestroy()
        {
            if (AllInvaderChuzzles.Contains(this))
            {
                //Debug.LogError("Contain this chuzzle: "+this);
                AllInvaderChuzzles.Remove(this);
            }
        }


        public static void OnCombinationDestroyed(IEnumerable<Chuzzle> chuzzles)
        {
            var invadersNear = new List<Chuzzle>();

            foreach (var chuzzle in chuzzles)
            {
                foreach (var x in AllInvaderChuzzles)
                {
                    if ((x.Current.Left == chuzzle.Real || x.Current.Right == chuzzle.Real || x.Current.Top == chuzzle.Real ||
                         x.Current.Bottom == chuzzle.Real) && !invadersNear.Contains(x))
                    {
                        invadersNear.Add(x);
                    }
                }
            }
            // Gamefield.InvaderWasDestroyed |= invadersNear.Any();

            while (invadersNear.Any())
            {   
                invadersNear.First().Destroy(true);
                invadersNear.RemoveAt(0);
            }
        }

        public static void Populate(Gamefield gamefield)
        {
            if (!AllInvaderChuzzles.Any() || AllInvaderChuzzles.Count == MaxInvadersOnLevel)
            {
                return;
            }

            var search = new PrimeSearch(AllInvaderChuzzles.Count);
            int p;
            while ((p = search.GetNext()) != -1)
            {
                var currentInvader = AllInvaderChuzzles[p];

                var targetTile =
                    gamefield.Level.Chuzzles.GetTiles(x=>x is ColorChuzzle).FirstOrDefault(
                                                                                           x =>
                                                                                           (x.Current == currentInvader.Current.Left || x.Current == currentInvader.Current.Right ||
                                                                                            x.Current == currentInvader.Current.Top || x.Current == currentInvader.Current.Bottom));

                if (targetTile != null)
                {
                    Instance.TilesFactory.CreateInvader(targetTile.Current);
                    targetTile.Destroy(false,true,true);
                    break;
                }
            }

            //  var listInvaders = AllInvaderChuzzles.Aggregate("", (current, allInvaderChuzzle) => current + (allInvaderChuzzle + "\n"));
            //  Debug.Log("Invaders: \n"+listInvaders);
        }
    }
}