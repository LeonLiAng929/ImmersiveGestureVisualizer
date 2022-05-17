using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class PreviewSwitcher : MonoBehaviour
{
    [HideInInspector]
    public DatasetPreview[] previews;
    [HideInInspector]
    public static int index = 0;
    protected XRSimpleInteractable xRSimpleInteractable;
    public bool previous = false;
    public bool next = false;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        if(previous)
            xRSimpleInteractable.selectExited.AddListener(PreviousPreview);
        else if(next)
            xRSimpleInteractable.selectExited.AddListener(NextPreview);
        previews = transform.parent.Find("Referents").GetComponentsInChildren<DatasetPreview>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextPreview(SelectExitEventArgs args)
    {
        previews[index].gameObject.SetActive(false);
        if (index == previews.Length - 1)
        {
            index = 0;
        }
        else
        {
            index += 1;
        }
        previews[index].gameObject.SetActive(true);
    }

    public void PreviousPreview(SelectExitEventArgs args)
    {
        previews[index].gameObject.SetActive(false);
        if (index == 0)
        {
            index = previews.Length - 1;
        }
        else
        {
            index -= 1;
        }
        previews[index].gameObject.SetActive(true);
    }
}
