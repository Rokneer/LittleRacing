using UnityEngine;

public class Finishing : MonoBehaviour
{
   [SerializeField] private GameObject meta;

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         meta.SetActive(true);
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         meta.SetActive(false);
      }
   }
}
