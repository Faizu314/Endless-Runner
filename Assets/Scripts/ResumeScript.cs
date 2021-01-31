using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumeScript : MonoBehaviour
{
    public string toLoad;
    public void Resume()
    {
        SceneManager.LoadScene(toLoad);
    }
}
