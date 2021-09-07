using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Board board;
    
    public GameObject winScreen;

    public Text winner;

    public GameObject resignWhite;
    public GameObject resignBlack;

    public GameObject whitePromotePopup;
    public GameObject blackPromotePopup;

    public Sprite whiteQueen;
    public Sprite whiteKnight;
    public Sprite whiteRook;
    public Sprite whiteBishop;
    public Sprite blackQueen;
    public Sprite blackKnight;
    public Sprite blackRook;
    public Sprite blackBishop;

    Vector2 promotingPosition;

    void Start()
    {
        board.OnKingTaken += KingTaken;
        board.OnRematch += DeactivateWinScreen;
        board.OnSwitchTurns += SwitchResignButton;
        board.OnResign += Resign;
        board.OnDeselectPiece += CancelPromotion;
        board.OnPromotion += OpenPromotePopup;
    }

    void KingTaken()
    {
        winScreen.SetActive(true);

        if (board.turnColor == "White")
        {
            winner.text = "Player 1 wins!";
        }
        else
        {
            winner.text = "Player 2 wins!";
        }
    }

    void Resign()
    {
        winScreen.SetActive(true);

        if (board.turnColor == "Black")
        {
            winner.text = "Player 1 wins!";
        }
        else
        {
            winner.text = "Player 2 wins!";
        }
    }

    void DeactivateWinScreen()
    {
        winScreen.SetActive(false);
    }

    void SwitchResignButton()
    {
        if (board.turnColor == "White")
        {
            resignWhite.SetActive(true);
            resignBlack.SetActive(false);
        }
        else
        {
            resignWhite.SetActive(false);
            resignBlack.SetActive(true);
        }
    }

    void OpenPromotePopup(Vector2 position)
    {
        if (position.y == 7)
        {
            whitePromotePopup.SetActive(true);
            whitePromotePopup.GetComponent<RectTransform>().localPosition = new Vector2(-360.5f + position.x * 103f, 180f);
        }
        else
        {
            blackPromotePopup.SetActive(true);
            blackPromotePopup.GetComponent<RectTransform>().localPosition = new Vector2(-360.5f + position.x * 103f, -180f);
        }

        promotingPosition = position;
    }

    public void Promote(string newType)
    {
        whitePromotePopup.SetActive(false);
        blackPromotePopup.SetActive(false);

        Image pieceImage = board.selectedPiece.GetComponentsInChildren<Image>()[1];

        if (board.selectedPiece.color == "White")
        {
            if (newType == "Queen")
            {
                pieceImage.sprite = whiteQueen;
            }
            else if (newType == "Knight")
            {
                pieceImage.sprite = whiteKnight;
            }
            else if (newType == "Rook")
            {
                pieceImage.sprite = whiteRook;
            }
            else if (newType == "Bishop")
            {
                pieceImage.sprite = whiteBishop;
            }
        }
        else
        {
            if (newType == "Queen")
            {
                pieceImage.sprite = blackQueen;
            }
            else if (newType == "Knight")
            {
                pieceImage.sprite = blackKnight;
            }
            else if (newType == "Rook")
            {
                pieceImage.sprite = blackRook;
            }
            else if (newType == "Bishop")
            {
                pieceImage.sprite = blackBishop;
            }
        }

        board.Promote(newType, promotingPosition);
    }

    public void CancelPromotion()
    {        
        whitePromotePopup.SetActive(false);
        blackPromotePopup.SetActive(false);

        board.FinishMove();
    }
}
