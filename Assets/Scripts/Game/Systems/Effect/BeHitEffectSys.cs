using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityMMO.Component;
using XLuaFramework;

namespace UnityMMO
{
    public class BeHitEffect : MonoBehaviour {
        public long EndTime;
        public EffectStatus Status;
        public BeHitEffect()
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
    partial class BeHitEffectSys : BaseComponentSystem
    {
        EntityQuery group;
        
        public BeHitEffectSys(GameWorld world) : base(world) {}
        
        protected override void OnCreate()
        {
            base.OnCreate();
            group = GetEntityQuery(typeof(BeHitEffect), typeof(LooksInfo));
        }

        protected override void OnUpdate()
        {
            var entityArray = group.ToEntityArray(Allocator.TempJob);
            var looksInfos = group.ToComponentDataArray<LooksInfo>(Allocator.TempJob);
            for (var i = 0; i < entityArray.Length; i++)
            {
                HandleEffect(EntityManager.GetComponentObject<BeHitEffect>(entityArray[i]), looksInfos[i]);
            }
            looksInfos.Dispose();
            entityArray.Dispose();
        }

        void HandleEffect(BeHitEffect effect, LooksInfo looks)
        {
            if (effect.Status == EffectStatus.WaitForRender)
            {
                if (looks.CurState == LooksInfo.State.Loaded)
                {
                    var trans = EntityManager.GetComponentObject<Transform>(looks.LooksEntity);
                    EffectUtil.SetHitEffectColor(trans, new Color(1, 1, 1, 1), true);
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
                        EffectUtil.SetHitEffectColor(trans, new Color(1, 1, 1, 0), false);
                    }
                    effect.Status = EffectStatus.None;
                }
            }
        }
    }
}