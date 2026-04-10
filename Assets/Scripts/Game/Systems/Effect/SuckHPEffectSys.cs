using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityMMO.Component;
using XLuaFramework;

namespace UnityMMO
{
    public class SuckHPEffect : MonoBehaviour {
        public long EndTime;
        public EffectStatus Status;
        public SuckHPEffect()
        {
            Status = EffectStatus.None;
            EndTime = 0;
        }
        public bool IsShowing()
        {
            return Status == EffectStatus.Rendering || Status == EffectStatus.WaitForRender;
        }
    }

    [DisableAutoCreation]
    partial class SuckHPEffectSys : BaseComponentSystem
    {
        EntityQuery group;
        
        public SuckHPEffectSys(GameWorld world) : base(world) {}
        
        protected override void OnCreate()
        {
            base.OnCreate();
            group = GetEntityQuery(typeof(SuckHPEffect), typeof(LooksInfo));
        }

        protected override void OnUpdate()
        {
            var entityArray = group.ToEntityArray(Allocator.TempJob);
            var looksInfos = group.ToComponentDataArray<LooksInfo>(Allocator.TempJob);
            for (var i = 0; i < entityArray.Length; i++)
            {
                HandleEffect(EntityManager.GetComponentObject<SuckHPEffect>(entityArray[i]), looksInfos[i]);
            }
            looksInfos.Dispose();
            entityArray.Dispose();
        }

        void HandleEffect(SuckHPEffect effect, LooksInfo looks)
        {
            if (effect.Status == EffectStatus.WaitForRender)
            {
                if (looks.CurState == LooksInfo.State.Loaded)
                {
                    var trans = EntityManager.GetComponentObject<Transform>(looks.LooksEntity);
                    EffectUtil.SetHitEffectColor(trans, new Color(1, 0.23f, 0, 1), true);
                    effect.Status = EffectStatus.Rendering;
                }
            }
            else if (effect.Status == EffectStatus.Rendering)
            {
                long curTime = TimeEx.ServerTime;
                if (curTime >= effect.EndTime)
                {
                    if (looks.CurState == LooksInfo.State.Loaded)
                    {
                        var trans = EntityManager.GetComponentObject<Transform>(looks.LooksEntity);
                        EffectUtil.SetHitEffectColor(trans, new Color(1, 0.23f, 0, 0), false);
                    }
                    effect.Status = EffectStatus.None;
                }
            }
        }
    }
}