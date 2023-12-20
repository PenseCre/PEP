﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SetLevelComponentData : MonoBehaviour
{
    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = entityManager.CreateEntity(typeof(LevelComponent));

        entityManager.SetComponentData(entity, new LevelComponent { level = 10f });
    }
}