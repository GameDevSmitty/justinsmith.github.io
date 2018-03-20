using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using UnityEngine.UI;

public class SlidePuzzle : MonoBehaviour, IActivatable
{
    [SerializeField]
    ColliderCheckForPieceHere[] checkPieceCollision;

    [SerializeField]
    Material[] puzzleMaterials;

    [SerializeField]
    GameObject[] puzzlePieces;
    
    [SerializeField]
    string nameText;

    [SerializeField]
    bool isDoor;

    [SerializeField]
    bool isFinalPuzzle;

    [SerializeField]
    AudioSource winGameSound;

    [SerializeField]
    Text winText;

    bool isSelected;
    public bool IsSelected { get { return isSelected; } }

    bool isComplete;
    public bool IsComplete { get { return isComplete; } }

    public string NameText { get { return nameText; } }
	
	void Update ()
    {
        HandleInput();
        UpdateCursor();

        if (isSelected == true && CheckIfImagesAreInOrder())
            HidePuzzlePieces();

    }

    private void HidePuzzlePieces()
    {
        foreach (GameObject puzzlePiece in puzzlePieces)
            puzzlePiece.SetActive(false);

        if (isDoor)
            gameObject.SetActive(false);

        if (isFinalPuzzle)
        {
            winGameSound.Play();
            while (winGameSound.isPlaying)
            {
                winText.text = "You WIN!!!";
            }
            Application.Quit();
        }

        isSelected = false;
        isComplete = true;
    }

    private void HandleInput()
    {
        if (isSelected == true && Input.GetButtonDown("Submit"))
        {
            isSelected = false;
        }
    }

    private void UpdateCursor()
    {
        if (isSelected)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    }

    bool CheckIfImagesAreInOrder()
    {
        bool isInOrder = true;
        int noMoreThanOneNull = 0;
        for (int i = 0; i < checkPieceCollision.Length; i++)
        {
            if (isInOrder)
            { 

                if (noMoreThanOneNull > 1)
                {
                    isInOrder = false;
                    break;
                }

                if (checkPieceCollision[i].PuzzlePieceCollider == null)
                {
                    isInOrder = true;
                    noMoreThanOneNull++;
                }
                else if (checkPieceCollision[i].PuzzlePieceCollider.gameObject.GetComponent<PuzzlePiece>().ThisMaterial 
                    != puzzleMaterials[i])
                {
                    isInOrder = false;
                    break;
                }
            }
        }
        return isInOrder;
    }

    public void DoActivate()
    {
        isSelected = true;
    }
}
