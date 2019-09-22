using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Random = UnityEngine.Random;

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

    void Start()
    {
        Application.targetFrameRate = fps;
        QualitySettings.vSyncCount = vsync;

        float step = 2f / resolution * scaleMult;
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
            GraphComponent.speed = speed;
            GraphComponent.offset = offset;
            GraphComponent.value1 = value1;
            GraphComponent.value2 = value2;
            GraphComponent.value3 = value3;
            GraphComponent.scaleMult = scaleMult;
            GraphComponent.scaleOffset = scaleOffset;
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
        GraphComponent.speed = speed;
        GraphComponent.offset = offset;
        GraphComponent.value1 = value1;
        GraphComponent.value2 = value2;
        GraphComponent.value3 = value3;
        GraphComponent.scaleMult = scaleMult;
        GraphComponent.scaleOffset = scaleOffset;
        if (GraphComponent.function != (int)function)
        {
            GraphComponent.function = (int)function;
        }
    }
}
