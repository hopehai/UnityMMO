
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityMMO.Component;

namespace UnityMMO
{    
[DisableAutoCreation]
partial class HandleMovementQueries : BaseComponentSystem
{
    EntityQuery Group;
	
    public HandleMovementQueries(GameWorld world) : base(world) {}
	
    protected override void OnCreate()
    {
        base.OnCreate();
        Group = GetEntityQuery(typeof(MoveQuery));
    }

    protected override void OnUpdate()
    {
        var entityArray = Group.ToEntityArray(Allocator.Temp);
        for (var i = 0; i < entityArray.Length; i++)
        {
            var query = EntityManager.GetComponentObject<MoveQuery>(entityArray[i]);
            if (!query.IsAutoFinding)
            {
                var charController = query.charController;
                float3 currentControllerPos = charController.transform.position;
                if (math.distance(currentControllerPos, query.moveQueryStart) > 0.01f)
                {
                    currentControllerPos = query.moveQueryStart;
                    charController.transform.position = currentControllerPos;
                }

                var deltaPos = query.moveQueryEnd - currentControllerPos; 
                // Debug.Log("deltaPos : "+deltaPos.x+" "+deltaPos.y+" "+deltaPos.z);
                charController.Move(deltaPos);
                query.moveQueryResult = charController.transform.position;
                query.isGrounded = charController.isGrounded;
                // Debug.Log("res pos : "+query.moveQueryResult.x+" "+query.moveQueryResult.y+" "+query.moveQueryResult.z);
                query.transform.localPosition = query.moveQueryResult;
            }
            else
            {
                query.UpdateSpeed();
                var isReachTarget = !query.navAgent.pathPending && query.navAgent.remainingDistance<=query.navAgent.stoppingDistance;
                var newPos = query.navAgent.transform.localPosition;
                // Debug.Log("newPos :"+newPos.x+" "+newPos.y+" "+newPos.z+" reach:"+isReachTarget+" remainDis:"+query.navAgent.remainingDistance+" stopDis:"+query.navAgent.stoppingDistance);
                query.isGrounded = query.charController.isGrounded;
                query.transform.localPosition = newPos;
                if (isReachTarget)
                {
                    query.StopFindWay();
                }
                else
                {
                    var nextTargetPos = query.navAgent.nextPosition;
                    var dir = nextTargetPos - query.navAgent.transform.position;
                    // Debug.Log("nextTargetPos : "+nextTargetPos.x+" "+nextTargetPos.y+" "+nextTargetPos.z+" dir:"+dir.x+" "+dir.y+" "+dir.z);
                    nextTargetPos = nextTargetPos + dir*10;//for rotation in MovementUpdateSystem only
                    // lastTargetPos.Value = nextTargetPos;
                    query.ownerGOE.EntityManager.SetComponentData<TargetPosition>(query.ownerGOE.Entity, new TargetPosition{Value=nextTargetPos});
                }
            }
        }
        entityArray.Dispose();
        // Profiler.EndSample();
    }
}
}