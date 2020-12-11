using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(CustomEnemyPositions))]
public class CustomEnemyPositionsEditor : Editor
{
    CustomEnemyPositions CEP;
    private string[] SpawneeNames;
    private string[] EnemyData;
    int x = 0, y = 0;
    private bool editModeEnabled = true;

    private void OnEnable()
    {
        CEP = (CustomEnemyPositions)target;
        editModeEnabled = CEP.editModeEnabled;
        UpdateEnemyData();
        UpdateSpawneeNames();
    }

    private void UpdateSpawneeNames()
    {
        SpawneeNames = new string[CEP.spawnees.Count];
        for (int i = 0; i < CEP.spawnees.Count; i++)
        {
            SpawneeNames[i] = CEP.spawnees[i].enemyName;
        }
    }
    private void UpdateEnemyData()
    {
        EnemyData = new string[CEP.data.Count];
        for (int i = 0; i < CEP.data.Count; i++)
        {
            EnemyData[i] = CEP.data[i].enemyName;
        }
    }
    private void UpdateSpawneePositions()
    {
        CEP.UpdateSpawneePositions();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (EnemyData != null)
        {
            GUI.backgroundColor = Color.grey;
            x = GUILayout.SelectionGrid(x, EnemyData, CEP.data.Count);
        }

        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Update Enemy Data"))
        {
            UpdateEnemyData();
        }

        if (GUILayout.Button("Update Spawnee Data"))
        {
            UpdateSpawneeNames();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Enemy"))
        {
            CEP.AddEnemy(x);
            UpdateSpawneeNames();
        }
        if (GUILayout.Button("Remove Enemy"))
        {
            CEP.RemoveEnemy(y);
            UpdateSpawneeNames();
        }
        GUILayout.EndHorizontal();

        if (SpawneeNames != null)
        {
            y = GUILayout.SelectionGrid(y, SpawneeNames, 2);
            CEP.selectedSpawnee = y;
        }

        if (editModeEnabled)
        {
            UpdateSpawneePositions();
            if (GUILayout.Button("Save Positions"))
            {
                CEP.SavePositions();
                editModeEnabled = false;
            }
        }
        else
        {
            if (GUILayout.Button("Review Positions"))
            {
                CEP.ReviewPositions();
                editModeEnabled = true;
            }
        }
    }
}
