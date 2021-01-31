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
    [SerializeField] private float rangeMultiplier;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private float powerUpsTime;

    private float sensitivity;
    private Touch touch;
    private float flashLightBattery;
    private float coorX = 0f;
    private bool isTouchDevice;

    private float infiniteBattery = 0;
    private float longerRange = 0;
    private float moreDamage = 0;

    private void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
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
                _light.SetActive(true);
        }
        else if (Input.GetMouseButton(0))
        {
            float dx = Input.mousePosition.x - coorX;
            player.Rotate(0f, dx * sensitivity, 0f);
            coorX = Input.mousePosition.x;
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

        batteryFill.rectTransform.localScale = new Vector3(1.13f * Mathf.InverseLerp(1, batteryCapacity, flashLightBattery), 1.05f, 1);
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
                    _light.SetActive(true);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _light.SetActive(false);
            }
            else
            {
                float dx = touch.position.x - coorX;
                player.Rotate(0f, dx * sensitivity, 0f);
                coorX = touch.position.x;

                if (infiniteBattery == 0)
                    flashLightBattery -= Time.deltaTime;
                if (flashLightBattery <= 1)
                    _light.SetActive(false);
            }

        }
        if (Input.touchCount == 0)
            flashLightBattery += Time.deltaTime * batteryRechargeRate;

        if (flashLightBattery < 1)
            flashLightBattery = 1;
        if (flashLightBattery > batteryCapacity)
            flashLightBattery = batteryCapacity;

        batteryFill.rectTransform.localScale = new Vector3(Mathf.InverseLerp(1.13f, batteryCapacity, flashLightBattery), 1.05f, 1);
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
        }
        else
        {
            _light.transform.localScale = Vector3.Lerp(currentScale, new Vector3(1, 1, rangeMultiplier), Time.deltaTime);
        }
        if (moreDamage == 0)
            playerDamage = 1;
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
    }
    public void LongerRange()
    {
        longerRange = powerUpsTime;
    }
    public void MoreDamage()
    {
        playerDamage = damageMultiplier;
        moreDamage = powerUpsTime;
    }
}