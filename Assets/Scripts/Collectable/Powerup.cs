using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class Powerup : Collectable {
    [Min(0)] public float Duration;
    [Min(1)] public float Strength;

    public bool IsRandom;
    public PowerupType Type;

    private void Start() {
        if(IsRandom) {
            Type = (PowerupType)Enum.ToObject(typeof(PowerupType), Random.Range(0, 3));
        }
    }

    public override void Collect() {
        PlayerController.Instance.SetPowerup(Duration, Strength, Type);
        Destroy(gameObject);
    }
}

public enum PowerupType {
    Speed,
    AttackRange,
    Health
}