using System;
using UnityEngine;

namespace Cocos
{
    public class LuaFunc : ActionInterval
    {
        Action<Transform> startWithFunc;
        Action<float> updateFunc;

        public static LuaFunc Create(Action<Transform> startWithFunc, Action<float> updateFunc)
        {
            LuaFunc ret = new LuaFunc();
            if (ret != null && ret.Init(startWithFunc, updateFunc))
                return ret;
            return null;
        }

        public virtual bool Init(Action<Transform> startWithFunc, Action<float> updateFunc)
        {
            return true;
        }

        public override void StartWithTarget(Transform target)
        {
            this.target = target;
            this.originalTarget = target;
        }

        public override void Update(float progress)
        {
        }

        public override void Stop()
        {
        }

        public override bool IsDone()
        {
            return true;
        }

        public override Action Clone()
        {
            return null;
        }

        public override Action Reverse()
        {
            return null;
        }

    }
}