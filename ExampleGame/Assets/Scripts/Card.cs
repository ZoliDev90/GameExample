using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    private bool isAnimating = false;
    private bool isFlipped = false;
    public int id;


    public bool IsFlipped()
    {
        return isFlipped;
    }


    public void Flip()
    {
        if (!isAnimating)
        {
            isFlipped = !isFlipped;
            float angle = isFlipped ? 180f : 0f;
            StartCoroutine(FlipAnimation(angle));
        }

    }

    private IEnumerator FlipAnimation(float endAngle)
    {
        float duration = 0.5f;
        float time = 0;
        Vector3 startRotation = transform.eulerAngles;
        Vector3 endRotation = new Vector3(startRotation.x, startRotation.y, endAngle);

        while (time < duration)
        {
            transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = endRotation;
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}