using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;

// [DisableAutoCreation]
public abstract partial class BaseComponentSystem : SystemBase
{
    protected BaseComponentSystem(GameWorld world)
    {
        m_world = world;
    }

    readonly protected GameWorld m_world;
}

// [DisableAutoCreation]
 public abstract partial class BaseComponentSystem<T1> : BaseComponentSystem
 	where T1 : MonoBehaviour
 {
 	EntityQuery Group;
 	protected ComponentType[] ExtraComponentRequirements;
	string name;

 	public BaseComponentSystem(GameWorld world) : base(world) {}

    protected override void OnCreate()
 	{
 		base.OnCreate();
		name = GetType().Name;
 		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
 		list.AddRange(new ComponentType[] { typeof(T1) } );
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
 		Group = GetEntityQuery(list.ToArray());
 	}
 
 	protected override void OnUpdate()
 	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.Temp);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], EntityManager.GetComponentObject<T1>(entityArray[i]));
		}
		entityArray.Dispose();
		Profiler.EndSample();
 	}
 	
 	protected abstract void Update(Entity entity,T1 data);
 }


// [DisableAutoCreation]
public abstract partial class BaseComponentSystem<T1,T2> : BaseComponentSystem
	where T1 : MonoBehaviour
	where T2 : MonoBehaviour
{
	EntityQuery Group;
	protected ComponentType[] ExtraComponentRequirements;
	string name; 
	
	public BaseComponentSystem(GameWorld world) : base(world) {}
	
	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
		list.AddRange(new ComponentType[] {typeof(T1), typeof(T2)});
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
		Group = GetEntityQuery(list.ToArray());
	}

	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], EntityManager.GetComponentObject<T1>(entityArray[i]),
			       EntityManager.GetComponentObject<T2>(entityArray[i]));
		}
		entityArray.Dispose();
		Profiler.EndSample();
	}
	
	protected abstract void Update(Entity entity,T1 data1,T2 data2);
}


// [DisableAutoCreation]
public abstract partial class BaseComponentSystem<T1,T2,T3> : BaseComponentSystem
	where T1 : MonoBehaviour
	where T2 : MonoBehaviour
	where T3 : MonoBehaviour
{
	EntityQuery Group;
	protected ComponentType[] ExtraComponentRequirements;
	string name;
	
	public BaseComponentSystem(GameWorld world) : base(world) {}
	
	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2), typeof(T3) } );
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
		Group = GetEntityQuery(list.ToArray());
	}

	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.TempJob);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], EntityManager.GetComponentObject<T1>(entityArray[i]),
			       EntityManager.GetComponentObject<T2>(entityArray[i]),
			       EntityManager.GetComponentObject<T3>(entityArray[i]));
		}
		entityArray.Dispose();
		Profiler.EndSample();
	}
	
	protected abstract void Update(Entity entity,T1 data1,T2 data2,T3 data3);
}

// [DisableAutoCreation]
public abstract partial class BaseComponentDataSystem<T1> : BaseComponentSystem
	where T1 : unmanaged,IComponentData
{
	EntityQuery Group;
	protected ComponentType[] ExtraComponentRequirements;
	string name;
	
	public BaseComponentDataSystem(GameWorld world) : base(world) {}
	
	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
		list.AddRange(new ComponentType[] { typeof(T1) } );
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
		Group = GetEntityQuery(list.ToArray());
	}

	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.Temp);
		var dataArray = Group.ToComponentDataArray<T1>(Allocator.Temp);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], dataArray[i]);
		}
		entityArray.Dispose();
		dataArray.Dispose();
		Profiler.EndSample();
	}
	
	protected abstract void Update(Entity entity,T1 data);
}

