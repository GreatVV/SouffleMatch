using System.Linq;
using TutorialSpace;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public void Begin()
    {
        currentPage = startPage;
        currentPage.Show();
    }

    public void End()
    {
        currentPage.Hide();
        currentPage.End -= OnPageEnd;
    }

    [SerializeField]
    private TutorialPage startPage;

    private TutorialPage currentPage;
    public int[] blockedY = new int[0];
    public int[] blockedX = new int[0];
    public static Tutorial instance;

    void OnPageEnd(TutorialPage page)
    {
        page.End -= OnPageEnd;
        page.Hide();
        if (page.NextPage)
        {
            currentPage = page.NextPage;
            currentPage.End += OnPageEnd;
            currentPage.Show();
        }
    }

    public bool CantMoveThisChuzzle(Chuzzle currentChuzzle)
    {
        return blockedX.Any(x => x == currentChuzzle.Current.x) ||
               blockedY.Any(y => y == currentChuzzle.Current.y);
    }

    public void Awake()
    {
        instance = this;
    }

    public bool CantMoveThisCell(Cell cell)
    {
        return blockedX.Any(x => x == cell.x) ||
               blockedY.Any(y => y == cell.y);
    }
}