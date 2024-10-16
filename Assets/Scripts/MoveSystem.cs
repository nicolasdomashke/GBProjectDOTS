using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Transforms;
using Unity.Mathematics;


public partial class MoveSystem : SystemBase
{
    private InputSystem_Actions inputSystem;

    protected override void OnCreate()
    {
        inputSystem = new InputSystem_Actions();
        inputSystem.Enable();
    }

    protected override void OnUpdate()
    {
        float2 moveInput = inputSystem.Player.Move.ReadValue<Vector2>();
        bool isDashPressed = inputSystem.Player.Dash.WasPressedThisFrame();
        float dTime = SystemAPI.Time.DeltaTime;

        Entities.WithAll<InputEnabled>().ForEach((ref LocalTransform transform, ref Dashable dash, in MovementSpeed moveSpeed) =>
        {
            if (dash.dashBoostCurrent > 0f)
            {
                float3 pos = transform.Position;
                float3 lookVector = transform.Forward();
                pos += lookVector * moveSpeed.speed * dTime * dash.dashBoost;
                transform.Position = pos;
                dash.dashBoostCurrent -= dTime;
            }
            else
            {
                float3 pos = transform.Position;
                float3 lookVector = new float3(moveInput.x, 0f, moveInput.y);
                pos += lookVector * moveSpeed.speed * dTime;
                transform.Position = pos;

                if (!lookVector.Equals(float3.zero)) 
                {
                    transform.Rotation = Quaternion.LookRotation(lookVector, Vector3.up);
                }
            }
            if (dash.dashCDCurrent > 0f)
            {
                dash.dashCDCurrent -= dTime;
            }
            else if (isDashPressed)
            {
                dash.dashCDCurrent = dash.dashCD;
                dash.dashBoostCurrent = dash.dashDuration;
            }



        }).ScheduleParallel();
    }

    protected override void OnDestroy()
    {
        inputSystem.Disable();
    }
}
