using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CreateEntity : MonoBehaviour
{
    void Start()
    {
        EntityManager em = World.Active.EntityManager;
        em.CreateEntity();
    }
}
