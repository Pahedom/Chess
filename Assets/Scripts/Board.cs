using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    public Piece[] pieces;

    public string turnColor;

    public Piece selectedPiece;

    public GameObject piecesSpot;
    public RectTransform whitePiecesTakenSpot;
    public RectTransform blackPiecesTakenSpot;

    Button[,] tileButtons;
    Tile[,] tileTiles;

    bool[,] availableTiles;

    List<RectTransform> whitePiecesTaken;
    List<RectTransform> blackPiecesTaken;

    bool whiteKingRookMove = false;
    bool whiteQueenRookMove = false;
    bool blackKingRookMove = false;
    bool blackQueenRookMove = false;
    bool whiteKingMove = false;
    bool blackKingMove = false;

    public UnityAction OnKingTaken;
    public UnityAction OnRematch;
    public UnityAction OnSwitchTurns;
    public UnityAction OnResign;
    public UnityAction OnDeselectPiece;
    public UnityAction<Vector2> OnPromotion;

    void Start()
    {
        tileButtons = new Button[8, 8];
        tileTiles = new Tile[8, 8];
        availableTiles = new bool[8, 8];
        whitePiecesTaken = new List<RectTransform>();
        blackPiecesTaken = new List<RectTransform>();

        Button[] allTiles = GetComponentsInChildren<Button>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tileButtons[i, j] = allTiles[i * 8 + j];

                tileTiles[i, j] = tileButtons[i, j].gameObject.GetComponent<Tile>();

                tileTiles[i, j].position = new Vector2(i, j);
            }
        }

        Rematch();
    }

    void ArrangePieces(Piece[] thisPieces)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tileTiles[i, j].piece = null;
            }
        }

        Vector2 newPosition = new Vector2();
        
        for (int i = 0; i < thisPieces.Length; i++)
        {
            if (thisPieces[i].type == "Pawn")
            {
                if (thisPieces[i].color == "White")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (tileTiles[j, 1].piece == null)
                        {
                            newPosition = new Vector2(j, 1);
                            break;
                        }
                    }
                }
                else if (thisPieces[i].color == "Black")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (tileTiles[j, 6].piece == null)
                        {
                            newPosition = new Vector2(j, 6);
                            break;
                        }
                    }
                }
            }
            else if (thisPieces[i].type == "Rook")
            {
                if (thisPieces[i].color == "White")
                {
                    if (tileTiles[0, 0].piece == null)
                    {
                        newPosition = new Vector2(0, 0);
                    }
                    else
                    {
                        newPosition = new Vector2(7, 0);
                    }
                }
                else if (thisPieces[i].color == "Black")
                {
                    if (tileTiles[0, 7].piece == null)
                    {
                        newPosition = new Vector2(0, 7);
                    }
                    else
                    {
                        newPosition = new Vector2(7, 7);
                    }
                }
            }
            else if (thisPieces[i].type == "Knight")
            {
                if (thisPieces[i].color == "White")
                {
                    if (tileTiles[1, 0].piece == null)
                    {
                        newPosition = new Vector2(1, 0);
                    }
                    else
                    {
                        newPosition = new Vector2(6, 0);
                    }
                }
                else if (thisPieces[i].color == "Black")
                {
                    if (tileTiles[1, 7].piece == null)
                    {
                        newPosition = new Vector2(1, 7);
                    }
                    else
                    {
                        newPosition = new Vector2(6, 7);
                    }
                }
            }
            else if (thisPieces[i].type == "Bishop")
            {
                if (thisPieces[i].color == "White")
                {
                    if (tileTiles[2, 0].piece == null)
                    {
                        newPosition = new Vector2(2, 0);
                    }
                    else
                    {
                        newPosition = new Vector2(5, 0);
                    }
                }
                else if (thisPieces[i].color == "Black")
                {
                    if (tileTiles[2, 7].piece == null)
                    {
                        newPosition = new Vector2(2, 7);
                    }
                    else
                    {
                        newPosition = new Vector2(5, 7);
                    }
                }
            }
            else if (thisPieces[i].type == "Queen")
            {
                if (thisPieces[i].color == "White")
                {
                    newPosition = new Vector2(3, 0);
                }
                else if (thisPieces[i].color == "Black")
                {
                    newPosition = new Vector2(3, 7);
                }
            }
            else if (thisPieces[i].type == "King")
            {
                if (thisPieces[i].color == "White")
                {
                    newPosition = new Vector2(4, 0);
                }
                else if (thisPieces[i].color == "Black")
                {
                    newPosition = new Vector2(4, 7);
                }
            }

            tileTiles[(int)newPosition.x, (int)newPosition.y].piece = thisPieces[i];
            thisPieces[i].Reposition(newPosition);
        }
    }

    public void SelectPiece(Piece piece)
    {
        FinishMove();

        selectedPiece = piece;

        selectedPiece.SelectPiece(true);

        CheckAvailableMoves(piece);
    }

    void CheckAvailableMoves(Piece piece)
    {
        if (piece.type == "Pawn")
        {
            if (piece.color == "White")
            {
                // Check if pawn isn't on the top edge of the board
                if (piece.position.y < 7)
                {
                    // Check if pawn isn't on the right edge of the board
                    if (piece.position.x < 7)
                    {
                        // Check if pawn can take on right diagonal
                        if (tileTiles[(int)piece.position.x + 1, (int)piece.position.y + 1].piece != null)
                        {
                            if (tileTiles[(int)piece.position.x + 1, (int)piece.position.y + 1].piece.color != turnColor)
                            {
                                availableTiles[(int)piece.position.x + 1, (int)piece.position.y + 1] = true;
                                tileTiles[(int)piece.position.x + 1, (int)piece.position.y + 1].SetAvailableTake(true);
                            }
                        }
                    }
                    // Check if pawn isn't on the left edge of the board
                    if (piece.position.x > 0)
                    {
                        // Check if pawn can take on left diagonal
                        if (tileTiles[(int)piece.position.x - 1, (int)piece.position.y + 1].piece != null)
                        {
                            if (tileTiles[(int)piece.position.x - 1, (int)piece.position.y + 1].piece.color != turnColor)
                            {
                                availableTiles[(int)piece.position.x - 1, (int)piece.position.y + 1] = true;
                                tileTiles[(int)piece.position.x - 1, (int)piece.position.y + 1].SetAvailableTake(true);
                            }
                        }
                    }

                    // Check if 1st tile ahead is free
                    if (tileTiles[(int)piece.position.x, (int)piece.position.y + 1].piece == null)
                    {
                        availableTiles[(int)piece.position.x, (int)piece.position.y + 1] = true;
                        tileTiles[(int)piece.position.x, (int)piece.position.y + 1].SetAvailableTile(true);

                        // Check if pawn is on its inicial position
                        if (piece.position.y == 1)
                        {
                            // Check if 2nd tile ahead is free
                            if (tileTiles[(int)piece.position.x, (int)piece.position.y + 2].piece == null)
                            {
                                availableTiles[(int)piece.position.x, (int)piece.position.y + 2] = true;
                                tileTiles[(int)piece.position.x, (int)piece.position.y + 2].SetAvailableTile(true);
                            }
                        }
                    }
                }
            }
            else if (piece.color == "Black")
            {
                // Same thing for the black pawn
                if (piece.position.y > 0)
                {
                    if (piece.position.x < 7)
                    {
                        if (tileTiles[(int)piece.position.x + 1, (int)piece.position.y - 1].piece != null)
                        {
                            if (tileTiles[(int)piece.position.x + 1, (int)piece.position.y - 1].piece.color != turnColor)
                            {
                                availableTiles[(int)piece.position.x + 1, (int)piece.position.y - 1] = true;
                                tileTiles[(int)piece.position.x + 1, (int)piece.position.y - 1].SetAvailableTake(true);
                            }
                        }
                    }
                    if (piece.position.x > 0)
                    {
                        if (tileTiles[(int)piece.position.x - 1, (int)piece.position.y - 1].piece != null)
                        {
                            if (tileTiles[(int)piece.position.x - 1, (int)piece.position.y - 1].piece.color != turnColor)
                            {
                                availableTiles[(int)piece.position.x - 1, (int)piece.position.y - 1] = true;
                                tileTiles[(int)piece.position.x - 1, (int)piece.position.y - 1].SetAvailableTake(true);
                            }
                        }
                    }
                    
                    if (tileTiles[(int)piece.position.x, (int)piece.position.y - 1].piece == null)
                    {
                        availableTiles[(int)piece.position.x, (int)piece.position.y - 1] = true;
                        tileTiles[(int)piece.position.x, (int)piece.position.y - 1].SetAvailableTile(true);

                        if (piece.position.y == 6)
                        {
                            if (tileTiles[(int)piece.position.x, (int)piece.position.y - 2].piece == null)
                            {
                                availableTiles[(int)piece.position.x, (int)piece.position.y - 2] = true;
                                tileTiles[(int)piece.position.x, (int)piece.position.y - 2].SetAvailableTile(true);
                            }
                        }
                    }
                }
            }
        }
        if (piece.type == "Rook" || piece.type == "Queen")
        {
            // Check tiles above
            for (int i = (int)piece.position.y + 1; i <= 7; i++)
            {
                CheckTile((int)piece.position.x, i);

                if (tileTiles[(int)piece.position.x, i].piece != null)
                {
                    break;
                }
            }
            // Check tiles below
            for (int i = (int)piece.position.y - 1; i >= 0; i--)
            {
                CheckTile((int)piece.position.x, i);

                if (tileTiles[(int)piece.position.x, i].piece != null)
                {
                    break;
                }
            }
            // Check tiles to the right
            for (int i = (int)piece.position.x + 1; i <= 7; i++)
            {
                CheckTile(i, (int)piece.position.y);

                if (tileTiles[i, (int)piece.position.y].piece != null)
                {
                    break;
                }
            }
            // Check tiles to the left
            for (int i = (int)piece.position.x - 1; i >= 0; i--)
            {
                CheckTile(i, (int)piece.position.y);

                if (tileTiles[i, (int)piece.position.y].piece != null)
                {
                    break;
                }
            }
        }
        if (piece.type == "Knight")
        {
            if (piece.position.x < 7)
            {
                if (piece.position.y < 6)
                {
                    CheckTile((int)piece.position.x + 1, (int)piece.position.y + 2);
                }
                if (piece.position.y > 1)
                {
                    CheckTile((int)piece.position.x + 1, (int)piece.position.y - 2);
                }

                if (piece.position.x < 6)
                {
                    if (piece.position.y < 7)
                    {
                        CheckTile((int)piece.position.x + 2, (int)piece.position.y + 1);
                    }
                    if (piece.position.y > 0)
                    {
                        CheckTile((int)piece.position.x + 2, (int)piece.position.y - 1);
                    }
                }
            }
            if (piece.position.x > 0)
            {
                if (piece.position.y < 6)
                {
                    CheckTile((int)piece.position.x - 1, (int)piece.position.y + 2);
                }
                if (piece.position.y > 1)
                {
                    CheckTile((int)piece.position.x - 1, (int)piece.position.y - 2);
                }

                if (piece.position.x > 1)
                {
                    if (piece.position.y < 7)
                    {
                        CheckTile((int)piece.position.x - 2, (int)piece.position.y + 1);
                    }
                    if (piece.position.y > 0)
                    {
                        CheckTile((int)piece.position.x - 2, (int)piece.position.y - 1);
                    }
                }
            }
        }
        if (piece.type == "Bishop" || piece.type == "Queen")
        {
            // Check tiles on top right diagonal
            for (int i = (int)piece.position.x + 1, j = (int)piece.position.y + 1; i <= 7 && j <= 7; i++, j++)
            {
                CheckTile(i, j);

                if (tileTiles[i, j].piece != null)
                {
                    break;
                }
            }
            // Check tiles on bottom right diagonal
            for (int i = (int)piece.position.x + 1, j = (int)piece.position.y - 1; i <= 7 && j >= 0; i++, j--)
            {
                CheckTile(i, j);

                if (tileTiles[i, j].piece != null)
                {
                    break;
                }
            }
            // Check tiles on bottom left diagonal
            for (int i = (int)piece.position.x - 1, j = (int)piece.position.y - 1; i >= 0 && j >= 0; i--, j--)
            {
                CheckTile(i, j);

                if (tileTiles[i, j].piece != null)
                {
                    break;
                }
            }
            // Check tiles on top left diagonal
            for (int i = (int)piece.position.x - 1, j = (int)piece.position.y + 1; i >= 0 && j <= 7; i--, j++)
            {
                CheckTile(i, j);

                if (tileTiles[i, j].piece != null)
                {
                    break;
                }
            }
        }
        if (piece.type == "King")
        {
            if (piece.position.x < 7)
            {
                CheckTile((int)piece.position.x + 1, (int)piece.position.y);

                if (piece.position.y < 7)
                {
                    CheckTile((int)piece.position.x + 1, (int)piece.position.y + 1);
                }

                if (piece.position.y > 0)
                {
                    CheckTile((int)piece.position.x + 1, (int)piece.position.y - 1);
                }
            }
            if (piece.position.x > 0)
            {
                CheckTile((int)piece.position.x - 1, (int)piece.position.y);

                if (piece.position.y < 7)
                {
                    CheckTile((int)piece.position.x - 1, (int)piece.position.y + 1);
                }

                if (piece.position.y > 0)
                {
                    CheckTile((int)piece.position.x - 1, (int)piece.position.y - 1);
                }
            }
            if (piece.position.y < 7)
            {
                CheckTile((int)piece.position.x, (int)piece.position.y + 1);
            }
            if (piece.position.y > 0)
            {
                CheckTile((int)piece.position.x, (int)piece.position.y - 1);
            }

            // Check for castles
            if (piece.color == "White")
            {
                if (piece.position == new Vector2(4, 0) && !whiteKingMove)
                {
                    // Check king's side castle
                    if (tileTiles[5, 0].piece == null && tileTiles[6, 0].piece == null && !whiteKingRookMove)
                    {
                        availableTiles[6, 0] = true;
                        tileTiles[6, 0].SetAvailableTile(true);
                    }
                    // Check queen's side castle
                    if (tileTiles[3, 0].piece == null && tileTiles[2, 0].piece == null && tileTiles[1, 0].piece == null && !whiteQueenRookMove)
                    {
                        availableTiles[2, 0] = true;
                        tileTiles[2, 0].SetAvailableTile(true);
                    }
                }
            }
            if (piece.color == "Black")
            {
                if (piece.position == new Vector2(4, 7) && !blackKingMove)
                {
                    // Check king's side castle
                    if (tileTiles[5, 7].piece == null && tileTiles[6, 7].piece == null && !blackKingRookMove)
                    {
                        availableTiles[6, 7] = true;
                        tileTiles[6, 7].SetAvailableTile(true);
                    }
                    // Check queen's side castle
                    if (tileTiles[3, 7].piece == null && tileTiles[2, 7].piece == null && tileTiles[1, 7].piece == null && !blackQueenRookMove)
                    {
                        availableTiles[2, 7] = true;
                        tileTiles[2, 7].SetAvailableTile(true);
                    }
                }
            }
        }
    }

    void CheckTile(int tileX, int tileY)
    {
        if (tileTiles[tileX, tileY].piece == null)
        {
            availableTiles[tileX, tileY] = true;

            tileTiles[tileX, tileY].SetAvailableTile(true);
        }
        else if (tileTiles[tileX, tileY].piece.color != turnColor)
        {
            availableTiles[tileX, tileY] = true;

            tileTiles[tileX, tileY].SetAvailableTake(true);
        }
    }

    public void MovePiece(Piece piece, Vector2 newPosition, bool switchTurns)
    {
        if (piece == null)
        {
            return;
        }

        // Cancel move if new position isn't available
        if (!availableTiles[(int)newPosition.x, (int)newPosition.y])
        {
            OnDeselectPiece.Invoke();

            return;
        }

        // Check for possible pawn promotion
        if (piece.type == "Pawn")
        {
            if ((piece.color == "White" && newPosition.y == 7f) || (piece.color == "Black" && newPosition.y == 0f))
            {
                OnPromotion.Invoke(newPosition);

                return;
            }
        }

        // Move rook too if castling
        if (piece.type == "King")
        {
            if (piece.color == "White" && piece.position == new Vector2(4, 0))
            {
                if (newPosition == new Vector2(6, 0))
                {
                    MovePiece(tileTiles[7, 0].piece, new Vector2(5, 0), false);
                }
                else if (newPosition == new Vector2(2, 0))
                {
                    MovePiece(tileTiles[0, 0].piece, new Vector2(3, 0), false);
                }
            }
            else if (piece.color == "Black" && piece.position == new Vector2(4, 7))
            {
                if (newPosition == new Vector2(6, 7))
                {
                    MovePiece(tileTiles[7, 7].piece, new Vector2(5, 7), false);
                }
                else if (newPosition == new Vector2(2, 7))
                {
                    MovePiece(tileTiles[0, 7].piece, new Vector2(3, 7), false);
                }
            }
        }

        // Take opponent's piece
        if (tileTiles[(int)newPosition.x, (int)newPosition.y].piece != null)
        {
            if (tileTiles[(int)newPosition.x, (int)newPosition.y].piece.color != turnColor)
            {
                TakePiece(tileTiles[(int)newPosition.x, (int)newPosition.y].piece);
            }
        }

        // Check for rook or king's first move (so they can't castle anymore)
        if (piece.type == "Rook")
        {
            if (piece.color == "White")
            {
                if (piece.position == new Vector2(0, 0))
                {
                    whiteQueenRookMove = true;
                }
                else if (piece.position == new Vector2(7, 0))
                {
                    whiteKingRookMove = true;
                }
            }
            else
            {
                if (piece.position == new Vector2(0, 7))
                {
                    blackQueenRookMove = true;
                }
                else if (piece.position == new Vector2(7, 7))
                {
                    blackKingRookMove = true;
                }
            }
        }
        else if (piece.type == "King")
        {
            if (piece.color == "White" && piece.position == new Vector2(0, 4))
            {
                whiteKingMove = true;
            }
            else if (piece.color == "Black" && piece.position == new Vector2(7, 4))
            {
                blackKingMove = true;
            }
        }

        // Remove piece from its previous position
        tileTiles[(int)piece.position.x, (int)piece.position.y].piece = null;

        // Move piece to its new position
        tileTiles[(int)newPosition.x, (int)newPosition.y].piece = piece;

        // Reposition piece on the board
        piece.Reposition(newPosition);

        FinishMove();

        // Switch turns
        if (switchTurns)
        {
            if (turnColor == "White")
            {
                turnColor = "Black";
            }
            else
            {
                turnColor = "White";
            }

            OnSwitchTurns.Invoke();
        }
    }

    void TakePiece(Piece piece)
    {
        RectTransform pieceTransform = piece.GetComponent<RectTransform>();

        pieceTransform.sizeDelta = new Vector2(51.5f, 51.5f);

        if (piece.color == "White")
        {
            whitePiecesTaken.Add(pieceTransform);

            piece.transform.SetParent(whitePiecesTakenSpot);
            pieceTransform.localPosition = new Vector2(35f * (whitePiecesTaken.Count - 0.5f), 0f);
        }
        else
        {
            blackPiecesTaken.Add(pieceTransform);

            piece.transform.SetParent(blackPiecesTakenSpot);
            pieceTransform.localPosition = new Vector2(35f * (blackPiecesTaken.Count - 0.5f), 0f);
        }

        if (piece.type == "King")
        {
            OnKingTaken.Invoke();
        }
    }

    public void FinishMove()
    {
        if (selectedPiece != null)
        {
            selectedPiece.SelectPiece(false);
            selectedPiece = null;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                availableTiles[i, j] = false;

                tileTiles[i, j].SetAvailableTile(false);
                tileTiles[i, j].SetAvailableTake(false);
            }
        }
    }

    public void Rematch()
    {
        OnRematch.Invoke();

        string[] checkTypes = { "Queen", "Knight", "Rook", "Bishop" };
        int maxCount;
        List<Piece> piecesToCheck = new List<Piece>();

        for (int i = 0; i < 4; i++)
        {
            piecesToCheck.Clear();

            for (int j = 0; j < 32; j++)
            {
                if (pieces[j].type == checkTypes[i])
                {
                    piecesToCheck.Add(pieces[j]);
                }
            }

            maxCount = 4;

            if (i == 0)
            {
                maxCount = 2;
            }

            for (int j = maxCount; j < piecesToCheck.Count; j++)
            {
                piecesToCheck[j].type = "Pawn";
            }
        }

        for (int i = 0; i < 32; i++)
        {
            Debug.Log(pieces[i].type);
        }

        // Remove pieces from the taken pieces spot
        RectTransform pieceTransform;
        Vector2 originalSize = new Vector2(103, 103);
        for (int i = 0; i < 32; i++)
        {
            pieceTransform = pieces[i].GetComponent<RectTransform>();
            pieceTransform.sizeDelta = originalSize;

            pieces[i].transform.SetParent(piecesSpot.transform);
        }

        ArrangePieces(pieces);

        whiteKingRookMove = false;
        whiteQueenRookMove = false;
        blackKingRookMove = false;
        blackQueenRookMove = false;
        whiteKingMove = false;
        blackKingMove = false;

        turnColor = "White";

        OnSwitchTurns.Invoke();
    }

    public void Resign()
    {
        FinishMove();

        OnResign.Invoke();
    }

    public void Promote(string newType, Vector2 newPosition)
    {
        selectedPiece.type = newType;

        MovePiece(selectedPiece, newPosition, true);
    }

    public Piece GetSelectedPiece()
    {
        return selectedPiece;
    }
}
