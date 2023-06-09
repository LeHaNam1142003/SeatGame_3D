using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraWin : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _rotateCamera = new Vector3(0, 70, 0);
    private Vector3 _moveCamera;
    private void OnEnable()
    {
        Observer.IntroWinGame += IntroWinGame;
    }
    private void OnDisable()
    {
        Observer.IntroWinGame -= IntroWinGame;
    }
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _moveCamera = new Vector3(-9, 8, -1);
    }
    void IntroWinGame()
    {
        _camera.orthographic = false;
        transform.DORotate(_rotateCamera, 2).OnUpdate((() =>
        {
            transform.DOLocalMove(_moveCamera, 2);
        })).OnComplete((() => Observer.DoneLevel?.Invoke()));
        ;
    }
}
