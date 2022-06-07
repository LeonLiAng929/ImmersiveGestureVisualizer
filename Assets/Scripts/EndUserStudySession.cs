using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndUserStudySession : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    public float duration = 3f;
    public Color fadeColor;
    public GameObject textMesh;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = textMesh.GetComponent<Renderer>();
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(EndSession);
        Color c = rend.material.color;
        c.a = 0;
        rend.material.color = c;
    }

    private void EndSession(SelectExitEventArgs args)
    {
        ShowPrompt();
        UserStudy.instance.EndSession();
    }

    public void ShowPrompt()
    {
        //textMesh.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
        FadeOut();
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
        while (timer <= duration)
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
