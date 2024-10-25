using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using System.Linq;

public partial class EnemyBehaviourSystem : SystemBase
{

    private EntityQuery enemiesQuery;
    private EntityQuery targetsQuery;
    private WaitingBehaviour b0;
    private ShootingBehaviour b1;
    private DashingBehaviour b2;
    protected override void OnCreate()
    {
        enemiesQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyTag>());
        targetsQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>(), ComponentType.ReadOnly<LocalTransform>());
        b0 = new WaitingBehaviour();
        b1 = new ShootingBehaviour();
        b2 = new DashingBehaviour();
    }
    protected override void OnUpdate() 
    {
        float dTime = SystemAPI.Time.DeltaTime;
        var enemies = enemiesQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
        var playerTransform = targetsQuery.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.TempJob);

        if (playerTransform.Length > 0)
        {
            foreach (var entity in enemies)
            {
                float [] weights = new float[3];
                weights[0] = b0.Evaluate(entity, EntityManager, dTime);
                weights[1] = b1.Evaluate(entity, EntityManager, dTime);
                weights[2] = b2.Evaluate(entity, EntityManager, dTime);
                float maxVal = weights.Max();
                if (maxVal == weights[0])
                {
                    b0.Execute(entity, EntityManager, dTime);
                    var transform = EntityManager.GetComponentData<LocalTransform>(entity);
                    float3 lookVector = playerTransform[0].Position - transform.Position;
                    transform.Rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                    EntityManager.SetComponentData(entity, transform);
                }
                else if (maxVal == weights[1])
                {
                    b1.Execute(entity, EntityManager, dTime);
                    var transform = EntityManager.GetComponentData<LocalTransform>(entity);
                    float3 lookVector = playerTransform[0].Position - transform.Position;
                    transform.Rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                    EntityManager.SetComponentData(entity, transform);
                }
                else
                {
                    b2.Execute(entity, EntityManager, dTime);
                }
            }
        }
    }
}