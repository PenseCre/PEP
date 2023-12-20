using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SetComponentData : MonoBehaviour
{
    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = entityManager.CreateEntity(typeof(MyComponent));

        entityManager.SetComponentData(entity, new MyComponent { level = 10f });
    }
}