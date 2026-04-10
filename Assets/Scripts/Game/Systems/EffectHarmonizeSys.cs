using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityMMO.Component;
using XLuaFramework;

namespace UnityMMO
{
    [DisableAutoCreation]
    partial class EffectHarmonizeSys : BaseComponentSystem
    {
        EntityQuery suckHPAndHitGroup;
        
        public EffectHarmonizeSys(GameWorld world) : base(world) {}
        
        protected override void OnCreate()
        {
            base.OnCreate();
            suckHPAndHitGroup = GetEntityQuery(typeof(SuckHPEffect), typeof(BeHitEffect));
        }

        protected override void OnUpdate()
        {
            var entityArray = suckHPAndHitGroup.ToEntityArray(Allocator.TempJob);
            for (var i = 0; i < entityArray.Length; i++)
            {
                HandleSuckHPAndHit(
                    EntityManager.GetComponentObject<SuckHPEffect>(entityArray[i]),
                    EntityManager.GetComponentObject<BeHitEffect>(entityArray[i]));
            }
            entityArray.Dispose();
        }

        void HandleSuckHPAndHit(SuckHPEffect suckHP, BeHitEffect beHit)
        {
            //如果同时有吸血和受击，那吸血优先
            if (suckHP.IsShowing() && beHit.IsShowing())
            {
                beHit.Status = EffectStatus.None;
            }
        }
    }
}