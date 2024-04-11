using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentence
{
   
    public Transform Is { get; set; }
    public Transform Left { get; set; }
    public Transform Right { get; set; }
    public Transform Up { get; set; }
    public Transform Down { get; set; }

    public bool[] isCorrect;

    public Sentence()
    {
        //Is = new GameObject().transform;
        isCorrect = new bool[2];
    }

    
}
