using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject boardCell;
    private BoardCell[,] boardCells = new BoardCell[8, 8];
    [SerializeField] Transform cellSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        float startX = -3.5f;
        float startY = -3.5f;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3 position = new Vector3(startX + x, startY + y, 0);
                GameObject cellObj = Instantiate(boardCell, position, Quaternion.identity);
                cellObj.transform.SetParent(cellSpawn, true);
                BoardCell cell = cellObj.GetComponent<BoardCell>();

                cell.x = x;
                cell.y = y;

                boardCells[x, y] = cell;
            }
        }

        boardCells[3, 3].PlacePiece(true);
        boardCells[4, 4].PlacePiece(true);
        boardCells[3, 4].PlacePiece(false);
        boardCells[4, 3].PlacePiece(false);
    }

    public BoardCell GetCellAt(int x, int y)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return null;
        }

        return boardCells[x, y];
    }

    // 특정 플레이어가 둘 수 있는 유효한 수가 있는지 확인
    public bool HasValidMoves(bool isBlack)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                BoardCell cell = boardCells[x, y];
                if (!cell.HasPiece() && cell.IsValidMove(isBlack))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
