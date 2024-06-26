using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;  

public class GameManager : MonoBehaviour
{
    public Button[] buttons;
    public Text gameOverText;
    public Button restartButton;
    public Toggle difficultyToggle;

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
        difficultyToggle.isOn = false;  // 默认关闭
        difficultyToggle.onValueChanged.AddListener(ToggleValueChanged);
    }

    void ToggleValueChanged(bool state)
    {
        if (state)
        {
            Debug.Log("使用高级AI");
        }
        else
        {
            Debug.Log("使用简单AI");
        }
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
        difficultyToggle.gameObject.SetActive(false);

    }

    int EvaluateBoard(string[] board)
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
            if (board[winCombinations[i, 0]] != null &&
                board[winCombinations[i, 0]] == board[winCombinations[i, 1]] &&
                board[winCombinations[i, 1]] == board[winCombinations[i, 2]])
            {
                if (board[winCombinations[i, 0]] == "X")
                    return -10;
                else if (board[winCombinations[i, 0]] == "O")
                    return 10;
            }
        }
        return 0;
    }

    int Minimax(string[] board, int depth, bool isMaximizing)
    {
        int score = EvaluateBoard(board);

        if (score == 10)
            return score - depth;
        if (score == -10)
            return score + depth;
        if (CheckDraw())
            return 0;

        if (isMaximizing)
        {
            int best = int.MinValue;

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == null)
                {
                    board[i] = "O";
                    best = Mathf.Max(best, Minimax(board, depth + 1, false));
                    board[i] = null;
                }
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == null)
                {
                    board[i] = "X";
                    best = Mathf.Min(best, Minimax(board, depth + 1, true));
                    board[i] = null;
                }
            }
            return best;
        }
    }

    void AIMove()
    {
        int bestMove = -1;

        if (difficultyToggle.isOn)
        {
            // 使用高级AI (Minimax算法)
            int bestValue = int.MinValue;

            for (int i = 0; i < boardState.Length; i++)
            {
                if (boardState[i] == null)
                {
                    boardState[i] = "O";
                    int moveValue = Minimax(boardState, 0, false);
                    boardState[i] = null;

                    if (moveValue > bestValue)
                    {
                        bestMove = i;
                        bestValue = moveValue;
                    }
                }
            }
        }
        else
        {
            // 使用简单AI (随机选择)
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
                bestMove = emptyIndices[Random.Range(0, emptyIndices.Count)];
            }
        }

        if (bestMove != -1)
        {
            boardState[bestMove] = currentPlayer;
            buttons[bestMove].GetComponentInChildren<Text>().text = currentPlayer;
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
        difficultyToggle.gameObject.SetActive(true);

    }
}
