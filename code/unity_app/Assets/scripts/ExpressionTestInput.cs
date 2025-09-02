using UnityEngine;

public class ExpressionTestInput : MonoBehaviour
{
    public ExpressionController expressionController;
    public AudioSource audioSource;
    public WebSocketAudioReceiver webSocketReceiver;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            expressionController.SetExpression("happy");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            expressionController.SetExpression("angry");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            expressionController.SetExpression("sad");
        if (Input.GetKeyDown(KeyCode.Alpha4))
            expressionController.SetExpression("surprised");
        if (Input.GetKeyDown(KeyCode.Alpha5))
            expressionController.SetExpression("neutral");
        if (Input.GetKeyDown(KeyCode.T)) // T for Talk
            expressionController.SetExpression("talk");
        
        // Modified to request audio through WebSocket when L is pressed
        if (Input.GetKeyDown(KeyCode.L)) {
            expressionController.SetExpression("talk");
            
            // Instead of directly playing audio, request it from the server
            if (webSocketReceiver != null)
            {
                webSocketReceiver.RequestAudio();
            }
            else
            {
                Debug.LogError("WebSocketAudioReceiver not assigned!");
                
                // Fall back to local audio if WebSocket isn't set up
                if (audioSource != null)
                    audioSource.Play();
            }
        }
    }
}