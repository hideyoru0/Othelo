using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [SerializeField] BoardManager boardManager;
    [SerializeField] GameObject finishUI;
    [SerializeField] GameObject wturnUI;
    [SerializeField] GameObject bturnUI;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] TextMeshProUGUI whiteText;
    [SerializeField] TextMeshProUGUI blackText;

    bool isBlackTurn = true;
    bool isEnded = false;
    int whiteCount = 0;
    int blackCount = 0;

    private void Awake()
    {
        bturnUI.SetActive(true);
        finishUI.SetActive(true);
    }

    private void Start()
    {
        HighlightValidMoves();
    }

    private void Update()
    {
        if (isEnded) return;

        if (CheckGameOver())
        {
            EndGame();
        }
        else if (CheckTurnOver())
        {
            ChangeTurn();
        }
    }

    bool CheckGameOver()
    {
        bool blackHasMove = boardManager.HasValidMoves(true);
        bool whiteHasMove = boardManager.HasValidMoves(false);

        if (!blackHasMove && !whiteHasMove)
        {
            return true;
        }
        return false;
    }

    bool CheckTurnOver()
    {
        bool hasMove = boardManager.HasValidMoves(isBlackTurn);

        if (!hasMove)
        {
            isBlackTurn = !isBlackTurn;
            if (!boardManager.HasValidMoves(isBlackTurn))
            {
                return true;
            }
            else
            {
                UpdateTurnUI();
                HighlightValidMoves();
                return false;
            }
        }
        return false;
    }

    void ChangeTurn()
    {
        isBlackTurn = !isBlackTurn;
        UpdateTurnUI();
        HighlightValidMoves();
    }

    void UpdateTurnUI()
    {
        if (isBlackTurn)
        {
            wturnUI.SetActive(false);
            bturnUI.SetActive(true);
        }
        else
        {
            wturnUI.SetActive(true);
            bturnUI.SetActive(false);
        }
    }

    void HighlightValidMoves()
    {
        var allCells = FindObjectsOfType<BoardCell>();

        foreach (var cell in allCells)
        {
            bool isValidMove = cell.IsValidMove(isBlackTurn);
            cell.HighlightCell(isValidMove);
        }
    }

    void UpdateScore()
    {
        int newBlackCount = 0;
        int newWhiteCount = 0;

        var allPiece = FindObjectsOfType<BoardCell>();
        foreach (var piece in allPiece)
        {
            if (piece.HasPiece())
            {
                if (piece.IsBlack())
                {
                    newBlackCount++;
                }
                else
                {
                    newWhiteCount++;
                }
            }
        }

        blackCount = newBlackCount;
        whiteCount = newWhiteCount;

        blackText.text = blackCount.ToString("000");
        whiteText.text = whiteCount.ToString("000");

        Debug.Log($"black = {blackCount}, white = {whiteCount}");
    }


    void EndGame()
    {
        isEnded = true;
        UpdateScore();

        if (blackCount > whiteCount)
        {
            Debug.Log("black win");
            finishUI.SetActive(true);
            winText.text = ("Black win");
        }
        else if (blackCount < whiteCount)
        {
            Debug.Log("white win");
            finishUI.SetActive(true);
            winText.text = ("White win");
        }
        else
        {
            Debug.Log("Draw");
            finishUI.SetActive(true);
            winText.text = ("Draw");
        }
    }

    public void GetScore()
    {
        var allPiece = FindObjectsOfType<BoardCell>();
        foreach (var piece in allPiece)
        {
            if (piece.IsBlack())
            {
                blackCount++;
            }
            else
            {
                whiteCount++;
            }
        }
        blackText.text = blackCount.ToString("000");
        whiteText.text = whiteCount.ToString("000");
        Debug.Log($"black = {blackCount}, white ={whiteCount}");
    }

    public void OnCellClicked(BoardCell cell)
    {
        if (cell.HasPiece())
        {
            Debug.Log("Cell is already occupied");
            return;
        }

        if (cell.IsValidMove(isBlackTurn))
        {
            cell.PlacePiece(isBlackTurn);
            FlipPieces(cell.x, cell.y, isBlackTurn);

            UpdateScore();
            
            ChangeTurn();
        }
    }

    bool CanPlacePiece(int x, int y, bool isBlack)
    {
        int[,] directions =
        {
            {-1, -1}, {-1, 0}, {-1, 1},
            {0, -1},           {0, 1},
            {1, -1}, {1, 0}, {1, 1}
        };

        int directionCount = directions.GetLength(0);

        for (int i = 0; i < directionCount; i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];

            if (CanFlipinDirection(x, y, dx, dy, isBlack))
            {
                return true;
            }
        }
        return false;
    }

    bool CanFlipinDirection(int x, int y, int dx, int dy, bool isBlack)
    {
        int nx = x + dx;
        int ny = y + dy;

        bool hasOppenent = false;

        while (boardManager.GetCellAt(nx, ny) != null)
        {
            BoardCell nextCell = boardManager.GetCellAt(nx, ny);

            if (!nextCell.HasPiece())
            {
                return false;
            }

            if (nextCell.IsBlack() != isBlack)
            {
                Debug.Log($"check 2 = {isBlackTurn}");
                hasOppenent = true;
                nx += dx;
                ny += dy;
            }
            else
            {
                Debug.Log($"check 1 = {isBlackTurn}");
                return hasOppenent;
            }
        }
        return false;
    }

    void FlipPieces(int x, int y, bool isBlack)
    {
        int[,] directions =
        {
            {-1, -1}, {-1, 0}, {-1, 1},
            {0, -1},           {0, 1},
            {1, -1}, {1, 0}, {1, 1}
        };

        int directionCount = directions.GetLength(0);

        for (int i = 0; i < directionCount; i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];

            if (CanFlipinDirection(x, y, dx, dy, isBlack))
            {
                FlipInDirection(x, y, dx, dy, isBlack);
            }
        }
    }

    void FlipInDirection(int x, int y, int dx, int dy, bool isBlack)
    {
        int nx = x + dx;
        int ny = y + dy;

        while (boardManager.GetCellAt(nx, ny) != null)
        {
            BoardCell nextCell = boardManager.GetCellAt(nx, ny);

            if (!nextCell.HasPiece() || nextCell.IsBlack() == isBlack) break;

            nextCell.PlacePiece(isBlack);
            nx += dx;
            ny += dy;
        }

        UpdateScore();
    }
}
