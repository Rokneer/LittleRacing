using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    public int CheckPointIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPoint.Instance.PlayerHitCheckpoint(CheckPointIndex);
        }
    }
}
