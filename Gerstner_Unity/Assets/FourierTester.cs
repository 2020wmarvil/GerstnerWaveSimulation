using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourierTester : MonoBehaviour
{
    void Start()
    {
        List<ComplexNumber> Coefficients = new List<ComplexNumber> { 
            new ComplexNumber(5, 0), 
            new ComplexNumber(3, 0), 
            new ComplexNumber(2, 0), 
            new ComplexNumber(1, 0) 
        };
        List<ComplexNumber> Results = FourierTransform.FFT(Coefficients);

        foreach (ComplexNumber CN in Results)
        {
            print(CN);
        }
    }
}
