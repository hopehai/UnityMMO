using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityMMO.Component;
using XLuaFramework;

namespace UnityMMO
{
    public enum EffectStatus
    {
        WaitForRender,
        Rendering,
        None,
    }
    public class ParticleEffect
    {
        public long EndTime;
        public EffectStatus Status;
        public ParticleEffect()
        {
            Status = EffectStatus.None;
            EndTime = 0;
        }
        public bool IsShowing()
        {
            return Status == EffectStatus.Rendering || Status == EffectStatus.WaitForRender;
        }
    } 
    public class ParticleEffects : MonoBehaviour {
        List<ParticleEffect> Effects;
    }

    [DisableAutoCreation]
    partial class ParticleEffectSys : BaseComponentSystem
    {
        EntityQuery group;
        
        public ParticleEffectSys(GameWorld world) : base(world) {}
        
        protected override void OnCreate()
        {
            base.OnCreate();
            group = GetEntityQuery(typeof(ParticleEffects), typeof(LooksInfo));
        }

        protected override void OnUpdate()
        {
            var entityArray = group.ToEntityArray(Allocator.TempJob);
            var looksInfos = group.ToComponentDataArray<LooksInfo>(Allocator.TempJob);
            for (var i = 0; i < entityArray.Length; i++)
            {
                HandleEffect(EntityManager.GetComponentObject<ParticleEffects>(entityArray[i]), looksInfos[i]);
            }
            looksInfos.Dispose();
            entityArray.Dispose();
        }

        void HandleEffect(ParticleEffects effect, LooksInfo looks)
        {

        }
    }

}