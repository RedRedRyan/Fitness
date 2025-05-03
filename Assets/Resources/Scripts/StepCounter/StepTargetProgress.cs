using UnityEngine;
using UnityEngine.UI;

public class StepTargetProgress : MonoBehaviour
{
    [Header("Target Settings")]
    public int dailyStepTarget = 10000;
    
    [Header("UI References")]
    [SerializeField] private Image progressImage;
    [SerializeField] private Text progressPercent; // Renamed field

    private void Update()
    {
        if (StepCounter.Instance == null) return;

        int currentSteps = StepCounter.Instance.GetStepCount();
        float progress = Mathf.Clamp01((float)currentSteps / dailyStepTarget);
        
        progressImage.fillAmount = progress;
        
        if (progressPercent != null)
        {
            // Whole number percentage with no decimals
            int percentage = Mathf.RoundToInt(progress * 100f);
            progressPercent.text = $"{percentage}%";
            
            // Optional color transition
            progressPercent.color = Color.Lerp(Color.red, Color.green, progress);
        }
    }
}