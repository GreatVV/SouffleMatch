using UnityEngine;

namespace Assets.Game.Gameplay.Chuzzles.Utils
{
    public class TipArrow : MonoBehaviour
    {
        private Chuzzle chuzzle;

        [SerializeField] private float speed = 4f;

        private float timePassed;

        public Chuzzle Chuzzle
        {
            get { return chuzzle; }
            set
            {
                if (chuzzle == value)
                {
                    return;
                }
                timePassed = 0f;

                if (chuzzle)
                {
                    chuzzle.Died -= OnDied;
                }
                chuzzle = value;

                gameObject.SetActive(chuzzle);
                //Debug.Log("Chuzzle for tip: " + chuzzle);

                if (chuzzle)
                {
                    chuzzle.Died += OnDied;
                }
            }
        }

        private void OnDied(Chuzzle deadChuzzle)
        {
            Chuzzle = null;
        }

        public void UpdateState()
        {
            if (Chuzzle && !Chuzzle.IsDead)
            {
                transform.position = Chuzzle.transform.position;
                timePassed += Time.deltaTime*speed;
                transform.localScale = Vector3.one*(0.75f + Mathf.Sin(timePassed)/4);
                if (timePassed > 2*Mathf.PI)
                {
                    timePassed = 0f;
                }
            }
       
        }
    }
}