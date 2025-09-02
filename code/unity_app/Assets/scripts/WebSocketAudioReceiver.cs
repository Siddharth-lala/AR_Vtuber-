using UnityEngine;
using System;
using System.Collections;
using System.IO;
using NativeWebSocket;

public class WebSocketAudioReceiver : MonoBehaviour
{
    [Header("WebSocket Settings")]
    [SerializeField] private string serverUrl = "ws://localhost:8080";
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SimpleLipSync lipSync;

    private WebSocket webSocket;
    private bool isConnected = false;
    private byte[] audioData;

    private void Start()
    {
        ConnectToServer();
    }

    private async void ConnectToServer()
    {
        webSocket = new WebSocket(serverUrl);

        webSocket.OnOpen += () => {
            Debug.Log("Connection open!");
            isConnected = true;
        };

        webSocket.OnError += (e) => {
            Debug.LogError("Error! " + e);
        };

        webSocket.OnClose += (e) => {
            Debug.Log("Connection closed!");
            isConnected = false;
        };

        webSocket.OnMessage += (bytes) => {
            Debug.Log("Received audio data: " + bytes.Length + " bytes");
            ProcessAudioData(bytes);
        };

        // Keep trying to connect if it fails
        await webSocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (webSocket != null)
        {
            webSocket.DispatchMessageQueue();
        }
#endif
    }

    private async void OnApplicationQuit()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.Close();
        }
    }

    private void ProcessAudioData(byte[] data)
    {
        audioData = data;
        StartCoroutine(LoadAndPlayAudio());
    }

    private IEnumerator LoadAndPlayAudio()
    {
        // Create a temporary MP3 file from the received data
        string tempPath = Path.Combine(Application.temporaryCachePath, "receivedAudio.mp3");
        File.WriteAllBytes(tempPath, audioData);

        // Create a WWW request to load the audio clip
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip("file://" + tempPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
                
                // Stop any currently playing audio
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                
                // Assign and play the new clip
                audioSource.clip = clip;
                audioSource.Play();
                
                Debug.Log("Playing received audio clip");
                
                // The lip sync script will automatically detect that audio is playing
                // since it references the same AudioSource
            }
            else
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
        }

        // Clean up the temp file
        try
        {
            File.Delete(tempPath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not delete temp file: " + e.Message);
        }
    }

    // Manually request audio from the server
    public void RequestAudio()
    {
        if (isConnected)
        {
            webSocket.SendText("REQUEST_AUDIO");
            Debug.Log("Audio request sent to server");
        }
        else
        {
            Debug.LogWarning("Not connected to server. Cannot request audio.");
            // Try to reconnect
            ConnectToServer();
        }
    }
}