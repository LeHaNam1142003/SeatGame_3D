using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Vector3 _destination;
    private void OnEnable()
    {
        Observer.ShipMove += Move;
    }
    private void OnDisable()
    {
        Observer.ShipMove -= Move;
    }
    private void Start()
    {
        _destination = new Vector3(transform.position.x, transform.position.y, transform.position.z - 20);
    }
    void Move()
    {
        transform.DOMove(_destination, 3).OnComplete((() => Observer.OnWin?.Invoke()));
    }
}
