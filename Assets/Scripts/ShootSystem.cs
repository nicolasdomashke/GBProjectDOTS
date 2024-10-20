using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;


public partial class ShootSystem : SystemBase
{
    private InputSystem_Actions inputSystem;

    protected override void OnCreate()
    {
        inputSystem = new InputSystem_Actions();
        inputSystem.Enable();
    }

    protected override void OnUpdate()
    {
        bool isShootPressed = inputSystem.Player.Shoot.WasPressedThisFrame();
        float dTime = SystemAPI.Time.DeltaTime;

        Entities.ForEach((ref LocalTransform transform, ref Shooter shoot) =>
        {
            if (shoot.gunCDCurrent > 0f)
            {
                shoot.gunCDCurrent -= dTime;
            }
            else if (isShootPressed)
            {
                shoot.gunCDCurrent = shoot.gunCD;
                float3 spawnLocation = transform.Position + transform.Forward();
                Entity bullet = EntityManager.Instantiate(shoot.bullet);
                EntityManager.SetComponentData(bullet, new LocalTransform {Position = spawnLocation, Rotation = transform.Rotation, Scale = .25f});
                EntityManager.SetComponentData(bullet, new PhysicsVelocity {Linear = transform.Forward() * shoot.bulletSpeed});
            }
        }).WithStructuralChanges().Run();
    }

    protected override void OnDestroy()
    {
        inputSystem.Disable();
    }
}