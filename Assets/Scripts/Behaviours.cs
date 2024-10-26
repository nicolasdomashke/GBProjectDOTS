using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;

public class WaitingBehaviour : IBehaviour
{
    public float Evaluate(Entity entity, EntityManager entityManager, float dTime)
    {
        return .2f;
    }
    public void Execute(Entity entity, EntityManager entityManager, float dTime) 
    {
        //Debug.Log("wait");
        return;
    }
}

public class ShootingBehaviour : IBehaviour
{
    public float Evaluate(Entity entity, EntityManager entityManager, float dTime)
    {
        Shooter shoot = entityManager.GetComponentData<Shooter>(entity);

        if (shoot.gunCDCurrent > 0)
        {
            shoot.gunCDCurrent -= dTime;
            entityManager.SetComponentData(entity, shoot);
            return 0f;
        }
        return .5f;
    }
    public void Execute(Entity entity, EntityManager entityManager, float dTime) 
    {
        LocalTransform transform = entityManager.GetComponentData<LocalTransform>(entity);
        Shooter shoot = entityManager.GetComponentData<Shooter>(entity);

        shoot.gunCDCurrent = shoot.gunCD;
        float3 spawnLocation = transform.Position + transform.Forward() + new float3(0f, .5f, 0f);
        Entity bullet = entityManager.Instantiate(shoot.bullet);
        entityManager.SetComponentData(bullet, new LocalTransform {Position = spawnLocation, Rotation = transform.Rotation, Scale = .25f});
        entityManager.SetComponentData(bullet, new PhysicsVelocity {Linear = transform.Forward() * shoot.bulletSpeed});
        entityManager.SetComponentData(entity, shoot);
        //Debug.Log("shoot");
    }
}

public class DashingBehaviour : IBehaviour
{
    public float Evaluate(Entity entity, EntityManager entityManager, float dTime)
    {
        Dashable dash = entityManager.GetComponentData<Dashable>(entity);
        EvatuationData evalData = entityManager.GetComponentData<EvatuationData>(entity);

        if (dash.dashBoostCurrent > 0f) 
        {
            return 1f;
        }
        if (dash.dashCDCurrent > 0f)
        {
            dash.dashCDCurrent -= dTime;
            entityManager.SetComponentData(entity, dash);
            return 0f;
        }
        if (evalData.distanceToClosestBullet > 0)
        {
            return 1f / (evalData.distanceToClosestBullet + 1f);
        }
        return 0f;

    }
    public void Execute(Entity entity, EntityManager entityManager, float dTime)
    {
        Dashable dash = entityManager.GetComponentData<Dashable>(entity);
        LocalTransform transform = entityManager.GetComponentData<LocalTransform>(entity);
        PhysicsVelocity velocity = entityManager.GetComponentData<PhysicsVelocity>(entity);

        if (dash.dashBoostCurrent <= 0f)
        {
            float2 circle = UnityEngine.Random.insideUnitCircle.normalized;
            float3 lookVector = new float3 (circle.x, 0f, circle.y);
            transform.Rotation = Quaternion.LookRotation(lookVector, Vector3.up);
            dash.dashCDCurrent = dash.dashCD;
            dash.dashBoostCurrent = dash.dashDuration;
            velocity.Linear = lookVector * dash.dashBoost;

            entityManager.SetComponentData(entity, transform);
            entityManager.SetComponentData(entity, velocity);
        }
        dash.dashBoostCurrent -= dTime;
        entityManager.SetComponentData(entity, dash);
        if (dash.dashBoostCurrent <= 0f)
        {
            velocity.Linear = float3.zero;
            entityManager.SetComponentData(entity, velocity);
        }
        //Debug.Log("dash");
    }
}