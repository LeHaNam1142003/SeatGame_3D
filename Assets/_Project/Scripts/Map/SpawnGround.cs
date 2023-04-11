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
        var getSizeGroundX = ground.transform.localScale.x;
        var getSizeGroundY = ground.transform.localScale.y;
        var getSizeGroundZ = ground.transform.localScale.z;
        var getSizeRowUIX = rowUI.transform.localScale.x;
        var getSizeRowUIY = rowUI.transform.localScale.y;
        var getSizeRowUIZ = rowUI.transform.localScale.z;
        var getSizeColumnUIX = columnUI.transform.localScale.x;
        var getSizeColumnUIY = columnUI.transform.localScale.y;
        var getSizeColumnUIZ = columnUI.transform.localScale.z;
        Clear();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var spawnGround = SpawnObj(ground).GetComponent<Ground>();
                spawnGround.transform.position = new Vector3(getSizeGroundX * j, 0, getSizeGroundZ * i);
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
        for (int i = 0; i <= rows + 1; i++)
        {
            var spawnLeftRowUI = SpawnObj(rowUI);
            var spawnRightRowUI = SpawnObj(rowUI);
            spawnLeftRowUI.transform.position = new Vector3(-(getSizeGroundX / 2 + getSizeRowUIX / 2), getSizeRowUIY / 2, getSizeRowUIZ * i - 1);
            spawnRightRowUI.transform.position = new Vector3((getSizeGroundX * columns - getSizeGroundX / 2 + getSizeRowUIX / 2), getSizeRowUIY / 2, getSizeRowUIZ * i - 1);
            spawnRightRowUI.transform.eulerAngles = new Vector3(0, 180, 0);

        }
        for (int i = 0; i <= columns + 1; i++)
        {
            var spawnLeftColumnUI = SpawnObj(columnUI);
            var spawnRightColumnUI = SpawnObj(columnUI);
            spawnLeftColumnUI.transform.position = new Vector3(getSizeColumnUIX * i - 1, getSizeColumnUIY / 2, -(getSizeGroundZ / 2 + getSizeColumnUIZ / 2));
            spawnRightColumnUI.transform.position = new Vector3(getSizeColumnUIX * i - 1, getSizeColumnUIY / 2, (getSizeGroundZ * rows - getSizeGroundZ / 2 + getSizeColumnUIZ / 2));
            spawnRightColumnUI.transform.eulerAngles = new Vector3(0, 180, 0);
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
