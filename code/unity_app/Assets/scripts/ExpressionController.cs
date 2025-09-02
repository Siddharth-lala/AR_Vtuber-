using UnityEngine;
using System.Collections.Generic;

public class ExpressionController : MonoBehaviour
{
    public SkinnedMeshRenderer faceMesh;
    public Animator animator;


    // Map emotion keywords to blendshape names
    private Dictionary<string, string> expressionMap = new Dictionary<string, string>() {
        { "happy", "smile" },
        { "angry", "angry_face" },
        { "sad", "sad" },
        { "surprised", "Surprised" },
        { "neutral", "Default" }
    };

    private Dictionary<string, int> blendShapeIndices = new Dictionary<string, int>();
    private string currentExpression = "";

    void Start()
    {
        // Cache blendshape indices for quick access
        foreach (var pair in expressionMap)
        {
            int index = faceMesh.sharedMesh.GetBlendShapeIndex(pair.Value);
            if (index >= 0)
                blendShapeIndices[pair.Key] = index;
        }
    }

public void SetExpression(string expression)
{
    currentExpression = expression;

    // Reset all blendshapes first
    foreach (var pair in blendShapeIndices)
        faceMesh.SetBlendShapeWeight(pair.Value, 0f);

    // Apply the selected facial expression
    if (blendShapeIndices.ContainsKey(expression))
        faceMesh.SetBlendShapeWeight(blendShapeIndices[expression], 100f);

    // Trigger full-body animation if it exists
    if (animator != null)
    {
        // Reset other triggers first (optional, if you want exclusive transitions)
        animator.ResetTrigger("Sad");
        animator.ResetTrigger("Dance");
        animator.ResetTrigger("Wave");

        // Trigger the animation based on the expression keyword
        if (expression == "sad") animator.SetTrigger("Sad");
        if (expression == "dance") animator.SetTrigger("Dance");
        if (expression == "wave") animator.SetTrigger("Wave");
    }
}

        public string CurrentExpression => currentExpression;

}
