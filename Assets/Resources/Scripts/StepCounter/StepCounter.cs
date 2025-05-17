using UnityEngine;
using System;
using UnityEngine.UI;

public class StepCounter : MonoBehaviour
{
    [Header("UI References")]
    public Text StepsText;
    public Text DistanceText;
    
    [Header("Configuration")]
    public StepCounterConfig config;
    
    [Header("Runtime Variables")]
    [SerializeField] private float distanceWalked = 0f;
    [SerializeField] private int stepCount = 0;
    
    private Vector3 acceleration;
    private Vector3 prevAcceleration;
    
    // Singleton setup
    private static StepCounter _instance;
    public static StepCounter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StepCounter>();
                if (_instance == null)
                {
                    GameObject container = new GameObject("StepCounter");
                    _instance = container.AddComponent<StepCounter>();
                }
            }
            return _instance;
        }
    }

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("StepCounterConfig is missing!");
            return;
        }
        prevAcceleration = Input.acceleration;
        InitializeUI();
        StepDataHandler.Instance.CheckForNewDay();
    }

    private void Update()
    {
        if (config == null) return;
        
        DetectSteps();
        CalculateDistance();
        
        try 
        {
            UpdateUI();
        }
        catch (MissingReferenceException e)
        {
            Debug.LogError($"UI update failed: {e.Message}");
        }
    }

    private void InitializeUI()
    {
        if (StepsText != null) StepsText.text = "0";
        if (DistanceText != null) DistanceText.text = "0m";
    }

    private void UpdateUI()
    {
        if (StepsText != null) 
            StepsText.text = stepCount.ToString("N0");
        
        if (DistanceText != null)
            DistanceText.text = $"{distanceWalked:N1}m";
    }

    private void DetectSteps()
    {
        acceleration = Input.acceleration;
        float delta = (acceleration - prevAcceleration).magnitude;
        
        if (delta > config.threshold)
        {
            stepCount++;
            Debug.Log($"Step detected! Count: {stepCount}");
        }
        prevAcceleration = acceleration;
    }

    private void CalculateDistance()
    {
        distanceWalked = stepCount * config.stepLength;
    }

    public void ResetStepData()
    {
        stepCount = 0;
        distanceWalked = 0f;
        UpdateUI(); // Force immediate UI update
        Debug.Log("Step counter reset");
    }

    // Rest of the methods remain the same
    public void CalibrateStepLength(float newStepLength)
    {
        if (newStepLength > 0)
        {
            config.stepLength = newStepLength;
            Debug.Log($"Step length calibrated to: {config.stepLength}m");
        }
        else
        {
            Debug.LogWarning("Invalid step length");
        }
    }

    public void LoadStepData(int loadedStepCount)
    {
        stepCount = loadedStepCount;
        CalculateDistance();
        UpdateUI(); // Refresh UI when loading data
    }

    public float GetDistanceWalked() => distanceWalked;
    public int GetStepCount() => stepCount;
}