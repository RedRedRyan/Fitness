using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class DailyStepData
{
    public string date;
    public int steps;
    public float distance;
}

public class StepDataLogger : MonoBehaviour
{
    private string savePath;
    private List<DailyStepData> stepHistory = new List<DailyStepData>();
    
    private DateTime lastCheckTime;
    private const string saveFileName = "StepHistory.json";
    private const string lastRecordedDateKey = "LastRecordedDate"; // Matching StepDataHandler
    private const string dailyStepsKey = "DailySteps"; // Matching StepDataHandler

    [Header("Settings")]
    public bool enablePlayerPrefsFallback = true; // Works with existing StepDataHandler
    public int maxHistoryDays = 30; // Auto-purge old entries

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        LoadStepHistory();
        lastCheckTime = DateTime.Now;
        CheckForNewDay(); // Integrated from StepDataHandler
    }

    private void Update()
    {
        // Midnight check - now handles both JSON and PlayerPrefs
        if (DateTime.Now.Date > lastCheckTime.Date)
        {
            OnNewDay();
            lastCheckTime = DateTime.Now;
        }
    }

    private void OnNewDay()
    {
        SaveDailyData();
        StepCounter.Instance.ResetStepData();
        
        if (enablePlayerPrefsFallback)
        {
            PlayerPrefs.SetString(lastRecordedDateKey, DateTime.Now.ToString("yyyyMMdd"));
            PlayerPrefs.SetInt(dailyStepsKey, 0);
        }
    }

    public void SaveDailyData()
    {
        DailyStepData todayData = new DailyStepData
        {
            date = DateTime.Now.ToString("yyyy-MM-dd"),
            steps = StepCounter.Instance.GetStepCount(),
            distance = StepCounter.Instance.GetDistanceWalked()
        };

        // Update or add today's entry
        int existingIndex = stepHistory.FindIndex(d => d.date == todayData.date);
        if (existingIndex >= 0)
        {
            stepHistory[existingIndex] = todayData;
        }
        else
        {
            stepHistory.Add(todayData);
            PurgeOldEntries();
        }

        // Dual saving (JSON + PlayerPrefs)
        string jsonData = JsonUtility.ToJson(new StepHistoryWrapper { data = stepHistory }, true);
        File.WriteAllText(savePath, jsonData);
        
        if (enablePlayerPrefsFallback)
        {
            PlayerPrefs.SetInt(dailyStepsKey, todayData.steps);
        }

        Debug.Log($"Saved {todayData.steps} steps to {savePath}");
    }

    private void CheckForNewDay()
    {
        string currentDate = DateTime.Now.ToString("yyyyMMdd");
        string lastDate = enablePlayerPrefsFallback ? 
            PlayerPrefs.GetString(lastRecordedDateKey, currentDate) : 
            stepHistory.Count > 0 ? 
                DateTime.ParseExact(stepHistory[^1].date, "yyyy-MM-dd", null).ToString("yyyyMMdd") : 
                currentDate;

        if (currentDate != lastDate)
        {
            OnNewDay();
        }
        else if (enablePlayerPrefsFallback)
        {
            LoadDailySteps();
        }
    }

    private void LoadDailySteps()
    {
        int savedSteps = PlayerPrefs.GetInt(dailyStepsKey, 0);
        StepCounter.Instance.LoadStepData(savedSteps);
    }

    private void PurgeOldEntries()
    {
        if (stepHistory.Count > maxHistoryDays)
        {
            stepHistory.RemoveRange(0, stepHistory.Count - maxHistoryDays);
            Debug.Log($"Purged old entries, keeping last {maxHistoryDays} days");
        }
    }

    private void LoadStepHistory()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string jsonData = File.ReadAllText(savePath);
                StepHistoryWrapper wrapper = JsonUtility.FromJson<StepHistoryWrapper>(jsonData);
                stepHistory = wrapper.data ?? new List<DailyStepData>();
                Debug.Log($"Loaded {stepHistory.Count} historical records");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed loading step history: {e.Message}");
                stepHistory = new List<DailyStepData>();
            }
        }
    }

    [System.Serializable]
    private class StepHistoryWrapper
    {
        public List<DailyStepData> data;
    }

    private void OnApplicationQuit() => SaveDailyData();
    private void OnApplicationPause(bool pauseStatus) { if (pauseStatus) SaveDailyData(); }

    // New methods for data access
    public List<DailyStepData> GetStepHistory() => new List<DailyStepData>(stepHistory);
    public int GetTodaySteps() => stepHistory.Find(d => d.date == DateTime.Now.ToString("yyyy-MM-dd"))?.steps ?? 0;
}