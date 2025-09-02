using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class EmotionAudioFileProcessor : MonoBehaviour
{
    [Header("Components")]
    public ExpressionController expressionController;
    public EmotionAnimationMapper emotionMapper;
    public AudioSource audioSource;
    
    [Header("File Settings")]
    public string folderName = "'/Users/siddharthlala/Desktop/Unity/New Unity Project 4/Assets/StreamingAssets/LiveInput'";
    public float checkInterval = 1f;
    public bool deleteProcessedFiles = false;
    
    [Header("Debug")]
    public bool debugMode = true;
    
    private string folderPath;
    private HashSet<string> processedFiles = new HashSet<string>();
    private bool isProcessing = false;
    
    void Start()
    {
        Debug.Log("Starting EmotionAudioFileProcessor");
        
        // Set up folder path
        folderPath = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        Debug.Log("Monitoring folder: " + folderPath);
        
        // Start monitoring
        StartCoroutine(MonitorFolder());
    }
    
    IEnumerator MonitorFolder()
    {
        WaitForSeconds wait = new WaitForSeconds(checkInterval);
        
        while (true)
        {
            // Only process if not already processing
            if (!isProcessing)
            {
                CheckForNewFiles();
            }
            
            yield return wait;
        }
    }
    
    void CheckForNewFiles()
    {
        if (!Directory.Exists(folderPath))
        {
            if (debugMode) Debug.Log("Directory doesn't exist: " + folderPath);
            return;
        }
        
        try
        {
            // Get all text files
            string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");
            
            if (txtFiles.Length > 0 && debugMode)
            {
                Debug.Log("Found " + txtFiles.Length + " text files");
            }
            
            // Process each file
            foreach (string txtFile in txtFiles)
            {
                string baseName = Path.GetFileNameWithoutExtension(txtFile);
                
                // Skip already processed files
                if (processedFiles.Contains(baseName))
                {
                    continue;
                }
                
                // Check if corresponding MP3 exists
                string audioFile = Path.Combine(folderPath, baseName + ".mp3");
                if (!File.Exists(audioFile))
                {
                    continue;
                }
                
                if (debugMode) Debug.Log("Found new file pair: " + baseName);
                
                // Mark as processed
                processedFiles.Add(baseName);
                isProcessing = true;
                
                // Process the files
                StartCoroutine(ProcessFiles(txtFile, audioFile));
                
                // Only process one pair at a time
                break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error checking files: " + e.Message);
        }
    }
    
    IEnumerator ProcessFiles(string txtFile, string audioFile)
    {
        // Read the emotion from text file
        string emotion = "talking";
        try
        {
            emotion = File.ReadAllText(txtFile).Trim().ToLower();
            Debug.Log("Read emotion: " + emotion);
        }
        catch
        {
            Debug.LogError("Error reading text file");
        }
        
        // Apply emotion
        if (expressionController != null)
        {
            expressionController.SetExpression(emotion);
            Debug.Log("Set expression: " + emotion);
        }
        
        // Trigger animation
        if (emotionMapper != null)
        {
            emotionMapper.TriggerEmotion(emotion);
            Debug.Log("Triggered animation: " + emotion);
        }
        
        // Play the audio
        Debug.Log("Loading audio: " + audioFile);
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFile, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                
                if (clip != null)
                {
                    // Stop any current audio
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                    
                    // Make sure volume is up
                    if (audioSource.volume < 0.1f)
                    {
                        audioSource.volume = 1.0f;
                    }
                    
                    // Play the clip
                    audioSource.clip = clip;
                    audioSource.Play();
                    
                    Debug.Log("Playing audio, length: " + clip.length);
                    
                    // Wait for audio to finish
                    yield return new WaitForSeconds(clip.length + 0.5f);
                }
                else
                {
                    Debug.LogError("Failed to load audio clip");
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                Debug.LogError("Audio load error: " + www.error);
                yield return new WaitForSeconds(1f);
            }
        }
        
        // Delete files if needed
        if (deleteProcessedFiles)
        {
            try
            {
                File.Delete(txtFile);
                File.Delete(audioFile);
                Debug.Log("Deleted processed files");
            }
            catch
            {
                Debug.LogWarning("Failed to delete files");
            }
        }
        
        // Done processing
        isProcessing = false;
    }
}