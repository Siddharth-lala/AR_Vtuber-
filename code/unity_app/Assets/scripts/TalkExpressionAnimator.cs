using System.Collections;
using UnityEngine;

public class TalkExpressionAnimator : MonoBehaviour
{
    public SkinnedMeshRenderer faceMesh;
    public ExpressionController expressionController;
    public string triggerExpression = "talk";

    private int mouth1Index;
    private int mouth2Index;
    private int blinkIndex;
    private int eyebrowIndex;

    void Start()
    {
        mouth1Index = faceMesh.sharedMesh.GetBlendShapeIndex("open_mouth2.001");
        mouth2Index = faceMesh.sharedMesh.GetBlendShapeIndex("open_mouth.002");
        blinkIndex = faceMesh.sharedMesh.GetBlendShapeIndex("blink");
        eyebrowIndex = faceMesh.sharedMesh.GetBlendShapeIndex("raise_eyebrows");

        StartCoroutine(AnimateLoop());
    }

    IEnumerator AnimateLoop()
    {
        while (true)
        {
            if (expressionController != null && expressionController.CurrentExpression == triggerExpression)
            {
                StartCoroutine(AnimateMouth());
                if (Random.value > 0.7f) StartCoroutine(AnimateEyebrows());
                if (Random.value > 0.8f) StartCoroutine(BlinkEyes());
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator AnimateMouth()
    {
        for (int i = 0; i < 3; i++)
        {
            faceMesh.SetBlendShapeWeight(mouth1Index, 60f);
            faceMesh.SetBlendShapeWeight(mouth2Index, 40f);
            yield return new WaitForSeconds(0.1f);

            faceMesh.SetBlendShapeWeight(mouth1Index, 0f);
            faceMesh.SetBlendShapeWeight(mouth2Index, 0f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator BlinkEyes()
    {
        faceMesh.SetBlendShapeWeight(blinkIndex, 100f);
        yield return new WaitForSeconds(0.1f);
        faceMesh.SetBlendShapeWeight(blinkIndex, 0f);
    }

    IEnumerator AnimateEyebrows()
    {
        faceMesh.SetBlendShapeWeight(eyebrowIndex, 25f);
        yield return new WaitForSeconds(0.3f);
        faceMesh.SetBlendShapeWeight(eyebrowIndex, 0f);
    }
}
