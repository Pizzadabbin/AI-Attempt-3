using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bill : Collectable {
    [SerializeField] private float _moneyAmount;

    public override void Collect() {
        Debug.Log("Collected a Bill with: " + _moneyAmount + " value");
        Destroy(gameObject);
    }
}
