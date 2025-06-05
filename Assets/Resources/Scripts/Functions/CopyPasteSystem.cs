using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CopyPasteSystem : MonoBehaviour
{
    [SerializeField] public TMP_Text textToCopy;
    [SerializeField] public Image copyImage;

    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.green; // Feedback color
    [SerializeField] private float flashDuration = 0.5f;    // Feedback duration

    private Color originalColor;    // Stores initial color
    private Coroutine flashRoutine; // Reference to active coroutine

    void Start()
    {
        // Store initial color on start
        if (copyImage != null)
        {
            originalColor = copyImage.color;
        }
    }

    public void CopyToClipboard()
    {
        // Existing copy functionality
        TextEditor textEditor = new TextEditor();
        textEditor.text = textToCopy.text;
        textEditor.SelectAll();
        textEditor.Copy();

        // Trigger visual feedback
        if (copyImage != null)
        {
            // Stop any active flash before starting new one
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashImage());
        }
    }

    private IEnumerator FlashImage()
    {
        // Apply flash color
        copyImage.color = flashColor;
        
        // Wait for specified duration
        yield return new WaitForSeconds(flashDuration);
        
        // Revert to original color
        copyImage.color = originalColor;
        
        // Clear coroutine reference
        flashRoutine = null;
    }

    public void PasteFromClipboard()
    {
        // Existing paste functionality remains unchanged
        TextEditor textEditor = new TextEditor();
        textEditor.Paste();
        textToCopy.text = textEditor.text;
    }
}