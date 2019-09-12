using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using MathNet.Numerics;
using System;

public class GraphSystem : ComponentSystem
{

    #region Declarations
    static GraphFunction[] functions = {
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction,
        Ripple, Cylinder, Sphere, Torus, MyFunction
    };

    const float PI = math.PI;
    const float E = math.E;
    const double PI_DBL = math.PI_DBL;
    const double E_DBL = math.E_DBL;
    #endregion

    #region Unity API
    protected override void OnCreate()
    {
        k1 = new Complex32();
        k2 = new Complex32();
    }   

    protected override void OnUpdate()
    {
        float t = Time.time * GraphComponent.speed + GraphComponent.offset;
        GraphFunction f = functions[GraphComponent.function];
        int resolution = GraphComponent.resolution;
        float step = 2f / resolution;

        Entities.ForEach((ref GraphComponent graphComponent, ref Translation translation) =>
        {
            float v = (graphComponent.id.x + 0.5f) * step - 1f;
            float u = (graphComponent.id.y + 0.5f) * step - 1f;
            translation.Value = f(u, v, t);
        });
    }
    #endregion

    #region Functions implementation (default from catlikecoding tutorial)
    static Vector3 SineFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = math.sin(PI * (x + t));
        p.z = z;
        return p;
    }

    static Vector3 MultiSineFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = math.sin(PI * (x + t));
        p.y += math.sin(2f * PI * (x + 2f * t)) / 2f;
        p.y *= 2f / 3f;
        p.z = z;
        return p;
    }

    static Vector3 Sine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = math.sin(PI * (x + t));
        p.y += math.sin(PI * (z + t));
        p.y *= 0.5f;
        p.z = z;
        return p;
    }

    static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = 4f * math.sin(PI * (x + z + t / 2f));
        p.y += math.sin(PI * (x + t));
        p.y += math.sin(2f * PI * (z + 2f * t)) * 0.5f;
        p.y *= 1f / 5.5f;
        p.z = z;
        return p;
    }

    static Vector3 Ripple(float x, float z, float t)
    {
        Vector3 p;
        float d = math.sqrt(x * x + z * z);
        p.x = x;
        p.y = math.sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = z;
        return p;
    }

    static Vector3 Cylinder(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.8f + math.sin(PI * (6f * u + 2f * v + t)) * 0.2f;
        p.x = r * math.sin(PI * u);
        p.y = v;
        p.z = r * math.cos(PI * u);
        return p;
    }

    static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.8f + math.sin(PI * (6f * u + t)) * 0.1f;
        r += math.sin(PI * (4f * v + t)) * 0.1f;
        float s = r * math.cos(PI * 0.5f * v);
        p.x = s * math.sin(PI * u);
        p.y = r * math.sin(PI * 0.5f * v);
        p.z = s * math.cos(PI * u);
        return p;
    }

    static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 0.65f + math.sin(PI * (6f * u + t)) * 0.1f;
        float r2 = 0.2f + math.sin(PI * (4f * v + t)) * 0.05f;
        float s = r2 * math.cos(PI * v) + r1;
        p.x = s * math.sin(PI * u);
        p.y = r2 * math.sin(PI * v);
        p.z = s * math.cos(PI * u);
        return p;
    }
    #endregion

    #region My Complex Function
    static Complex32 k1;
    static Complex32 k2;

    static Vector3 MyFunction(float u, float v, float t)
    {
        //u = i % resolution
        //v = (i + resolution) / resolution
        // float v = (graphComponent.id.x + 0.5f) * step - 1f;
        // float u = (graphComponent.id.y + 0.5f) * step - 1f;
        // failed attempt to pre-process the uv coordinate numbers to adapt to the original's required matching input functions
        //u = u * GraphComponent.resolution;
        //v = (v + GraphComponent.resolution) * GraphComponent.resolution;

        Vector3 p;
        
        float n = 5;

        //guessing
        float a = u;
        float b = v;
        float alpha = t + (u + v) * GraphComponent.resolution;
        k1 = new Complex32(a + math.sin(alpha) / GraphComponent.resolution, a + math.cos(alpha));
        k2 = new Complex32(b + math.cos(alpha) / GraphComponent.resolution, b + math.sin(alpha));
        //k1 = new Complex32(a, alpha);
        //k2 = new Complex32(b, alpha);
        //k1 = new Complex32(b * alpha, a * alpha);
        //k2 = new Complex32(b * alpha, a * alpha);

        Complex32 Z1k = z1k(a, b, n, k1);
        Complex32 Z2k = z2k(a, b, n, k2);
        
        p.x = Re(Z1k);
        p.y = Re(Z2k);
        p.z = Im(Z1k * u + v * Z2k);
        //p.z = Im(Z1k) * u + v * Im(Z2k);
        //p.z = math.cos(alpha) * Im(Z1k) +
        //      math.sin(alpha) * Im(Z2k);

        return p;
    }

    private static Complex32 z1k(float a, float b, float n, Complex32 k1)
    {
        return Multiply(Pow(E, Multiply((Multiply(k1,new Complex32(2f * PI, Im(I)))), Divide(I, new Complex32(1f, n)))),
            Pow(u1(a, b), (2f/n)));
    }

    private static Complex32 z2k(float a, float b, float n, Complex32 k2)
    {
        return Multiply(Pow(E, Multiply((Multiply(k2, new Complex32(2f * PI, Im(I)))), Divide(I, new Complex32(1f, n)))),
            Pow(u2(a, b), (2f / n)));
    }

    private static Complex32 u1(float a, float b)
    {
        return Add(
            Multiply(new Complex32(0.5f, Im(I)), Pow(E, Add(new Complex32(a, Im(I)), Multiply(I, new Complex32(1f, b))))),
            (Pow(E, Subtract(new Complex32(-a, Im(I)), Multiply(I, new Complex32(1f, b))))));
    }

    private static Complex32 u2(float a, float b)
    {
        return Subtract(
            Multiply(new Complex32(0.5f, Im(I)), Pow(E, Add(new Complex32(a, Im(I)), Multiply(I, new Complex32(1f, b))))),
            (Pow(E, Subtract(new Complex32(-a, Im(I)), Multiply(I, new Complex32(1f, b))))));
    }

    #endregion

    #region Complex wrapper functions
    public static float Re(Complex32 input) { return input.Real; }
    public static float Im(Complex32 input) { return input.Real; }
    public static Complex32 I { get { return Complex32.ImaginaryOne; } }
    public static Complex32 Cos(Complex32 input) { return Complex32.Cos(input); }
    public static Complex32 Sin(Complex32 input) { return Complex32.Sin(input); }
    public static Complex32 Pow(Complex32 val, Complex32 pow) { return Complex32.Pow(val, pow); }
    public static Complex32 Add(Complex32 left, Complex32 right) { return Complex32.Add(left, right); }
    public static Complex32 Subtract(Complex32 left, Complex32 right) { return Complex32.Subtract(left, right); }
    public static Complex32 Multiply(Complex32 left, Complex32 right) { return Complex32.Multiply(left, right); }
    public static Complex32 Divide(Complex32 dividend, Complex32 divisor) { return Complex32.Divide(dividend, divisor); }
    #endregion
}
