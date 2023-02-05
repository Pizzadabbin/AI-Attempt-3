using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour {
    [SerializeField] private GameObject[] _gameObjectsToSpawn;

    private readonly List<GameObject> _go = new();
    public void Spawn() {
        foreach(GameObject go in _gameObjectsToSpawn) {
            Instantiate(go);
            _go.Add(go);
        }
    }

    public void Spawn(Vector3 position, bool destroy = false, float destroyTime = 1) {
        foreach(GameObject go in _gameObjectsToSpawn) {
            if(destroy) {
                Destroy(Instantiate(go, position, Quaternion.identity), destroyTime);
            } else {
                Instantiate(go, position, Quaternion.identity);
            }
            _go.Add(go);
        }
    }

    public GameObject[] GetObjects() {
        return _go.ToArray();
    }
}