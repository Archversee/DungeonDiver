using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDunGen), true)]
public class RandomDunGenEditor : Editor
{
    AbstractDunGen generator;

    private void Awake()
    {
        generator = (AbstractDunGen)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Create Dungeon"))
        {
            generator.GenerateDungeon();
        }
    }
}
