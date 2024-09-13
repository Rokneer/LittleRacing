using TMPro;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public static CheckPoint Instance { get; private set; }

    public Transform[] checkpoints;
    private int currentCheckpoint = 0;
    private int lapsCompleted = 0;
    public TextMeshProUGUI LapText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateLapUI();
    }

    public void PlayerHitCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == currentCheckpoint)
        {
            currentCheckpoint++;
            checkpoints[checkpointIndex].gameObject.SetActive(false);

            if (currentCheckpoint >= checkpoints.Length)
            {
                Debug.Log("Vuelta completada!");
                currentCheckpoint = 0;
                lapsCompleted++;
                ResetCheckPoint();
                UpdateLapUI();
            }
        }
        else
        {
            Debug.Log("Pasaste por un checkpoint incorrecto.");
        }
    }

    private void UpdateLapUI()
    {
        LapText.text = $"{lapsCompleted}/3";
    }

    private void ResetCheckPoint()
    {
        foreach (Transform checkpoint in checkpoints)
        {
            checkpoint.gameObject.SetActive(true);
        }
    }
}
