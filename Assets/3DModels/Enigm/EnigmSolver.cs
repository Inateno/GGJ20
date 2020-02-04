using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmSolver : MonoBehaviour
{
    public int valueExpected = 2;
    public int currentValue = 0;
    public Hatch chest;

    public void OnPartSolved()
    {
        ++currentValue;
        if (currentValue == valueExpected) {
            chest.Open();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
