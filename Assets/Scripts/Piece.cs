using UnityEngine;

public class Piece : MonoBehaviour
{
    public string type;
    public string color;

    public Vector2 position;

    public GameObject selectedPieceSprite;

    void Start()
    {
        Reposition(position);
    }

    public void Reposition(Vector2 newPosition)
    {
        position = newPosition;

        transform.localPosition = position * 103;
    }

    public void SelectPiece(bool select)
    {
        selectedPieceSprite.SetActive(select);
    }
}
