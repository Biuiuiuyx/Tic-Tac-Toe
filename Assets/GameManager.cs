using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;  // 添加这一行

public class GameManager : MonoBehaviour
{
    public Button[] buttons;
    public Text gameOverText;
    public Button restartButton;

    private string currentPlayer;
    private string[] boardState;

    void Start()
    {
        currentPlayer = "X";
        boardState = new string[9];
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    void OnButtonClick(int index)
    {
        if (boardState[index] == null && currentPlayer == "X")
        {
            boardState[index] = currentPlayer;
            buttons[index].GetComponentInChildren<Text>().text = currentPlayer;
            if (CheckWin())
            {
                EndGame(currentPlayer + " Wins!");
            }
            else if (CheckDraw())
            {
                EndGame("Draw!");
            }
            else
            {
                currentPlayer = "O";
                Invoke("AIMove", 1f); // AI在1秒后执行
            }
        }
    }

    void AIMove()
    {
        int index = GetRandomEmptyIndex();
        if (index != -1)
        {
            boardState[index] = currentPlayer;
            buttons[index].GetComponentInChildren<Text>().text = currentPlayer;
            if (CheckWin())
            {
                EndGame(currentPlayer + " Wins!");
            }
            else if (CheckDraw())
            {
                EndGame("Draw!");
            }
            else
            {
                currentPlayer = "X";
            }
        }
    }

    int GetRandomEmptyIndex()
    {
        List<int> emptyIndices = new List<int>();
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] == null)
            {
                emptyIndices.Add(i);
            }
        }
        if (emptyIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyIndices.Count);
            return emptyIndices[randomIndex];
        }
        return -1;
    }

    bool CheckWin()
    {
        int[,] winCombinations = new int[,]
        {
            {0, 1, 2},
            {3, 4, 5},
            {6, 7, 8},
            {0, 3, 6},
            {1, 4, 7},
            {2, 5, 8},
            {0, 4, 8},
            {2, 4, 6}
        };

        for (int i = 0; i < winCombinations.GetLength(0); i++)
        {
            if (boardState[winCombinations[i, 0]] != null &&
                boardState[winCombinations[i, 0]] == boardState[winCombinations[i, 1]] &&
                boardState[winCombinations[i, 1]] == boardState[winCombinations[i, 2]])
            {
                return true;
            }
        }
        return false;
    }

    bool CheckDraw()
    {
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    void EndGame(string result)
    {
        gameOverText.text = result;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }

    void RestartGame()
    {
        currentPlayer = "X";
        boardState = new string[9];
        foreach (var button in buttons)
        {
            button.GetComponentInChildren<Text>().text = "";
            button.interactable = true;
        }
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }
}
