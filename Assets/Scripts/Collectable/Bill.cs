using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bill : Collectable {
    [SerializeField] private float _moneyAmount;

    public override void Collect() {
        Destroy(gameObject);
    }
}
