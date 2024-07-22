using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField]
    private int degreesPerSecond = 20;

    [SerializeField]
    private float amplitude = 0.5f;

    [SerializeField]
    private float frequency = 1f;
    private Vector3 posOffset;
    private Vector3 tempPos;

    void Start()
    {
        posOffset = transform.position;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, degreesPerSecond, 0) * Time.deltaTime);
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }
}
