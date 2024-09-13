using System.Collections;
using UnityEngine;


public class rainLight : MonoBehaviour
{
    private float duration = 2f;  // Duraci�n del cambio de color e intensidad
    public Light dlight;
    public ParticleSystem particle;
    public ParticleSystem particle2;
    public ParticleSystem particle3;

    private Color originalColor;
    private float originalIntensity;

    private void Start()
    {
        if (dlight != null)
        {
            originalColor = dlight.color; // Guarda el color original de la luz
            originalIntensity = dlight.intensity; // Guarda la intensidad original
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ChangeLightGradually(Color.gray, 0.1f));  // Cambiar a gris e intensidad baja
            particle.Play();
            particle2.Stop();
            particle3.Stop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ChangeLightGradually(originalColor, originalIntensity));  // Restaurar color original e intensidad
            particle.Stop();
            particle2.Play();
            particle3.Play();
        }
    }

    private IEnumerator ChangeLightGradually(Color targetColor, float targetIntensity)
    {
        float elapsedTime = 0f;
        Color startColor = dlight.color;
        float startIntensity = dlight.intensity;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Interpola el color e intensidad
            dlight.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            dlight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);

            yield return null;  // Espera hasta el siguiente frame
        }

        // Aseg�rate de que el color y la intensidad se establezcan al valor final
        dlight.color = targetColor;
        dlight.intensity = targetIntensity;

    }

}







