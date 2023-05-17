using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Pancake;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Data/StateHardModeData")]
public class StateModeData : ScriptableObject
{
    [ReadOnly] public List<SetupStateMode> setStateModes;
    #if UNITY_EDITOR
    public void Clear()
    {
        setStateModes.Clear();
    }
  #endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(StateModeData), true)]
[CanEditMultipleObjects]
public class EditSateMode : Editor
{
    private StateModeData _stateModeData;
    private void OnEnable()
    {
        _stateModeData = target as StateModeData;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        if (GUILayout.Button("CLear", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            if (_stateModeData.setStateModes.Count != 0)
            {
                _stateModeData.Clear();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
[Serializable]
public class SetupStateMode
{
    public int modeIndex;
    public EStateMode eStateMode;
}
