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
    public static int[] blockedRows;
    public static int[] blockedColumns;

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

    
}