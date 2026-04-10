using Unity.Entities;
using UnityEngine;

namespace UnityMMO
{
    public struct UID : IComponentData
    {
        public long Value;
    }

    [DisallowMultipleComponent] 
    public class UIDProxy : MonoBehaviour 
    { 
        public long Value;

        class UIDProxyBaker : Baker<UIDProxy>
        {
            public override void Bake(UIDProxy authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UID { Value = authoring.Value });
            }
        }
    }

}