using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public float duration = 1f;
    public Color fadeColor;
    private Renderer rend;
    #region Singleton
    public static FadeScreen instance;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        instance = this;
        FadeIn();
    }

    public void FadeIn() { Fade(1, 0); }
    public void FadeOut() { Fade(0, 1); }
    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while(timer <= duration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / duration);
            rend.material.SetColor("_Color", newColor);
            timer += Time.deltaTime;
            yield return null;
        }
        Color newColor1 = fadeColor;
        newColor1.a = alphaOut;
        rend.material.SetColor("_Color", newColor1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
