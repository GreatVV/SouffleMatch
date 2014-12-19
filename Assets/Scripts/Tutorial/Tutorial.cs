using Assets.Game.Gameplay.Cells;
using Assets.Game.Gameplay.Chuzzles;
using UnityEngine;

namespace Assets.Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        public static Tutorial Instance { get; set; }
        public static bool isActive;

        public Chuzzle takeableChuzzle;
        public Cell targetCell;
        [SerializeField] private TutorialPage startPage;

        private TutorialPage currentPage;

        public static void Begin()
        {
            isActive = true;
            Instance.currentPage = Instance.startPage;
            Instance.currentPage.End -= Instance.OnPageEnd;
            Instance.currentPage.End += Instance.OnPageEnd;
            Instance.currentPage.Show();
        }

        public static void End()
        {
            if (Instance.currentPage)
            {
                Instance.currentPage.Hide();
                Instance.currentPage.End -= Instance.OnPageEnd;
            }
            isActive = false;
            Destroy(Instance.gameObject);
        }

        private void OnPageEnd(TutorialPage page)
        {
            page.End -= OnPageEnd;
            page.Hide();
            if (page.NextPage)
            {
                currentPage = page.NextPage;
                currentPage.End -= OnPageEnd;
                currentPage.End += OnPageEnd;
                currentPage.Show();
            }
            else
            {
                End();
            }
        }

        public void Awake()
        {
            if (Instance)
            {
                Debug.Log("Instance already created");
                return;
            }

            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                isActive = false;
            }
        }

        public static bool CanTakeOnlyThisChuzzle(Chuzzle currentChuzzle)
        {
            if (!isActive)
            {
                return false;
            }

            return Instance.takeableChuzzle == currentChuzzle;
        }

        public bool IsTargetCell(Cell cell)
        {
            return cell == targetCell;
        }

        public static void SetActive(bool active)
        {
            if (Instance.currentPage)
            {
                if (active)
                {
                    Instance.currentPage.Show();
                }
                else
                {
                    Instance.currentPage.Hide();
                }
            }
        }
    }
}