// [DisableAutoCreation]
public abstract partial class BaseComponentDataSystem<T1,T2> : BaseComponentSystem
	where T1 : unmanaged,IComponentData
	where T2 : unmanaged,IComponentData
{
	EntityQuery Group;
	protected ComponentType[] ExtraComponentRequirements;
	private string name;
	
	public BaseComponentDataSystem(GameWorld world) : base(world) {}
	
	protected override void OnCreate()
	{
		name = GetType().Name;
		base.OnCreate();
		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2) } );
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
		Group = GetEntityQuery(list.ToArray());
	}

	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.TempJob);
		var dataArray1 = Group.ToComponentDataArray<T1>(Allocator.TempJob);
		var dataArray2 = Group.ToComponentDataArray<T2>(Allocator.TempJob);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], dataArray1[i], dataArray2[i]);
		}

		entityArray.Dispose();
		dataArray1.Dispose();
		dataArray2.Dispose();
		Profiler.EndSample();
	}
	
	protected abstract void Update(Entity entity,T1 data1,T2 data2);
}

// [DisableAutoCreation]
public abstract partial class BaseComponentDataSystem<T1,T2,T3> : BaseComponentSystem
	where T1 : unmanaged,IComponentData
	where T2 : unmanaged,IComponentData
	where T3 : unmanaged,IComponentData
{
	EntityQuery Group;
	protected ComponentType[] ExtraComponentRequirements;
	string name;
	
	public BaseComponentDataSystem(GameWorld world) : base(world) {}
	
	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2), typeof(T3) } );
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
		Group = GetEntityQuery(list.ToArray());
	}

	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.TempJob);
		var dataArray1 = Group.ToComponentDataArray<T1>(Allocator.TempJob);
		var dataArray2 = Group.ToComponentDataArray<T2>(Allocator.TempJob);
		var dataArray3 = Group.ToComponentDataArray<T3>(Allocator.TempJob);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], dataArray1[i], dataArray2[i], dataArray3[i]);
		}
		
		entityArray.Dispose();
		dataArray1.Dispose();
		dataArray2.Dispose();
		dataArray3.Dispose();
		Profiler.EndSample();
	}
	
	protected abstract void Update(Entity entity,T1 data1,T2 data2,T3 data3);
}


// [DisableAutoCreation]
public abstract partial class BaseComponentDataSystem<T1,T2,T3,T4> : BaseComponentSystem
	where T1 : unmanaged,IComponentData
	where T2 : unmanaged,IComponentData
	where T3 : unmanaged,IComponentData
	where T4 : unmanaged,IComponentData
{
	EntityQuery Group;
	protected ComponentType[] ExtraComponentRequirements;
	string name;
	
	public BaseComponentDataSystem(GameWorld world) : base(world) {}
	
	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) } );
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
		Group = GetEntityQuery(list.ToArray());
	}

	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.TempJob);
		var dataArray1 = Group.ToComponentDataArray<T1>(Allocator.TempJob);
		var dataArray2 = Group.ToComponentDataArray<T2>(Allocator.TempJob);
		var dataArray3 = Group.ToComponentDataArray<T3>(Allocator.TempJob);
		var dataArray4 = Group.ToComponentDataArray<T4>(Allocator.TempJob);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], dataArray1[i], dataArray2[i], dataArray3[i], dataArray4[i]);
		}
		
		entityArray.Dispose();
		dataArray1.Dispose();
		dataArray2.Dispose();
		dataArray3.Dispose();
		dataArray4.Dispose();
		Profiler.EndSample();
	}
	
	protected abstract void Update(Entity entity,T1 data1,T2 data2,T3 data3,T4 data4);
}

