using UnityEngine;
using UnityEngine.UI;

public class SetSensitivity : MonoBehaviour
{
    public Slider slider;
    public Text message;

    private void Awake()
    {
        slider.value = PlayerPrefs.GetFloat("Sensitivity");
    }
    public void SaveSensitivity()
    {
        PlayerPrefs.SetFloat("Sensitivity", slider.value);
        message.text = "Sensitivity set to " + slider.value;
    }
}
