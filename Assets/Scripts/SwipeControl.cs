using UnityEngine;
using UnityEngine.UI;

public class SwipeControl : MonoBehaviour
{
    public static float playerDamage = 1f;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject _light;
    [SerializeField] private Image batteryFill;
    [SerializeField] private float batteryCapacity;
    [SerializeField] [Range(1, 2)] private float batteryRechargeRate;

    [Space(10)] [Header("PowerUps")]
    [SerializeField] [Range(1, 2)] private float rangeMultiplier;
    [SerializeField] [Range(1, 5)] private float damageMultiplier;
    [SerializeField] private float powerUpsTime;

    [Space(10)] [Header("PowerUps Indicators")]
    [SerializeField] private Text infiniteBatteryIndicator;
    [SerializeField] private Text longerRangeIndicator;
    [SerializeField] private Text moreDamageIndicator;

    private float sensitivity;
    private Touch touch;
    private float flashLightBattery;
    private float coorX = 0f;
    private float coorY = 0f;
    private bool isTouchDevice;

    private float infiniteBattery = 0;
    private float longerRange = 0;
    private float moreDamage = 0;

    private void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        if (sensitivity == 0)
            sensitivity = 0.3f;
        flashLightBattery = batteryCapacity;
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            isTouchDevice = true;
        }
        else
        {
            isTouchDevice = false;
        }
        infiniteBatteryIndicator.enabled = false;
        longerRangeIndicator.enabled = false;
        moreDamageIndicator.enabled = false;
    }

    private void DesktopControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            coorX = Input.mousePosition.x;
            coorY = Input.mousePosition.y;
            if (flashLightBattery > 1.5f)
                _light.SetActive(true);
        }
        else if (Input.GetMouseButton(0))
        {
            float dx = Input.mousePosition.x - coorX + (Input.mousePosition.y - coorY) * 0.5f;
            player.Rotate(0f, dx * sensitivity, 0f);
            coorX = Input.mousePosition.x;
            coorY = Input.mousePosition.y;
            if (infiniteBattery == 0)
                flashLightBattery -= Time.deltaTime;
            if (flashLightBattery <= 1)
                _light.SetActive(false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _light.SetActive(false);
        }
        if (!Input.GetMouseButton(0))
            flashLightBattery += Time.deltaTime * batteryRechargeRate;

        if (flashLightBattery < 1)
            flashLightBattery = 1;
        if (flashLightBattery > batteryCapacity)
            flashLightBattery = batteryCapacity;

        batteryFill.rectTransform.localScale = new Vector3(1.07f * Mathf.InverseLerp(1, batteryCapacity, flashLightBattery), 1.04f, 1);
    }

    private void TouchControls()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    coorX = touch.position.x;
                    coorY = touch.position.y;
                    if (flashLightBattery > 1.5f)
                        _light.SetActive(true);
                    break;
                case TouchPhase.Ended:
                    _light.SetActive(false);
                    break;
                default:
                    float dx = touch.position.x - coorX + (touch.position.y - coorY) * 0.5f;
                    player.Rotate(0f, dx * sensitivity, 0f);
                    coorX = touch.position.x;
                    coorY = touch.position.y;
                    if (infiniteBattery == 0)
                        flashLightBattery -= Time.deltaTime;
                    if (flashLightBattery <= 1)
                        _light.SetActive(false);
                    break;
            }
        }
        if (Input.touchCount == 0)
            flashLightBattery += Time.deltaTime * batteryRechargeRate;

        if (flashLightBattery < 1)
            flashLightBattery = 1;
        if (flashLightBattery > batteryCapacity)
            flashLightBattery = batteryCapacity;

        batteryFill.rectTransform.localScale = new Vector3(1.07f * Mathf.InverseLerp(1f, batteryCapacity, flashLightBattery), 1.04f, 1);
    }

    private void DecrementPowerUpsTime()
    {
        if (moreDamage != 0)
        {
            moreDamage -= Time.deltaTime;
            if (moreDamage < 0)
                moreDamage = 0;
        }
        if (longerRange != 0)
        {
            longerRange -= Time.deltaTime;
            if (longerRange < 0)
                longerRange = 0;
        }
        if (infiniteBattery != 0)
        {
            infiniteBattery -= Time.deltaTime;
            if (infiniteBattery < 0)
                infiniteBattery = 0;
        }
    }

    private void ApplyPowerUps()
    {
        Vector3 currentScale = _light.transform.localScale;
        if (longerRange == 0)
        {
            _light.transform.localScale = Vector3.Lerp(currentScale, Vector3.one, Time.deltaTime);
            longerRangeIndicator.enabled = false;
        }
        else
        {
            _light.transform.localScale = Vector3.Lerp(currentScale, new Vector3(1, 1, rangeMultiplier), Time.deltaTime);
        }
        if (moreDamage == 0)
        {
            playerDamage = 1;
            moreDamageIndicator.enabled = false;
        }
        if (infiniteBattery == 0)
            infiniteBatteryIndicator.enabled = false;
    }

    private void Update()
    {
        if (isTouchDevice)
            TouchControls();
        else
            DesktopControls();

        ApplyPowerUps();
        DecrementPowerUpsTime();
    }

    public void InfiniteBattery()
    {
        infiniteBattery = powerUpsTime;
        infiniteBatteryIndicator.enabled = true;
    }
    public void LongerRange()
    {
        longerRange = powerUpsTime;
        longerRangeIndicator.enabled = true;
    }
    public void MoreDamage()
    {
        playerDamage = damageMultiplier;
        moreDamage = powerUpsTime;
        moreDamageIndicator.enabled = true;
    }
}