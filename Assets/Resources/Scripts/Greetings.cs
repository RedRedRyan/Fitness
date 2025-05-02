using UnityEngine;
using TMPro;
using System;

public class Greetings : MonoBehaviour
{
    public TMP_Text greetingText; // Assign your TMP text element in Inspector

    void Start()
    {
        UpdateGreeting();
    }

    void Update()
    {
        // Update every minute (optional optimization)
        if (DateTime.Now.Second == 0)
        {
            UpdateGreeting();
        }
    }

    void UpdateGreeting()
    {
        int currentHour = DateTime.Now.Hour;
        
        if (currentHour >= 21 || currentHour < 5) // 9PM to 4:59AM
        {
            greetingText.text = "Night Time";
        }
        else if (currentHour >= 5 && currentHour < 12) // 5AM to 11:59AM
        {
            greetingText.text = "Good Morning";
        }
        else if (currentHour >= 12 && currentHour < 21) // 12PM to 8:59PM
        {
            greetingText.text = "Good Evening";
        }
    }
}