using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(CustomEnemyPositions))]
public class CustomEnemyPositionsEditor : Editor
{
    CustomEnemyPositions CEP;
    int x = 0;
    int y = 0;
    private string[] EnemyNames;
    private string[] EnemyData;
    bool isShiftDown = false;

    private void OnEnable()
    {
        CEP = (CustomEnemyPositions)target;
        UpdateEnemyNames();
        UpdateSpawneeNames();
    }

    private void UpdateSpawneeNames()
    {
        EnemyNames = new string[CEP.spawnees.Count];
        for (int i = 0; i < CEP.spawnees.Count; i++)
        {
            EnemyNames[i] = CEP.spawnees[i].enemyName;
        }
    }
    private void UpdateEnemyNames()
    {
        EnemyData = new string[CEP.data.Count];
        for (int i = 0; i < CEP.data.Count; i++)
        {
            EnemyData[i] = CEP.data[i].enemyName;
            if (i != CEP.data[i].enemyIndex)
            {
                Debug.Log("Enemy data is not sorted!");
                break;
            }
        }
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
            UpdateEnemyNames();
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
            CEP.RemoveEnemy(x);
            UpdateSpawneeNames();
        }
        GUILayout.EndHorizontal();

        if (EnemyNames != null)
        {
            y = GUILayout.SelectionGrid(y, EnemyNames, 2);
            CEP.selectedSpawnee = y;
        }

        var e = Event.current;
        if (e.keyCode == KeyCode.LeftShift || e.keyCode == KeyCode.RightShift)
        {
            isShiftDown = true;
        }
        if (e.rawType == EventType.KeyUp && (e.keyCode == KeyCode.LeftShift || e.keyCode == KeyCode.RightShift))
        {
            isShiftDown = false;
        }

        if (e != null && e.keyCode == KeyCode.W)
        {
            if (isShiftDown)
            {
                CEP.spawnees[y].MoveUp_();
            }
            CEP.spawnees[y].MoveUp();
        }
        if (e != null && e.keyCode == KeyCode.S)
        {
            if (isShiftDown)
            {
                CEP.spawnees[y].MoveDown_();
            }
            CEP.spawnees[y].MoveDown();
        }
        if (e != null && e.keyCode == KeyCode.A)
        {
            if (isShiftDown)
            {
                CEP.spawnees[y].MoveLeft_();
            }
            CEP.spawnees[y].MoveLeft();
        }
        if (e != null && e.keyCode == KeyCode.D)
        {
            if (isShiftDown)
            {
                CEP.spawnees[y].MoveRight_();
            }
            CEP.spawnees[y].MoveRight();
        }
    }
}
