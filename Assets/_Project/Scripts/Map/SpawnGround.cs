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
    [SerializeField] private int startPosix;
    [SerializeField] private int startPosiy;
    public Ground[,] elements;
    [SerializeField] private Ground[] saveGrid;
    private Passenger _passenger;
    private void OnEnable()
    {
        Observer.CalculatePath += CalculatePath;
        Observer.StartPoint += GetStartPoint;
    }
    private void OnDisable()
    {
        Observer.StartPoint -= GetStartPoint;
        Observer.CalculatePath -= CalculatePath;
    }
    void GetStartPoint(Ground startGround, Passenger getPassenger)
    {
        _passenger = getPassenger;
        startPosix = startGround.x;
        startPosiy = startGround.y;
    }
    public void Spawn()
    {
        var getSizeColumnUI = columnUI.transform.localScale;
        var getSizeRowUI = rowUI.transform.localScale;
        var getSizeGround = ground.transform.localScale;
        elements = new Ground[columns, rows];
        saveGrid = new Ground[columns * rows];
        Clear();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var spawnGround = SpawnObj(ground).GetComponent<Ground>();
                spawnGround.x = j;
                spawnGround.y = i;
                spawnGround.mark = -1;
                elements[j, i] = spawnGround;
                saveGrid[(i * columns) + j] = spawnGround;
                spawnGround.transform.position = new Vector3(getSizeGround.x * j, 0, getSizeGround.z * i);

            }
        }
        for (int i = 0; i < rows; i++)
        {
            var spawnLeftRowUI = SpawnObj(rowUI);
            spawnLeftRowUI.transform.position = new Vector3(-(getSizeGround.x / 2 + getSizeRowUI.x / 2), getSizeRowUI.y / 2, getSizeRowUI.z * i);

        }
        for (int i = 0; i < columns; i++)
        {
            var spawnLeftColumnUI = SpawnObj(columnUI);
            spawnLeftColumnUI.transform.position = new Vector3(getSizeColumnUI.x * i, getSizeColumnUI.y / 2, -(getSizeGround.z / 2 + getSizeColumnUI.z / 2));
        }
    }
    private void Start()
    {
        elements = new Ground[columns, rows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (saveGrid[(i * columns) + j])
                {
                    var getObj = saveGrid[(i * columns) + j];
                    elements[j, i] = getObj;
                }
            }
        }
    }
    bool CheckDirection(int x, int y, int mark, int direction, bool isSetPath)
    {
        // 1 is up 2 is down 3 is left 4 is right
        switch (direction)
        {
            case 1:
                if (y + 1 < rows && elements[x, y + 1] && elements[x, y + 1].mark == mark && elements[x, y + 1].isTaken == false)
                {
                    if (elements[x, y].isHaveSeat)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                    return false;
            case 2:
                if (y - 1 > -1 && elements[x, y - 1] && elements[x, y - 1].mark == mark && elements[x, y - 1].isTaken == false)
                {

                    if (elements[x, y - 1].isHaveSeat)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                    return false;
            case 3:
                if (x - 1 > -1 && elements[x - 1, y] && elements[x - 1, y].mark == mark && elements[x - 1, y].isTaken == false)
                {

                    return true;
                }
                else
                    return false;
            case 4:
                if (x + 1 < columns && elements[x + 1, y] && elements[x + 1, y].mark == mark && elements[x + 1, y].isTaken == false)
                {
                    return true;
                }
                else
                    return false;
        }
        return false;
    }
    void SetCheckDirection(int x, int y, int newMark)
    {
        if (CheckDirection(x, y, -1, 1, false))
        {
            SetMarked(x, y + 1, newMark);
        }
        if (CheckDirection(x, y, -1, 2, false))
        {
            SetMarked(x, y - 1, newMark);
        }
        if (CheckDirection(x, y, -1, 3, false))
        {
            SetMarked(x - 1, y, newMark);
        }
        if (CheckDirection(x, y, -1, 4, false))
        {
            SetMarked(x + 1, y, newMark);
        }
    }
    void SetMarked(int x, int y, int newMark)
    {
        if (elements[x, y])
        {
            elements[x, y].mark = newMark;
        }
    }
    void InitializeSetUp()
    {
        foreach (var element in elements)
        {
            if (element)
            {
                element.mark = -1;
            }
        }
        elements[startPosix, startPosiy].mark = 0;
    }
    void CalculatePath(Ground getGround)
    {
        InitializeSetUp();
        for (int newMark = 1; newMark < rows * columns; newMark++)
        {
            foreach (var elementCheck in elements)
            {
                if (elementCheck)
                {
                    if (elementCheck.mark == newMark - 1)
                    {
                        SetCheckDirection(elementCheck.x, elementCheck.y, newMark);
                    }
                }
            }
        }
        SetPath(getGround);
    }
    void SetPath(Ground getGround)
    {
        int checkMark;
        int endx = getGround.x;
        int endy = getGround.y;
        if (elements[endx, endy] && elements[endx, endy].mark < 0)
        {
            _passenger.SetEmotion(Emotion.Block);
        }
    }
    GameObject SpawnObj(GameObject getObj)
    {
        var spawnObj = Instantiate(getObj, this.transform);
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
