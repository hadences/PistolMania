using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1f;

    private float intensityMultiplier = 1.0f;
    private Vector3 initialPosition;

    private void Start() {
        initialPosition = transform.localPosition; 
    }

    private void Update() {
        if(start) {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    public void shakeCamera(float duration, float intensity) {
        start = true;
        this.duration = duration;
        intensityMultiplier = intensity;
    }

    IEnumerator Shaking() {
        float elapsedTime = 0.0f;

        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = initialPosition + Random.insideUnitSphere * strength * intensityMultiplier;
            yield return null;
        }

        transform.localPosition = initialPosition;
    }
}
