using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {
    public virtual void Collect() {
        Debug.Log("Default collect object; " + gameObject.name);
    }
}
