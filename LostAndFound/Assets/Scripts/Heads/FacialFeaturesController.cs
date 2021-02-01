using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacialFeaturesController : MonoBehaviour
{
    private Material material;

    public float minBlinkDelay = 0.1f;
    public float maxBlinkDelay = 1f;
    public float blinkTime = 0.1f;

    public float talkDelay = 0.25f;
    public bool talking = false;
    private bool mouthOpen = false;
    private Coroutine talkCoroutine;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        StartCoroutine(Blink());
        //StartTalking();
    }

    private IEnumerator Blink()
    {
        float delayTime = Random.Range(minBlinkDelay, maxBlinkDelay);
        yield return new WaitForSeconds(delayTime);
        material.SetInt("_closeEyes", 1);
        StartCoroutine(Unblink());
    }

    private IEnumerator Unblink()
    {
        yield return new WaitForSeconds(blinkTime);
        material.SetInt("_closeEyes", 0);
        StartCoroutine(Blink());
    }

    public void StartTalking()
    {
        talkCoroutine = StartCoroutine(Talk());
    }

    public void StopTalking()
    {
        StopCoroutine(talkCoroutine);
        material.SetInt("_openMouth", 0);
    }

    private IEnumerator Talk()
    {
        yield return new WaitForSeconds(talkDelay);

        if(mouthOpen)
        {
            material.SetInt("_openMouth", 0);
        }
        else
        {
            material.SetInt("_openMouth", 1);
        }
        mouthOpen = !mouthOpen;
        Debug.Log(material.GetInt("_openMouth"));

        StartTalking();
    }
}
