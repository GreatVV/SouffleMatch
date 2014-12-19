using System.Collections.Generic;

namespace Assets.Game.Gameplay.Chuzzles.PowerUps
{
    public class VerticalLineChuzzle : Chuzzle, IPowerUp
    {
        #region IPowerUp Members

        public IEnumerable<Chuzzle> ToDestroy
        {
            get
            {
                //return PowerUpDestroyManager.GetColumn(Current.x);
                return new List<Chuzzle>();
            }
        }

        #endregion

        #region Events Subscribers

        protected override void OnAwake()
        {
        }

        #endregion
    }
}