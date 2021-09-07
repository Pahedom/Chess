using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    public Piece piece;

    public Board board;

    public Vector2 position;

    public GameObject availableTileSprite;
    public GameObject availableTakeSprite;

    public void SelectTile()
    {
        if (piece != null)
        {
            if (piece.color == board.turnColor)
            {
                board.SelectPiece(piece);
                return;
            }
        }

        board.MovePiece(board.selectedPiece, position, true);
    }

    public void SetAvailableTile(bool set)
    {
        availableTileSprite.SetActive(set);
    }

    public void SetAvailableTake(bool set)
    {
        availableTakeSprite.SetActive(set);
    }
}