// [DisableAutoCreation]
public abstract partial class BaseComponentDataSystem<T1,T2,T3,T4, T5> : BaseComponentSystem
	where T1 : unmanaged,IComponentData
	where T2 : unmanaged,IComponentData
	where T3 : unmanaged,IComponentData
	where T4 : unmanaged,IComponentData
	where T5 : unmanaged,IComponentData
{
	EntityQuery Group;
	protected ComponentType[] ExtraComponentRequirements;
	string name;
	
	public BaseComponentDataSystem(GameWorld world) : base(world) {}
	
	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		var list = new List<ComponentType>(6);
		if(ExtraComponentRequirements != null)		
			list.AddRange(ExtraComponentRequirements);
		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) } );
		// list.Add(ComponentType.Subtractive<DespawningEntity>());
		Group = GetEntityQuery(list.ToArray());
	}

	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var entityArray = Group.ToEntityArray(Allocator.Temp);
		var dataArray1 = Group.ToComponentDataArray<T1>(Allocator.Temp);
		var dataArray2 = Group.ToComponentDataArray<T2>(Allocator.Temp);
		var dataArray3 = Group.ToComponentDataArray<T3>(Allocator.Temp);
		var dataArray4 = Group.ToComponentDataArray<T4>(Allocator.Temp);
		var dataArray5 = Group.ToComponentDataArray<T5>(Allocator.Temp);

		for (var i = 0; i < entityArray.Length; i++)
		{
			Update(entityArray[i], dataArray1[i], dataArray2[i], dataArray3[i], dataArray4[i], dataArray5[i]);
		}
		
		entityArray.Dispose();
		dataArray1.Dispose();
		dataArray2.Dispose();
		dataArray3.Dispose();
		dataArray4.Dispose();
		dataArray5.Dispose();
		Profiler.EndSample();
	}
	
	protected abstract void Update(Entity entity,T1 data1,T2 data2,T3 data3,T4 data4, T5 data5);
}

// [DisableAutoCreation]
public abstract partial class InitializeComponentSystem<T> : BaseComponentSystem
	where T : MonoBehaviour
{
	struct SystemState : IComponentData {}
	EntityQuery IncomingGroup;
	string name;
	
	public InitializeComponentSystem(GameWorld world) : base(world) {}

	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		IncomingGroup = GetEntityQuery(typeof(T),ComponentType.Exclude<SystemState>());
	}
    
	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var incomingEntityArray = IncomingGroup.ToEntityArray(Allocator.Temp);
		if (incomingEntityArray.Length > 0)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			for (var i = 0; i < incomingEntityArray.Length; i++)
			{
				var entity = incomingEntityArray[i];
				ecb.AddComponent(entity, new SystemState());
				Initialize(entity, EntityManager.GetComponentObject<T>(entity));
			}
			ecb.Playback(EntityManager);
			ecb.Dispose();
		}
		incomingEntityArray.Dispose();
		Profiler.EndSample();
	}

	protected abstract void Initialize(Entity entity, T component);
}


// [DisableAutoCreation]
public abstract partial class InitializeComponentDataSystem<T,K> : BaseComponentSystem
	where T : unmanaged, IComponentData
	where K : unmanaged, IComponentData
{
	
	EntityQuery IncomingGroup;
	string name;
	
	public InitializeComponentDataSystem(GameWorld world) : base(world) {}

	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		IncomingGroup = GetEntityQuery(typeof(T),ComponentType.Exclude<K>());
	}
    
	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var incomingEntityArray = IncomingGroup.ToEntityArray(Allocator.Temp);
		if (incomingEntityArray.Length > 0)
		{
			var incomingComponentDataArray = IncomingGroup.ToComponentDataArray<T>(Allocator.Temp);
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			for (var i = 0; i < incomingComponentDataArray.Length; i++)
			{
				var entity = incomingEntityArray[i];
				ecb.AddComponent(entity, default(K));
				Initialize(entity, incomingComponentDataArray[i]);
			}
			incomingComponentDataArray.Dispose();
			ecb.Playback(EntityManager);
			ecb.Dispose();
		}
		incomingEntityArray.Dispose();
		Profiler.EndSample();
	}

	protected abstract void Initialize(Entity entity, T component);
}



