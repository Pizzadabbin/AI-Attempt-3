using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Punchable : MonoBehaviour
{
    [SerializeField] private UnityEvent<Transform, int> _onPunch;
    public void GetPunched(Transform sender, int damage) {
        _onPunch?.Invoke(sender, damage);
    }
}
