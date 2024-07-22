using System.Collections;
using UnityEngine;

public class NitroPill : MonoBehaviour
{
    [SerializeField]
    private GameObject nitroObj;

    [SerializeField]
    private float nitroAcceleration = 40;

    [SerializeField]
    private float nitroDuration = 6;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CarController car = other.gameObject.GetComponentInParent<CarController>();

            StartCoroutine(ActivateNitro(car, car.acceleration));

            nitroObj.SetActive(false);
        }
    }

    private IEnumerator ActivateNitro(CarController car, float baseAcceleration)
    {
        Debug.Log("Nitro activated!");
        car.acceleration = nitroAcceleration;
        yield return new WaitForSeconds(nitroDuration);
        car.acceleration = baseAcceleration;
        Debug.Log("Nitro deactivated!");

        gameObject.SetActive(false);
    }
}
