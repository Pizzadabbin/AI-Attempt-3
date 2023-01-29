using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public enum Axis {
        X,Y,Z
    }
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Axis _axis;

    private Vector3 _rotateAxis = Vector3.zero;
    private void Start() {
        switch(_axis) {
            case Axis.X:
                _rotateAxis.x = _rotationSpeed;
                break;
            case Axis.Y:
                _rotateAxis.y = _rotationSpeed;
                break;
            case Axis.Z:
                _rotateAxis.z = _rotationSpeed;
                break;
        }
    }

    void FixedUpdate() {
        transform.Rotate(_rotateAxis);
    }
}
