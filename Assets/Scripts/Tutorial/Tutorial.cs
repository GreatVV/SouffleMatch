using TutorialSpace;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;
    public static bool isActive;

    public Chuzzle takeableChuzzle;
    public Cell targetCell;
    [SerializeField] private TutorialPage startPage;

    private TutorialPage currentPage;

    public void Begin()
    {
        isActive = true;
        currentPage = startPage;
        currentPage.End += OnPageEnd;
        currentPage.Show();
    }

    public void End()
    {
        currentPage.Hide();
        currentPage.End -= OnPageEnd;
        isActive = false;
    }

    private void OnPageEnd(TutorialPage page)
    {
        page.End -= OnPageEnd;
        page.Hide();
        if (page.NextPage)
        {
            currentPage = page.NextPage;
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
        instance = this;
    }

    public bool CanTakeOnlyThisChuzzle(Chuzzle currentChuzzle)
    {
        return takeableChuzzle == currentChuzzle;
    }

    public bool IsTargetCell(Cell cell)
    {
        return cell == targetCell;
    }
}