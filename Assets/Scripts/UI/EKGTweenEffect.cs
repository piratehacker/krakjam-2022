using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKGTweenEffect : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private LineRenderer lineRenderer = null;
    [SerializeField]
    private int points;

    [SerializeField]
    private float xMultiply = 30;

    [SerializeField]
    private float yMultiply = 100;

    [SerializeField]
    private float delay;

    private bool dead = false;

    protected void Draw()
    {
        lineRenderer.positionCount = points;

        for (int i = 0; i < points; i++)
        {
            float progress = ((float)i / (points - 1));
            float y = curve.Evaluate(progress + Time.time / delay);
            if (dead)
            {
                y = Mathf.Lerp(y, 1, (((progress) + (Time.time * (900 / delay))) * 20) / 6500);
            }
            lineRenderer.SetPosition(i, new Vector3(progress * xMultiply, y * yMultiply, 0));
        }
    }

    public void OnDead()
    {
        dead = true;
    }

    protected void Update()
    {
        Draw();
    }
}
