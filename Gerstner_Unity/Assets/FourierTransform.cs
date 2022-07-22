using System.Collections.Generic;
using UnityEngine;

public class ComplexNumber
{
    public float Real;
    public float Complex;

    public ComplexNumber(float InReal, float InComplex)
    {
        Real = InReal;
        Complex = InComplex;
    }

    public static ComplexNumber ComplexExp(ComplexNumber a)
    {
    	return new ComplexNumber(Mathf.Cos(a.Complex), Mathf.Sin(a.Complex)) * Mathf.Exp(a.Real);
    }

    public static ComplexNumber operator +(ComplexNumber a) => a;
    public static ComplexNumber operator -(ComplexNumber a) => new ComplexNumber(-a.Real, a.Complex);
    
    public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
        => new ComplexNumber(a.Real + b.Real, a.Complex + b.Complex);
    
    public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
        => a + (-b);
    
    public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
        => new ComplexNumber(a.Real * b.Real - a.Complex * b.Complex, a.Real * b.Complex + a.Complex * b.Real);

    public static ComplexNumber operator *(ComplexNumber a, float b)
        => new ComplexNumber(a.Real * b, a.Complex * b);

    public static ComplexNumber operator *(float b, ComplexNumber a)
        => new ComplexNumber(a.Real * b, a.Complex * b);
    
    public static ComplexNumber operator /(ComplexNumber a, float b)
        => new ComplexNumber(a.Real / b, a.Complex / b);

    public override string ToString() => $"{Real}" + ((Complex == 0) ? "" : $" + {Complex}i");
}

// from: The Fast Fourier Transform (FFT): Most Ingenious Algorithm Ever?
// https://www.youtube.com/watch?v=h7apO7q16V0
public class FourierTransform 
{
    public static List<ComplexNumber> FFT(List<ComplexNumber> Coefficients) 
    {
        int N = Coefficients.Count; // must be a power of 2

        if (N == 1) 
        {
            return Coefficients;
        }

        ComplexNumber Mult = 2f * Mathf.PI * new ComplexNumber(0, 1) / N;
        // TODO: can we compute the twiddle early here? 

        List<ComplexNumber> P_Even = new List<ComplexNumber>();
        List<ComplexNumber> P_Odd = new List<ComplexNumber>();

        for (int I = 0; I < N; ++I)
        {
            if (I % 2 == 0)
            {
                P_Even.Add(Coefficients[I]);
            }
            else
            {
                P_Odd.Add(Coefficients[I]);
            }
        }

        List<ComplexNumber> Y_Even = FFT(P_Even);
        List<ComplexNumber> Y_Odd = FFT(P_Odd);

        List<ComplexNumber> Y = new List<ComplexNumber>();
        for (int I = 0; I < N; ++I)
        {
            Y.Add(new ComplexNumber(0, 0));
        }
        
        for (int I = 0; I < N / 2; ++I)
        {
            ComplexNumber TwiddleI = ComplexNumber.ComplexExp(Mult * I); 

            Y[I] = Y_Even[I] + TwiddleI * Y_Odd[I];
            Y[I + N / 2] = Y_Even[I] - TwiddleI * Y_Odd[I];
        }

        return Y;
    }

    public static void InverseFFT() {
    }
}