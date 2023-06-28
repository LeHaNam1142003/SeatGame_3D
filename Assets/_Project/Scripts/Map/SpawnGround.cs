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
    [SerializeField] Material oddMat;
    [SerializeField] Material evenMat;
#if UNITY_EDITOR
    public void Spawn()
    {
        var getSizeColumnUI = columnUI.transform.localScale;
        var getSizeRowUI = rowUI.transform.localScale;
        var getSizeGround = ground.transform.localScale;
        Clear();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var spawnGround = SpawnObj(ground).GetComponent<Ground>();
                spawnGround.transform.position = new Vector3(getSizeGround.x * j, 0, getSizeGround.z * i);
                if (i % 2 == 0)
                {
                    if (j % 2 == 0)
                    {
                        spawnGround.SetMatGround(evenMat);
                    }
                    else
                    {
                        spawnGround.SetMatGround(oddMat);
                    }
                }
                else
                {
                    if (j % 2 == 0)
                    {
                        spawnGround.SetMatGround(oddMat);
                    }
                    else
                    {
                        spawnGround.SetMatGround(evenMat);
                    }
                }

            }
        }
        for (int i = 0; i < rows ; i++)
        {
            var spawnLeftRowUI = SpawnObj(rowUI);
            spawnLeftRowUI.transform.position = new Vector3(-(getSizeGround.x / 2 + getSizeRowUI.x / 2), getSizeRowUI.y / 2, getSizeRowUI.z * i);

        }
        for (int i = 0; i < columns ; i++)
        {
            var spawnLeftColumnUI = SpawnObj(columnUI);
            spawnLeftColumnUI.transform.position = new Vector3(getSizeColumnUI.x * i - 1, getSizeColumnUI.y / 2, -(getSizeGround.z / 2 + getSizeColumnUI.z / 2));
        }
    }
    GameObject SpawnObj(GameObject getObj)
    {
        var spawnObj = (GameObject)PrefabUtility.InstantiatePrefab(getObj, this.transform);
        return spawnObj;
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
