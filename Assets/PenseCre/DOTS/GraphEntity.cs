﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.IO;
using PenseCre;

public class GraphEntity : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private int resolution = 10;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float offset = 0.0f;
    [SerializeField] private float value1 = 0.0f;
    [SerializeField] private float value2 = 0.0f;
    [SerializeField] private float value3 = 0.0f;
    [SerializeField] private int fps = 30;
    [SerializeField] private int vsync = 1;
    [SerializeField] private GraphFunctionNameDots function;
    [SerializeField] private float scaleMult = 1f;
    [SerializeField] private float scaleOffset = 0f;

    public static float startTime = 0;
    public float GraphTime { get => (Time.timeSinceLevelLoad - startTime) * GraphComponent.speed + GraphComponent.offset; }

    public float Speed { get => speed; set => speed = value; }
    public float Offset { get => offset; set => offset = value; }
    public float Value1 { get => value1; set => value1 = value; }
    public float Value2 { get => value2; set => value2 = value; }
    public float Value3 { get => value3; set => value3 = value; }
    public int Fps { get => fps; set => fps = value; }
    public int Vsync { get => vsync; set => vsync = value; }
    public int Function { get => (int)function; set => function = (GraphFunctionNameDots)value; }
    public float ScaleMult { get => scaleMult; set => scaleMult = value; }
    public float ScaleOffset { get => scaleOffset; set => scaleOffset = value; }
    public float Resolution { get => resolution; set => resolution = (int)value; }

    public string Str_GraphTime { get => GraphTime.ToString(); set { } }
    public string Str_Speed { get => speed.ToString(); set => speed = float.Parse(value); }
    public string Str_Offset { get => offset.ToString(); set => offset = float.Parse(value); }
    public string Str_Value1 { get => value1.ToString(); set => value1 = float.Parse(value); }
    public string Str_Value2 { get => value2.ToString(); set => value2 = float.Parse(value); }
    public string Str_Value3 { get => value3.ToString(); set => value3 = float.Parse(value); }
    public string Str_Fps { get => fps.ToString(); set => fps = int.Parse(value); }
    public string Str_Vsync { get => vsync.ToString(); set => vsync = int.Parse(value); }
    public string Str_ScaleMult { get => scaleMult.ToString(); set => scaleMult = float.Parse(value); }
    public string Str_ScaleOffset { get => scaleOffset.ToString(); set => scaleOffset = float.Parse(value); }
    public string Str_Resolution { get => resolution.ToString(); set => resolution = int.Parse(value); }

    public void TakeScreenshot()
    {
        string userPath = string.Empty;
        string systemPathChar = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
                break;
            case RuntimePlatform.OSXPlayer:
                break;
            case RuntimePlatform.WindowsPlayer:
                userPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                systemPathChar = @"\";
                break;
            case RuntimePlatform.WindowsEditor:
                userPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                systemPathChar = @"\";
                break;
            case RuntimePlatform.IPhonePlayer:
                break;
            case RuntimePlatform.Android:
                var jc = new AndroidJavaClass("android.os.Environment");
                //userPath = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
                userPath = @jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", jc.GetStatic<string>("DIRECTORY_DCIM")).Call<string>("getAbsolutePath");
                systemPathChar = "";
                break;
            case RuntimePlatform.LinuxPlayer:
                break;
            case RuntimePlatform.LinuxEditor:
                break;
            case RuntimePlatform.WebGLPlayer:
                break;
            case RuntimePlatform.PS4:
                break;
            case RuntimePlatform.XboxOne:
                break;
            case RuntimePlatform.tvOS:
                break;
            case RuntimePlatform.Switch:
                break;
            case RuntimePlatform.Lumin:
                break;
            case RuntimePlatform.BJM:
                break;
            default:
                break;
        }
        if (string.IsNullOrEmpty(userPath)) return;

        string name = "PenseCre";
        string ext = ".png";
        string dateTime = FileUtils.GetFormattedDate();
        string uniqueNamefullPath = FileUtils.UniqueNameFullPath(@userPath + @systemPathChar, name + " " + dateTime, ext);
        Debug.Log(uniqueNamefullPath);
        ScreenCapture.CaptureScreenshot(uniqueNamefullPath, 1);
    }

    void Start()
    {
        Application.targetFrameRate = Fps;
        QualitySettings.vSyncCount = Vsync;

        if (PlayerPrefs.HasKey(nameof(resolution))){
            resolution = PlayerPrefs.GetInt(nameof(resolution));
        }

        float step = 2f / resolution * ScaleMult;
        Vector3 scale = Vector3.one * step;

        EntityManager entityManager = World.Active.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(GraphComponent)
        );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(resolution*resolution, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];

            entityManager.SetComponentData(entity,
                new Translation
                {
                    //Value = new float3(Random.Range(-8f, 8f), Random.Range(-5f, 5f), 0)
                }
            );

            entityManager.SetComponentData(entity,
                new Scale
                {
                    Value = step
                }
            );

            GraphComponent.resolution = resolution;
            GraphComponent.speed = Speed;
            GraphComponent.offset = Offset;
            GraphComponent.value1 = Value1;
            GraphComponent.value2 = Value2;
            GraphComponent.value3 = Value3;
            GraphComponent.scaleMult = ScaleMult;
            GraphComponent.scaleOffset = ScaleOffset;
            GraphComponent.function = 0;
            entityManager.SetComponentData(entity,
                new GraphComponent
                {
                    id = new Vector2Int(i % resolution, (i+resolution)/resolution),
                    scale = step
                }
            );

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material
            });
        }

        entityArray.Dispose();
    }

    private void Update()
    {
        GraphComponent.speed = Speed;
        GraphComponent.offset = Offset;
        GraphComponent.value1 = Value1;
        GraphComponent.value2 = Value2;
        GraphComponent.value3 = Value3;
        GraphComponent.scaleMult = ScaleMult;
        GraphComponent.scaleOffset = ScaleOffset;
        if (GraphComponent.function != (int)Function)
        {
            GraphComponent.function = (int)Function;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Reset();
        }
    }

    public void Reset()
    {
        var allEntities = World.Active.EntityManager.GetAllEntities(Allocator.Temp);
        if (allEntities == null) return;
        World.Active.EntityManager.DestroyEntity(allEntities);
        allEntities.Dispose();
        PlayerPrefs.SetInt(nameof(resolution), resolution);
        startTime = Time.timeSinceLevelLoad;
        Start();
        //SceneManager.LoadScene(0);
    }
}
