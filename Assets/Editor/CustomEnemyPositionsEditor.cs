using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomEnemyPositions))]
public class CustomEnemyPositionsEditor : Editor
{
    CustomEnemyPositions CEP;
    private string[] EnemyData;
    int x = 0;
    private bool editModeEnabled = true;

    private void OnEnable()
    {
        CEP = (CustomEnemyPositions)target;
        editModeEnabled = CEP.editModeEnabled;
        UpdateEnemyData();
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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Enemy"))
        {
            CEP.AddEnemy(x);
        }
        GUILayout.EndHorizontal();

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
