using UnityEngine;
using System.Collections.Generic;

public class EmotionAnimationMapper : MonoBehaviour
{
    [System.Serializable]
    public class EmotionMapping
    {
        public string emotionName;
        public string triggerName;
    }
    
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private List<EmotionMapping> emotionMappings = new List<EmotionMapping>();
    
    private Dictionary<string, string> mappingDictionary = new Dictionary<string, string>();
    
    void Start()
    {
        // Build the mapping dictionary
        foreach (var mapping in emotionMappings)
        {
            mappingDictionary[mapping.emotionName.ToLower()] = mapping.triggerName;
        }
    }
    
    public void TriggerEmotion(string emotion)
    {
        if (string.IsNullOrEmpty(emotion) || animator == null)
            return;
            
        // Convert to lowercase for case-insensitive matching
        emotion = emotion.ToLower().Trim();
        
        // Reset all animation triggers first (optional)
        ResetAllTriggers();
        
        // Check if we have a mapping for this emotion
        if (mappingDictionary.ContainsKey(emotion))
        {
            string triggerName = mappingDictionary[emotion];
            Debug.Log($"Triggering animation: {triggerName} for emotion: {emotion}");
            animator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning($"No animation mapping found for emotion: {emotion}");
        }
    }
    
    private void ResetAllTriggers()
    {
        // Reset all triggers that we use
        foreach (var mapping in emotionMappings)
        {
            animator.ResetTrigger(mapping.triggerName);
        }
    }
}