using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using MathNet.Numerics;
using System;

public struct LocalTransform : IComponentData
{
    public float3 Position;
    public float Scale;
    public quaternion Rotation;
}

public partial class GraphSystem : SystemBase //ComponentSystem
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

    }   

    protected override void OnUpdate()
    {
        float timeSinceLevelLoad = (float)SystemAPI.Time.ElapsedTime;
        float t = (timeSinceLevelLoad - GraphEntity.startTime) * GraphComponent.speed + GraphComponent.offset;
        GraphFunction f = functions[GraphComponent.function];
        int resolution = GraphComponent.resolution;
        float step = 2f / resolution;

        Entities
            .WithoutBurst()
            .ForEach((ref GraphComponent graphComponent, ref LocalToWorld localTransform) =>
        {
            float v = (graphComponent.id.x + 0.5f) * step - 1f;
            float u = (graphComponent.id.y + 0.5f) * step - 1f;

            //float v = graphComponent.id.x;
            //float u = graphComponent.id.y;
            
            var pos = f(u, v, t);
            
            localTransform.Value = Matrix4x4.TRS(pos, Quaternion.identity, ScaleFunction(pos) * Vector3.one);
        }).Run();
    }

    private float ScaleFunction(float3 value)
    {
        return math.length(float3.zero + value) * GraphComponent.scaleMult + GraphComponent.scaleOffset;
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

    static Vector3 MyFunction(float u, float v, float t)
    {
        Vector3 p;
        //u = math.remap(u, 0f, GraphComponent.resolution, 0f, math.PI / 2f);
        //v = math.remap(v, 0f, GraphComponent.resolution, -math.PI/2f, math.PI / 2f);
        float alpha = (u + v) * GraphComponent.resolution;
        
        p = Coordinate(u, v, t,
            u * GraphComponent.resolution * GraphComponent.value1,
            v * GraphComponent.resolution * GraphComponent.value2,
            alpha * GraphComponent.value3);
        return p;
    }

    static Vector3 Coordinate(float x, float y, float n, float k1, float k2, float a)
    {
        Complex32 z1 = Multiply(
            Exp(new Complex32(0, 2f * math.PI * k1 / n)),
            Pow(Complex32.Sin(new Complex32(x, y)), 2 / n)
        );
        Complex32 z2 = Multiply(
            Exp(new Complex32(0, 2f * math.PI * k2 / n)),
            Pow(Sin(new Complex32(x, y)), 2 / n)
        );
        return new Vector3(z1.Real, z2.Real, z1.Imaginary * math.cos(a) + z2.Imaginary * math.sin(a));
    }
    #endregion

    #region Complex wrapper functions
    public static float Re(Complex32 input) { return input.Real; }
    public static float Im(Complex32 input) { return input.Real; }
    public static Complex32 I { get { return Complex32.ImaginaryOne; } }
    public static Complex32 Cos(Complex32 input) { return Complex32.Cos(input); }
    public static Complex32 Sin(Complex32 input) { return Complex32.Sin(input); }
    public static Complex32 Exp(Complex32 value) { return Complex32.Exp(value); }
    public static Complex32 Pow(Complex32 val, Complex32 pow) { return Complex32.Pow(val, pow); }
    public static Complex32 Add(Complex32 left, Complex32 right) { return Complex32.Add(left, right); }
    public static Complex32 Subtract(Complex32 left, Complex32 right) { return Complex32.Subtract(left, right); }
    public static Complex32 Multiply(Complex32 left, Complex32 right) { return Complex32.Multiply(left, right); }
    public static Complex32 Divide(Complex32 dividend, Complex32 divisor) { return Complex32.Divide(dividend, divisor); }
    #endregion
}
