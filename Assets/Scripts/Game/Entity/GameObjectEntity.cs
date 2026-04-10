using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Compatibility shim replacing Unity.Entities.GameObjectEntity which was removed in Entities 1.0+.
/// Bridges GameObjects and ECS entities so managed MonoBehaviour components can be queried via EntityManager.
/// </summary>
public class GameObjectEntity : MonoBehaviour
{
    public Entity Entity { get; private set; }
    public EntityManager EntityManager { get; private set; }

    private static readonly List<GameObjectEntity> _all = new List<GameObjectEntity>();

    /// <summary>All active GameObjectEntity instances (for manual iteration in MonoBehaviour-based systems).</summary>
    public static IReadOnlyList<GameObjectEntity> All => _all;

    /// <summary>
    /// Replicates old Unity.Entities.GameObjectEntity.AddToEntityManager behavior:
    /// ensures the GameObject has a GameObjectEntity component and an ECS entity registered
    /// with all its MonoBehaviour components as managed component objects.
    /// </summary>
    public static void AddToEntityManager(EntityManager entityManager, GameObject go)
    {
        var goe = go.GetComponent<GameObjectEntity>();
        if (goe == null)
            goe = go.AddComponent<GameObjectEntity>();
        if (!entityManager.Exists(goe.Entity))
            goe.CreateEntity(entityManager);
    }

    internal void CreateEntity(EntityManager entityManager)
    {
        EntityManager = entityManager;
        Entity = entityManager.CreateEntity();
        // Register all components on this GameObject as managed ECS component objects
        // so EntityManager.GetComponentObject<T>(entity) works for Transform, MonoBehaviours, etc.
        foreach (var comp in GetComponents<Component>())
        {
            if (comp == null || comp is GameObjectEntity) continue;
            try { entityManager.AddComponentObject(Entity, comp); }
            catch { /* Skip if the type is already registered or unsupported */ }
        }
        Register();
    }

    private void Register()
    {
        if (!_all.Contains(this))
            _all.Add(this);
    }

    private void OnDestroy()
    {
        _all.Remove(this);
    }
}