// [DisableAutoCreation]
// [AlwaysUpdateSystem]
// public abstract class DeinitializeComponentSystem<T> : BaseComponentSystem
// 	where T : MonoBehaviour
// {
// 	EntityQuery OutgoingGroup;
// 	string name;

// 	public DeinitializeComponentSystem(GameWorld world) : base(world) {}

// 	protected override void OnCreate()
// 	{
// 		base.OnCreate();
// 		name = GetType().Name;
// 		// OutgoingGroup = GetComponentGroup(typeof(T), typeof(DespawningEntity));
// 	}
    
// 	protected override void OnUpdate()
// 	{
// 		Profiler.BeginSample(name);

// 		var outgoingComponentArray = OutgoingGroup.ToComponentArray<T>();
// 		var outgoingEntityArray = OutgoingGroup.ToEntityArray(Allocator.TempJob);
// 		for (var i = 0; i < outgoingComponentArray.Length; i++)
// 		{
// 			Deinitialize(outgoingEntityArray[i], outgoingComponentArray[i]);
// 		}
// 		outgoingEntityArray.Dispose();
// 		Profiler.EndSample();
// 	}

// 	protected abstract void Deinitialize(Entity entity, T component);
// }


// [DisableAutoCreation]
public abstract partial class DeinitializeComponentDataSystem<T> : BaseComponentSystem
	where T : unmanaged, IComponentData
{
	EntityQuery OutgoingGroup;
	string name;

	public DeinitializeComponentDataSystem(GameWorld world) : base(world) {}

	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		OutgoingGroup = GetEntityQuery(typeof(T), typeof(DespawningEntity));
	}
    
	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var outgoingComponentArray = OutgoingGroup.ToComponentDataArray<T>(Allocator.Temp);
		var outgoingEntityArray = OutgoingGroup.ToEntityArray(Allocator.Temp);
		for (var i = 0; i < outgoingComponentArray.Length; i++)
		{
			Deinitialize(outgoingEntityArray[i], outgoingComponentArray[i]);
		}
		outgoingComponentArray.Dispose();
		Profiler.EndSample();
	}

	protected abstract void Deinitialize(Entity entity, T component);
}

// [DisableAutoCreation]
public abstract partial class InitializeComponentGroupSystem<T,S> : BaseComponentSystem
	where T : MonoBehaviour
	where S : unmanaged, IComponentData
{
	EntityQuery IncomingGroup;
	string name;

	public InitializeComponentGroupSystem(GameWorld world) : base(world) {}

	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		IncomingGroup = GetEntityQuery(typeof(T),ComponentType.Exclude<S>());
	}
    
	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		var incomingEntityArray = IncomingGroup.ToEntityArray(Allocator.Temp);
		if (incomingEntityArray.Length > 0)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			for (var i = 0; i < incomingEntityArray.Length; i++)
				ecb.AddComponent(incomingEntityArray[i], new S());
			Initialize(ref IncomingGroup);
			ecb.Playback(EntityManager);
			ecb.Dispose();
		}
		incomingEntityArray.Dispose();
		Profiler.EndSample();
	}

	protected abstract void Initialize(ref EntityQuery group);
}



// [DisableAutoCreation]
public abstract partial class DeinitializeComponentGroupSystem<T> : BaseComponentSystem
	where T : MonoBehaviour
{
	EntityQuery OutgoingGroup;
	string name;

	public DeinitializeComponentGroupSystem(GameWorld world) : base(world) {}

	protected override void OnCreate()
	{
		base.OnCreate();
		name = GetType().Name;
		OutgoingGroup = GetEntityQuery(typeof(T), typeof(DespawningEntity));
	}
    
	protected override void OnUpdate()
	{
		Profiler.BeginSample(name);

		if (OutgoingGroup.CalculateEntityCount() > 0)
			Deinitialize(ref OutgoingGroup);
		
		Profiler.EndSample();
	}

	protected abstract void Deinitialize(ref EntityQuery group);
}