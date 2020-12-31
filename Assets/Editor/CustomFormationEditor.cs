using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomFormations))]
public class CustomFormationEditor : Editor
{
    CustomFormations CF;
    private string[] memberNames;
    int memberSelected = 0;
    private string[] formationNames;
    int formationSelected = -1;
    private bool editModeEnabled = false;

    private void OnEnable()
    {
        CF = (CustomFormations)target;
        editModeEnabled = CF.editModeEnabled;
        UpdateMemberNames();
        UpdateFormationNames();
        CF.UpdateFormations();
    }

    private void UpdateMemberNames()
    {
        if (memberNames != null)
        {
            for (int i = 0; i < memberNames.Length; i++)
            {
                memberNames[i] = null;
            }
            memberNames = null;
        }
        CF.UpdateEnemyData();
        memberNames = new string[CF.memberData.Count];
        for (int i = 0; i < memberNames.Length; i++)
        {
            memberNames[i] = CF.memberData[i].memberName;
        }
    }

    private void UpdateFormationNames()
    {
        if (formationNames != null)
        {
            for (int i = 0; i < formationNames.Length; i++)
            {
                formationNames[i] = null;
            }
            formationNames = null;
        }
        formationNames = CF.GetFormationNames();
    }

    public void AddFormation(string name)
    {
        CF.AddFormation(name);
        UpdateFormationNames();
    }

    public void RenameFormation(string name)
    {
        CF.RenameFormation(formationSelected, name);
        UpdateFormationNames();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Update Member Data"))
        {
            UpdateMemberNames();
        }
        if (GUILayout.Button("Update Formation Data"))
        {
            UpdateFormationNames();
        }

        if (!editModeEnabled)
        {
            if (GUILayout.Button("Review Formations"))
            {
                CF.ReviewFormations();
                editModeEnabled = true;
            }
            if (CF.randomGeneration)
            {
                if (GUILayout.Button("Connect to Spawner"))
                {
                    CF.ConnectToSpawner();
                }
            }
            return;
        }

        if (memberNames != null)
        {
            GUI.backgroundColor = Color.grey;
            memberSelected = GUILayout.SelectionGrid(memberSelected, memberNames, CF.memberData.Count);
        }
        if (formationNames != null)
        {
            formationSelected = GUILayout.SelectionGrid(formationSelected, formationNames, 4);
        }

        GUI.backgroundColor = Color.white;
        CF.UpdateFormations();

        if (GUILayout.Button("Add Formation"))
        {
            EditorInput input = new EditorInput();
            input.mode = true;
            input.target = this;
            input.Show();
            input.Focus();
        }
        if (formationNames != null && formationNames.Length != 0 && formationSelected != -1)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Rename Formation"))
            {
                EditorInput input = new EditorInput();
                input.mode = false;
                input.target = this;
                input.Show();
                input.Focus();
            }
            if (CF.openedFormations.Contains(formationSelected))
            {
                if (GUILayout.Button("Save Formation"))
                {
                    CF.SaveFormation(formationSelected);
                }
            }
            else
            {
                if (GUILayout.Button("Edit Formation"))
                {
                    CF.EditFormation(formationSelected);
                }
            }
            if (GUILayout.Button("Add Member"))
            {
                CF.AddMember(formationSelected, memberSelected);
            }
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Save"))
        {
            CF.Save();
            UpdateFormationNames();
            editModeEnabled = false;
        }
    }
}
