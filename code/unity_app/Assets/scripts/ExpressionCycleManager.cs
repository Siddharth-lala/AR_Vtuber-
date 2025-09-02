using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionCycleManager : MonoBehaviour
{
    public ExpressionController expressionController;
    public float intervalSeconds = 3f;

    // Simulating external JSON input as a list of expressions
    private List<string> mockInputs = new List<string>
    {
        "happy",
        "angry",
        "sad",
        "surprised",
        "neutral"
    };

    void Start()
    {
        StartCoroutine(CycleExpressions());
    }

    IEnumerator CycleExpressions()
    {
        while (true)
        {
            foreach (string emotion in mockInputs)
            {
                Debug.Log($"Setting expression: {emotion}");
                expressionController.SetExpression(emotion);
                yield return new WaitForSeconds(intervalSeconds);
            }
        }
    }
}
