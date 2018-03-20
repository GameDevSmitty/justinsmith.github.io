using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierAmountUI : MonoBehaviour
{
    [SerializeField] TextMesh multiplierText;
    [SerializeField] TextMesh multiplierShadowText;
    public Color MultiplierTextColor;
    public Color MultiplierShadowColor;
    private int multiplierDisplayNumber;
    private string stringToDisplay;

    public void GetMultiplier(int multiplier)
    {
        multiplierDisplayNumber = multiplier;
        stringToDisplay = "x " + multiplierDisplayNumber;
    }

    // Update is called once per frame
    void Update()
    {
        multiplierText.color = MultiplierTextColor;
        multiplierShadowText.color = MultiplierShadowColor;
        multiplierText.text = stringToDisplay;
        multiplierShadowText.text = stringToDisplay;
    }
}
