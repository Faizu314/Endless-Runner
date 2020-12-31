using UnityEditor;
using UnityEngine;
public class EditorInput : EditorWindow
{
    private string inputString = new string(' ', 0);
    public CustomFormationEditor target;
    public bool mode;
    [MenuItem("Window/Enter Formation Name")]
    public static void ShowWindow()
    {
        GetWindow<EditorInput>("Enter Formation Name");
    }

    private void OnGUI()
    {
        inputString = EditorGUILayout.TextField("Formation Name: ", inputString);
        inputString.Trim(' ');
        if (GUILayout.Button("enter"))
        {
            if (inputString == null || inputString.Length == 0)
            {
                Debug.Log("Try inputting something this time");
                return;
            }
            if (mode)
            {
                target.AddFormation(inputString);
                Close();
            }
            else
            {
                target.RenameFormation(inputString);
                Close();
            }
        }
    }
}
