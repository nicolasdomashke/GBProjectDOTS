using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using System.Linq;

public partial class EvaluationSystem : SystemBase
{
    private EntityQuery bulletQuery;

    protected override void OnCreate()
    {
        bulletQuery = GetEntityQuery(ComponentType.ReadOnly<FriendlyBulletTag>(), ComponentType.ReadOnly<LocalTransform>());
    }
    protected override void OnUpdate() 
    {
        var bulletEntities = bulletQuery.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.TempJob);

        Entities.WithAll<EnemyTag>().ForEach((ref EvatuationData evalData, in LocalTransform enemyTransform) =>
        {
            if (bulletEntities.Length > 0)
            {
                float closestBullet = math.distance(bulletEntities[0].Position, enemyTransform.Position);
                foreach (var bullet in bulletEntities)
                {
                    if (math.distance(bullet.Position, enemyTransform.Position) < closestBullet)
                    {
                        closestBullet = math.distance(bullet.Position, enemyTransform.Position);
                    }
                }
                evalData.distanceToClosestBullet = closestBullet;
            }
            else
            {
                evalData.distanceToClosestBullet = 0f;
            }
        }).ScheduleParallel();
    }
}

public struct EvatuationData: IComponentData
{
    public float distanceToClosestBullet;
}