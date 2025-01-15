using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using DG.Tweening;
using System.Linq;

public partial class TrapSystem : SystemBase
{
    private EntityQuery playerQuery;

    protected override void OnCreate()
    {
        playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>(), ComponentType.ReadOnly<LocalTransform>());
    }
    protected override void OnUpdate() 
    {
        var playerEntity = playerQuery.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.TempJob);

        Entities.WithAll<TrapTag>().ForEach((ref Entity entity, ref TrapTag trapTag, ref LocalTransform transform) =>
        {
            if (playerEntity.Length > 0)
            {
                if (!trapTag.isTriggered)
                {
                    float distanceToPlayer = math.distance(playerEntity[0].Position, transform.Position);
                    if (distanceToPlayer < 1.5f)
                    {
                        DotweenAnimate(entity, EntityManager);
                        trapTag.isTriggered = true;
                    }
                }
                else if (trapTag.isFinished)
                {
                    float distanceToPlayer = math.distance(playerEntity[0].Position, transform.Position);
                    if (distanceToPlayer < 2f)
                    {
                        var player = playerQuery.ToEntityArray(Unity.Collections.Allocator.TempJob)[0];
                        EntityManager.SetComponentData(player, new LocalTransform {Scale = 0f});
                        EntityManager.RemoveComponent<PlayerTag>(player);
                        Debug.Log("You died!");
                    }
                    Entity particles = EntityManager.Instantiate(trapTag.deathEffect);
                    EntityManager.SetComponentData(particles, new LocalTransform {Position = transform.Position, Rotation = Quaternion.Euler(-90, 0, 0),  Scale = 1f});
                    EntityManager.DestroyEntity(entity);
                }
            }
        }).WithStructuralChanges().Run();
    }

    private void DotweenAnimate(Entity entity, EntityManager entityManager)
    {
        RotationSpeed rotSpeed = entityManager.GetComponentData<RotationSpeed>(entity);
        DOTween.To(() => rotSpeed.speed, x => rotSpeed.speed = x, 10f, 3f).OnUpdate(() => { entityManager.SetComponentData(entity, rotSpeed); });
        LocalTransform transform = entityManager.GetComponentData<LocalTransform>(entity);
        DOTween.To(() => transform.Scale, x => transform.Scale = x, 2f, 3f).OnUpdate(() => 
        { 
            var newTransform = entityManager.GetComponentData<LocalTransform>(entity);
            newTransform.Scale = transform.Scale;
            entityManager.SetComponentData(entity, newTransform); 
        }).OnComplete(() => 
        {
            var trapTag = entityManager.GetComponentData<TrapTag>(entity);
            trapTag.isFinished = true;
            entityManager.SetComponentData(entity, trapTag); 
        });

    }
}
