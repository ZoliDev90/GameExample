using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    public float riseDistance = 50f; // How high the text rises
    public float fadeOutTime = 3f; // Duration of the fade out animation

    private Text popupText;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        popupText = GetComponent<Text>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(Vector3 position, string text, Color textColor)
    {
        popupText.text = text;
        popupText.color = textColor;
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        Vector3 startPosition = rectTransform.anchoredPosition;
        Vector3 endPosition = startPosition + new Vector3(0, riseDistance, 0);

        float startTime = Time.time;
        while (Time.time - startTime < fadeOutTime)
        {
            float t = (Time.time - startTime) / fadeOutTime;
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, t);
            canvasGroup.alpha = 1 - (Time.time - startTime) / fadeOutTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}