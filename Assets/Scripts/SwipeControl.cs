using UnityEngine;
using UnityEngine.UI;

public class SwipeControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject light;
    [SerializeField] private Image batteryFill;
    [SerializeField] private float batteryCapacity;
    [SerializeField] [Range(1, 2)] private float batteryRechargeRate;

    private Touch touch;
    private float flashLightBattery;
    private float coorX = 0f;
    private bool isTouchDevice;

    private void Start()
    {
        flashLightBattery = batteryCapacity;
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            isTouchDevice = true;
        }
        else
        {
            isTouchDevice = false;
        }
    }

    private void DesktopControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            coorX = Input.mousePosition.x;
            if (flashLightBattery > 1.5f)
                light.SetActive(true);
        }
        else if (Input.GetMouseButton(0))
        {
            float dx = Input.mousePosition.x - coorX;
            player.Rotate(0f, dx, 0f);
            coorX = Input.mousePosition.x;
            flashLightBattery -= Time.deltaTime;
            if (flashLightBattery <= 1)
                light.SetActive(false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            light.SetActive(false);
        }
        if (!Input.GetMouseButton(0))
            flashLightBattery += Time.deltaTime * batteryRechargeRate;

        if (flashLightBattery < 1)
            flashLightBattery = 1;
        if (flashLightBattery > batteryCapacity)
            flashLightBattery = batteryCapacity;

        batteryFill.rectTransform.localScale = new Vector3(1.06f * Mathf.InverseLerp(1, batteryCapacity, flashLightBattery), 1, 1);
    }

    private void TouchControls()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                coorX = touch.position.x;
                if (flashLightBattery > 1.5f)
                    light.SetActive(true);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                float dx = touch.position.x - coorX;
                player.Rotate(0f, dx, 0f);
                coorX = touch.position.x;
                flashLightBattery -= Time.deltaTime;
                if (flashLightBattery <= 1)
                    light.SetActive(false);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                light.SetActive(false);
            }
        }
        if (Input.touchCount == 0)
            flashLightBattery += Time.deltaTime * batteryRechargeRate;

        if (flashLightBattery < 1)
            flashLightBattery = 1;
        if (flashLightBattery > batteryCapacity)
            flashLightBattery = batteryCapacity;

        batteryFill.rectTransform.localScale = new Vector3(Mathf.InverseLerp(1, batteryCapacity, flashLightBattery), 1, 1);
    }

    private void Update()
    {
        if (isTouchDevice)
        {
            TouchControls();
        }
        else
        {
            DesktopControls();
        }
    }
}