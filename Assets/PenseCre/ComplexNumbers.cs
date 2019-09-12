
using MathNet.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexNumbers : MonoBehaviour
{
    [SerializeField] private Complex32 complex32;
    [SerializeField] private float complex32R;
    [SerializeField] private float complex32I;
    [SerializeField] private float complex32Mag;
    [SerializeField] private float complex32MagSqr;
    [SerializeField] private float complex32Phase;
    [SerializeField] private string complex32Sign;
    [SerializeField] private string complex32Str;

    // Start is called before the first frame update
    void Start()
    {
        complex32 = (Complex32.ImaginaryOne * 10);
        complex32R = complex32.Real;
        complex32I = complex32.Imaginary;
        complex32Mag = complex32.Magnitude;
        complex32MagSqr = complex32.MagnitudeSquared;
        complex32Phase = complex32.Phase;
        complex32Sign = complex32.Sign.ToString();
        complex32Str = complex32.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
