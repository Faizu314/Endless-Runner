using UnityEngine;
using UnityEngine.UI;

public class DeletePlayerPrefs : MonoBehaviour
{
    public Text message;
    public void DeletePlayerMemory()
    {
        PlayerPrefs.DeleteAll();
        message.text = "Memory Cleared";
    }
}
