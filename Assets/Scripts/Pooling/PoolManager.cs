using DevCase.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DevCase.Pooling
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private GameObject _gameobjectsParent;
        private GameObject _particlesParent;
        private GameObject _soundsParent;
        private Dictionary<GameObject, ObjectPool<GameObject>> _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        private Dictionary<GameObject, GameObject> _poolObjects = new Dictionary<GameObject, GameObject>();
        public void Initialize()
        {
            SetupParents();
        }
        private void SetupParents()
        {
            _gameobjectsParent = new GameObject("GameObject Pools");
            _particlesParent = new GameObject("Particle Pools");
            _soundsParent = new GameObject("Sound Pools");
            _gameobjectsParent.transform.SetParent(transform);
            _particlesParent.transform.SetParent(transform);
            _soundsParent.transform.SetParent(transform);
        }
        private void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation, PoolObjectTypes poolType = PoolObjectTypes.GameObject)
        {
            ObjectPool<GameObject> newpPool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, position, rotation, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject
            );
            _objectPools.Add(prefab, newpPool);
        }
        private void CreatePool(GameObject prefab, PoolObjectTypes poolType = PoolObjectTypes.GameObject)
        {
            ObjectPool<GameObject> newpPool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject
            );
            _objectPools.Add(prefab, newpPool);
        }
        private GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, PoolObjectTypes poolType = PoolObjectTypes.GameObject)
        {
            prefab.SetActive(false);
            GameObject newObject = Instantiate(prefab, position, rotation);
            prefab.SetActive(true);
            GameObject parentObject = poolType switch
            {
                PoolObjectTypes.GameObject => _gameobjectsParent,
                PoolObjectTypes.Particle => _particlesParent,
                PoolObjectTypes.Sound => _soundsParent,
                _ => _gameobjectsParent
            };
            newObject.transform.SetParent(parentObject.transform);
            return newObject;
        }
        private GameObject CreateObject(GameObject prefab, PoolObjectTypes poolType = PoolObjectTypes.GameObject)
        {
            prefab.SetActive(false);
            GameObject newObject = Instantiate(prefab);
            prefab.SetActive(true);
            GameObject parentObject = poolType switch
            {
                PoolObjectTypes.GameObject => _gameobjectsParent,
                PoolObjectTypes.Particle => _particlesParent,
                PoolObjectTypes.Sound => _soundsParent,
                _ => _gameobjectsParent
            };
            newObject.transform.SetParent(parentObject.transform);
            return newObject;
        }
        private void OnGetObject(GameObject obj)
        {
            if(obj.TryGetComponent<IPoolElement>(out var poolElement))
            {
                poolElement.OnSpawned();
            }
        }
        private void OnReleaseObject(GameObject obj)
        {
            if(obj.TryGetComponent<IPoolElement>(out var poolElement))
            {
                poolElement.OnDespawned();
            }
            obj.SetActive(false);
        }
        private void OnDestroyObject(GameObject obj)
        {
            if(obj.TryGetComponent<IPoolElement>(out var poolElement))
            {
                poolElement.OnDespawned();
            }
            if(_poolObjects.ContainsKey(obj))
            {
                _poolObjects.Remove(obj);
            }
        }
        private T SpawnObject<T>(GameObject prefab, Vector3 position, Quaternion rotation, PoolObjectTypes poolType = PoolObjectTypes.GameObject) where T : Object
        {
            if (!_objectPools.ContainsKey(prefab))
            {
                CreatePool(prefab, position, rotation, poolType);
            }
            GameObject newObject = _objectPools[prefab].Get();
            if(newObject != null)
            {
                if(!_poolObjects.ContainsKey(newObject))
                {
                    _poolObjects.Add(newObject, prefab);
                }
                newObject.transform.position = position;
                newObject.transform.rotation = rotation;
                newObject.SetActive(true);
                if(typeof(T) == typeof(GameObject))
                {
                    return newObject as T;
                }

                T component = newObject.GetComponent<T>();
                if(component == null)
                {
                    Debug.LogWarning($"PoolManager: The spawned object does not have component of type {typeof(T).Name}");
                    return null;
                }
                return component;
            }
            Debug.LogWarning($"{prefab.GetType().Name}): Failed to spawn object from pool.");
            return null;
        }
        private T SpawnObject<T>(GameObject prefab,PoolObjectTypes poolType = PoolObjectTypes.GameObject) where T : Object
        {
            if (!_objectPools.ContainsKey(prefab))
            {
                CreatePool(prefab, poolType);
            }
            GameObject newObject = _objectPools[prefab].Get();
            if(newObject != null)
            {
                if(!_poolObjects.ContainsKey(newObject))
                {
                    _poolObjects.Add(newObject, prefab);
                }
                newObject.SetActive(true);
                if(typeof(T) == typeof(GameObject))
                {
                    return newObject as T;
                }

                T component = newObject.GetComponent<T>();
                if(component == null)
                {
                   Debug.LogWarning($"PoolManager: The spawned object does not have component of type {typeof(T).Name}");
                   return null;
                }
                return component;
            }
            Debug.LogWarning($"{prefab.GetType().Name}): Failed to spawn object from pool.");
            return null;
        }
        public T SpawnObject<T>(T prefab, Vector3 position, Quaternion rotation, PoolObjectTypes poolType = PoolObjectTypes.GameObject) where T : Component
        {
            return SpawnObject<T>(prefab.gameObject, position, rotation, poolType);
        }
        public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, PoolObjectTypes poolType = PoolObjectTypes.GameObject)
        {
            return SpawnObject<GameObject>(prefab, position, rotation, poolType);
        }
        public T SpawnObject<T>(T prefab, PoolObjectTypes poolType = PoolObjectTypes.GameObject) where T : Component
        {
            return SpawnObject<T>(prefab.gameObject, poolType);
        }
        public GameObject SpawnObject(GameObject prefab,PoolObjectTypes poolType = PoolObjectTypes.GameObject)
        {
            return SpawnObject<GameObject>(prefab, poolType);
        }
        public void DespawnObject(GameObject obj, PoolObjectTypes poolType = PoolObjectTypes.GameObject)
        {
            if(_poolObjects.TryGetValue(obj, out GameObject prefab))
            {
                GameObject parentObject = poolType switch
                {
                    PoolObjectTypes.GameObject => _gameobjectsParent,
                    PoolObjectTypes.Particle => _particlesParent,
                    PoolObjectTypes.Sound => _soundsParent,
                    _ => _gameobjectsParent
                };
                obj.transform.SetParent(parentObject.transform);
                if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                {
                    pool.Release(obj);
                }
            }
            else
            {
                Debug.LogWarning("PoolManager: Attempting to despawn an object that is not managed by the pool.");
                Destroy(obj);
            }
        }
    }
    public enum PoolObjectTypes
    {
        GameObject,
        Particle,
        Sound
    }
}
