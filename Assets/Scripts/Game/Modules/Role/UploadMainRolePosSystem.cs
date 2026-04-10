using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityMMO;
using SprotoType;
using Unity.Collections;
using UnityMMO.Component;

[DisableAutoCreation]
public partial class UploadMainRolePosSystem : BaseComponentSystem
{
    float lastSynchTime = 0;
    EntityQuery group;
    public UploadMainRolePosSystem(GameWorld world) : base(world) {}

    protected override void OnCreate()
    {
        base.OnCreate();
        group = GetEntityQuery(typeof(Transform), typeof(TargetPosition), typeof(PosSynchInfo));
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.Time.ElapsedTime - lastSynchTime < 0.05 || !GameVariable.IsNeedSynchSceneInfo)
            return;
        var targetPositions = group.ToComponentDataArray<TargetPosition>(Allocator.Temp);
        var synchInfos = group.ToComponentDataArray<PosSynchInfo>(Allocator.Temp);
        var entities = group.ToEntityArray(Allocator.Temp);
        long synchTime = System.DateTime.Now.Millisecond;
        for (int i=0; i<targetPositions.Length; i++)
        {
            var targetPos = targetPositions[i].Value;
            var pos = EntityManager.GetComponentObject<Transform>(entities[i]).localPosition;
            var synchInfo = synchInfos[i];
            var distance = Vector3.Distance(targetPos, pos);
            var distance_with_last = Vector3.Distance(synchInfo.LastUploadPos, targetPos);
            // Debug.Log("distance:"+distance+" distance_with_last:"+distance_with_last+" upload pos"+pos.x+" "+pos.y+" "+pos.z);
            if (distance <= 0.5 && distance_with_last <= 0.5)
                continue;
            synchInfo.LastUploadPos = targetPos;
            // synchInfos[i] = synchInfo;
            EntityManager.SetComponentData<PosSynchInfo>(entities[i], synchInfo);
            scene_walk.request walk = new scene_walk.request();
            walk.start_x = (long)(pos.x*GameConst.RealToLogic);
            walk.start_y = (long)(pos.y*GameConst.RealToLogic);
            walk.start_z = (long)(pos.z*GameConst.RealToLogic);
            walk.end_x = (long)(targetPos.x*GameConst.RealToLogic);
            // walk.end_y = (long)(targetPos.y*GameConst.RealToLogic);
            walk.end_z = (long)(targetPos.z*GameConst.RealToLogic);
            walk.time = synchTime;
            walk.jump_state = 0;
            NetMsgDispatcher.GetInstance().SendMessage<Protocol.scene_walk>(walk);
            lastSynchTime = (float)SystemAPI.Time.ElapsedTime;
        }
        entities.Dispose();
        targetPositions.Dispose();
        synchInfos.Dispose();
    }
}