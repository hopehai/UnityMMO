using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace UnityMMO{
    public class MainWorld : MonoBehaviour
    {
        private MainWorld(){}
        public static MainWorld Instance = null;
        GameWorld m_GameWorld;
        SystemCollection m_Systems;

        private void Awake() {
            Instance = this;
        }

        public void Initialize() {
            m_GameWorld = new GameWorld("ClientWorld");
            TimelineManager.GetInstance().Init();
            SceneMgr.Instance.Init(m_GameWorld);
            SynchFromNet.Instance.Init();
            ECSHelper.entityMgr = m_GameWorld.GetEntityManager();
            InitializeSystems();
        }

        public void InitializeSystems() {
            m_Systems = new SystemCollection();
            //玩家输入
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new PlayerInputSystem()));
            //管理角色的外观信息获取和模型加载
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new HandleRoleLooks(m_GameWorld)));
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new HandleRoleLooksNetRequest(m_GameWorld)));
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new HandleRoleLooksSpawnRequests(m_GameWorld)));
            //从输入获取主角的目标坐标
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new CreateTargetPosFromUserInputSystem(m_GameWorld)));
            //角色移动相关
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new MovementUpdateSystem(m_GameWorld)));
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new HandleMovementQueries(m_GameWorld)));
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new MovementHandleGroundCollision(m_GameWorld)));
            //判定所有节点和地面的关系
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new GroundTestSystem(m_GameWorld)));
            //上传角色坐标信息
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new UploadMainRolePosSystem(m_GameWorld)));
            // m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new SkillSpawnSystem(m_GameWorld)));
            //管理 Timeline
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new TimelineSpawnSystem(m_GameWorld)));
            //管理状态 LocomotionState
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new LocomotionStateSys(m_GameWorld)));
            //管理 Animator
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new UpdateAnimatorSystem(m_GameWorld)));
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new ResetPosOffsetSystem(m_GameWorld)));
            //管理所有名字和血量条：Nameboard 对象
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new NameboardSystem(m_GameWorld)));
            //重置所有动作
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new ActionDataResetSystem(m_GameWorld)));
            //协调处理各种粒子或 shader 特效，解决特效间的冲突关系
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new EffectHarmonizeSys(m_GameWorld)));
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new BeHitEffectSys(m_GameWorld)));
            m_Systems.Add(m_GameWorld.GetECSWorld().AddSystemManaged(new SuckHPEffectSys(m_GameWorld)));
        }

        public void StartGame() {
            Initialize();
            if (GameVariable.IsSingleMode)
            {
                SceneMgr.Instance.AddMainRole(1, 1, "testRole", 2, Vector3.zero, 100, 100);
                SceneMgr.Instance.LoadScene(1001);
            }
            else
            {
                //开始从后端请求场景信息，一旦开启就会在收到回复时再次请求
                SynchFromNet.Instance.StartSynchFromNet();
            }
        }

        private void Update() {
            // Debug.Log("main world update");
            m_Systems.Update();
            GameInput.GetInstance().Reset();
            m_GameWorld.ProcessDespawns();
        }

    }

}