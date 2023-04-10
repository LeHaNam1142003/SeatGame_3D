using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnGround : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject rowUI;
    [SerializeField] private GameObject columnUI;
#if UNITY_EDITOR
    public void Spawn()
    {
        Clear();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var spawnGround = (GameObject)PrefabUtility.InstantiatePrefab(ground, this.transform);
                spawnGround.transform.position = new Vector3(spawnGround.transform.localScale.x * j, 0, spawnGround.transform.localScale.z * i);
            }
        }
    }
    public void Clear()
    {
        if (transform.childCount == 0) return;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var getChild = transform.GetChild(i);
            if (getChild.gameObject.activeInHierarchy)
            {
                DestroyImmediate(getChild.gameObject);
            }
        }
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(SpawnGround), true)]
[CanEditMultipleObjects]
public class SpawnGroundEditor : Editor
{
    private SpawnGround _spawnGround;
    private void OnEnable()
    {
        _spawnGround = target as SpawnGround;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        if (GUILayout.Button("Create", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            _spawnGround.Spawn();
        }
        if (GUILayout.Button("CLear", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            _spawnGround.Clear();
        }
        serializedObject.ApplyModifiedProperties();
    }

}
#endif
