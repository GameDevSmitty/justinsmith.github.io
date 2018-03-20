using UnityEngine;
using System.Collections;

public class PuzzlePiece : MonoBehaviour
{
    bool isSelected = false;
    Vector3 right;
    Vector3 left;
    Vector3 top;
    Vector3 bot;
    Vector3 directionToMove;
    Vector3 newPiecePosition;
    float distance;
    bool canMovePiece;

    [SerializeField]
    Transform distanceDesired;
    [SerializeField]
    Material thisMaterial;
    public Material ThisMaterial { get { return thisMaterial; } }

    void Start()
    {
        distance = Vector3.Distance(transform.position, distanceDesired.position);
    }

    void OnMouseDown()
    {
        CheckForEmptySpace();
        if (canMovePiece)
            MoveToEmptySpace();
    }
    
    void Update ()
    {
        isSelected = GetComponentInParent<SlidePuzzle>().IsSelected;
	}

    private void MoveToEmptySpace()
    {
        if (directionToMove == right)
        {
            newPiecePosition = new Vector3(transform.position.x + distance, transform.position.y, transform.position.z);
            transform.position = newPiecePosition;
        }
        else if (directionToMove == left)
        {
            newPiecePosition = new Vector3(transform.position.x - distance, transform.position.y, transform.position.z);
            transform.position = newPiecePosition;
        }
        else if (directionToMove == top)
        {
            newPiecePosition = new Vector3(transform.position.x, transform.position.y + distance, transform.position.z);
            transform.position = newPiecePosition;
        }
        else if (directionToMove == bot)
        {
            newPiecePosition = new Vector3(transform.position.x, transform.position.y - distance, transform.position.z);
            transform.position = newPiecePosition;
        }
    }

    private void CheckForEmptySpace()
    {
        canMovePiece = false;
        if (isSelected)
        {
            if (canMovePiece == false)
            {
                right = GetComponentsInChildren<Transform>()[1].position - transform.position;
                if (!Physics.Raycast(transform.position, right, distance))
                {
                    canMovePiece = true;
                    directionToMove = right;
                }
            }

            if (canMovePiece == false)
            {
                left = GetComponentsInChildren<Transform>()[2].position - transform.position;
                if (!Physics.Raycast(transform.position, left, distance))
                {
                    canMovePiece = true;
                    directionToMove = left;
                }
            }

            if (canMovePiece == false)
            {
                top = GetComponentsInChildren<Transform>()[3].position - transform.position;
                if (!Physics.Raycast(transform.position, top, distance))
                {
                    canMovePiece = true;
                    directionToMove = top;
                }
            }

            if (canMovePiece == false)
            {
                bot = GetComponentsInChildren<Transform>()[4].position - transform.position;
                if (!Physics.Raycast(transform.position, bot, distance))
                {
                    canMovePiece = true;
                    directionToMove = bot;
                }
            }

        }
    }
}
