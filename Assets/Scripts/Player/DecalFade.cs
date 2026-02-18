using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFade : MonoBehaviour
{
    private float lifetime = 5;
    float fadeDuration = 1.5f;

    DecalProjector projector;
    private Material decalMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        projector = GetComponent<DecalProjector>();
        decalMaterial = new Material(projector.material);
        projector.material = decalMaterial;
    }

    private void OnEnable()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float elapsed = 0;
        float initialFactor = projector.fadeFactor;
        float FadeDuration = 2;

        yield return new WaitForSeconds(lifetime);

        while (elapsed < 1)
        {
            projector.fadeFactor = Mathf.Lerp(initialFactor, 0, elapsed);
            elapsed += Time.deltaTime / FadeDuration;
            yield return null;
        }

        Destroy(gameObject);
    }
}