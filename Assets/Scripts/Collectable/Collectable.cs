using UnityEngine;
public class Collectable : MonoBehaviour {
    [SerializeField] private UnityEngine.Events.UnityEvent _onCollect;
    public virtual void Collect() {

    }
    public void CollectObject() {
        Collect();
        _onCollect?.Invoke();
        if(TryGetComponent<GameObjectSpawner>(out GameObjectSpawner gos)) {
            gos.Spawn();
        }
    }
}
