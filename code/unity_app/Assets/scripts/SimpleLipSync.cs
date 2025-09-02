using UnityEngine;

public class SimpleLipSync : MonoBehaviour
{
    public SkinnedMeshRenderer faceMesh;
    public AudioSource audioSource;
    private int mouth1Index = -1;
    private int mouth2Index = -1;
    public float sensitivity = 100f;
    private float[] samples = new float[256];
    
    [Header("Debug")]
    public bool debugMode = false;
    
    void Start()
    {
        if (faceMesh == null)
        {
            Debug.LogError("[SimpleLipSync] No SkinnedMeshRenderer assigned!");
            return;
        }
        
        if (audioSource == null)
        {
            Debug.LogError("[SimpleLipSync] No AudioSource assigned!");
            return;
        }
        
        mouth1Index = faceMesh.sharedMesh.GetBlendShapeIndex("open_mouth2.001");
        mouth2Index = faceMesh.sharedMesh.GetBlendShapeIndex("open_mouth.002");
        
        if (mouth1Index == -1 || mouth2Index == -1)
        {
            Debug.LogError("[SimpleLipSync] Could not find blend shapes: open_mouth2.001 or open_mouth.002");
        }
        else
        {
            Debug.Log($"[SimpleLipSync] Initialized with blend shapes: mouth1Index={mouth1Index}, mouth2Index={mouth2Index}");
        }
    }
    
    void Update()
    {
        if (faceMesh == null || audioSource == null || mouth1Index == -1 || mouth2Index == -1)
            return;
            
        if (audioSource.isPlaying)
        {
            audioSource.GetOutputData(samples, 0);
            float sum = 0f;
            foreach (var s in samples)
                sum += s * s;
            
            float rms = Mathf.Sqrt(sum / samples.Length);
            float volume = Mathf.Clamp01(rms * sensitivity);
            
            // Apply volume to mouth blendshapes
            float weight1 = volume * 50f;
            float weight2 = Mathf.Clamp(volume * 100f, 0f, 50f);
            
            faceMesh.SetBlendShapeWeight(mouth1Index, weight1);
            faceMesh.SetBlendShapeWeight(mouth2Index, weight2);
            
            if (debugMode && Time.frameCount % 30 == 0)
            {
                Debug.Log($"[SimpleLipSync] Audio playing: rms={rms:F3}, volume={volume:F3}, weights={weight1:F1}/{weight2:F1}");
            }
        }
        else
        {
            // Reset mouth if not talking
            faceMesh.SetBlendShapeWeight(mouth1Index, 0f);
            faceMesh.SetBlendShapeWeight(mouth2Index, 0f);
        }
    }
}