#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private bool IsSinglePlayer = false;
    private bool IsGaming = false;
    private bool IsPlayer1Turn = false;
    private int MarkCount;

    public GameObject ModeSelectWindow;
    public GameObject MainGameWindow;
    public GameObject ResultWindow;

    public List<Text> TurnIndicators = new List<Text>();
    public List<Text> BoardIndicators = new List<Text>();
    public List<Text> ResultIndicators = new List<Text>();

    void Start()
    {
        IsGaming = false;

        HideAllWindows();

        ModeSelectWindow.SetActive(true);
    }

    public void OnSinglePlayer()
    {
        IsSinglePlayer = true;

        StartGame();
    }

    public void OnVS()
    {
        IsSinglePlayer = false;

        StartGame();
    }

    public void OnExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void HideAllWindows()
    {
        ModeSelectWindow.SetActive(false);
        MainGameWindow.SetActive(false);
        ResultWindow.SetActive(false);
    }

    void StartGame()
    {
        HideAllWindows();

        foreach(Text TextObject in TurnIndicators)
            TextObject.gameObject.SetActive(false);

        foreach (Text BoardObject in BoardIndicators)
            BoardObject.text = "";

        IsGaming = true;
        IsPlayer1Turn = true;
        MarkCount = 0;

        UpdateTurnIndicator();

        MainGameWindow.SetActive(true);
    }

    void UpdateTurnIndicator()
    {
        foreach (Text TextObject in TurnIndicators)
            TextObject.gameObject.SetActive(false);

        int index = 0;

        if (IsSinglePlayer)
            index = IsPlayer1Turn ? 0 : 1;
        else
            index = IsPlayer1Turn ? 2 : 3;

        TurnIndicators[index].gameObject.SetActive(true);
    }

    public void OnClickBoard(int index)
    {
        if (!IsGaming)
            return;

        if (BoardIndicators[index].text != "")
            return;

        if(IsSinglePlayer)
        {
            if (!IsPlayer1Turn)
                return;

            BoardIndicators[index].text = "X";

            MarkCount++;
        }
        else
        {
            BoardIndicators[index].text = IsPlayer1Turn ? "X" : "O";

            MarkCount++;
        }

        if (CheckResult() == 0 && IsSinglePlayer)
            StartCoroutine(AIRoutine());
    }

    IEnumerator AIRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        List<int> emptys = new List<int>();

        for(int i = 0; i < 9; i ++)
        {
            if (BoardIndicators[i].text == "")
                emptys.Add(i);
        }

        int index = Random.Range(0, emptys.Count);

        BoardIndicators[emptys[index]].text = "O";

        MarkCount++;

        yield return new WaitForSeconds(0.5f);

        CheckResult();
    }

    int CheckResult()
    {
        int nResult = 0;

        int[,] ids = new int[,]
        {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 },
            { 2, 4, 6 }
        };

        string xo = IsPlayer1Turn ? "X" : "O";

        for(int i = 0; i < 8; i ++)
        {
            nResult = 1;

            for(int j = 0; j < 3; j ++)
            {
                if(BoardIndicators[ids[i, j]].text != xo)
                {
                    nResult = 0;
                    break;
                }
            }

            if (nResult == 1)
                break;
        }

        if (nResult == 0 && MarkCount == 9)
            nResult = 2;

        if (nResult == 0) 
        {
            IsPlayer1Turn = !IsPlayer1Turn;

            UpdateTurnIndicator();
        }
        else
        {
            IsGaming = false;

            HideAllWindows();

            foreach (Text TextObject in ResultIndicators)
                TextObject.gameObject.SetActive(false);

            int index = 4;

            if (nResult == 1)
            {
                if (IsSinglePlayer)
                    index = IsPlayer1Turn ? 0 : 1;
                else
                    index = IsPlayer1Turn ? 2 : 3;
            }

            ResultIndicators[index].gameObject.SetActive(true);

            ResultWindow.SetActive(true);
        }

        return nResult;
    }

    public void OnBack()
    {
        IsGaming = false;

        HideAllWindows();

        ModeSelectWindow.SetActive(true);
    }
}
