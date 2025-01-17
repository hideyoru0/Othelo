using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    public int x, y;
    public GameObject black;
    public GameObject white;
    private GameObject currentPiece;

    public void HighlightCell(bool highlightActive) 
    { 
        highlight.SetActive(highlightActive); 
    }

    public void PlacePiece(bool isBlack)
    {
        if (isBlack)
        {
            if (currentPiece != null) Destroy(currentPiece);
            currentPiece = Instantiate(black, transform.position, Quaternion.identity, transform);
        }
        else
        {
            if (currentPiece != null) Destroy(currentPiece);
            currentPiece = Instantiate(white, transform.position, Quaternion.identity, transform);
        }
    }
    
    public bool HasPiece()
    {
        return currentPiece != null;
    }

    public bool IsBlack()
    {
        if (currentPiece == null) return false;
        return currentPiece.GetComponent<SpriteRenderer>().color == Color.black;
    }

    // 유효한 수인지 확인
    public bool IsValidMove(bool isBlackTurn)
    {
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        
        if (HasPiece()) return false;

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

            int nx = x + dx;
            int ny = y + dy;

            bool hasOppenent = false;
            
            while (boardManager.GetCellAt(nx, ny) != null)
            {
                BoardCell nextCell = boardManager.GetCellAt(nx, ny);

                if (!nextCell.HasPiece()) break;
                
                if (nextCell.IsBlack() != isBlackTurn)
                {
                    hasOppenent = true;
                    nx += dx;
                    ny += dy;
                }
                else
                {
                    if (hasOppenent)
                    {
                        return true;
                    }
                    break;
                }
            }
        }

        return false;
    }
}
