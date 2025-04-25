using UnityEngine;

public class HurricaneWindParticle : MonoBehaviour
{
    [HideInInspector] public float rotateSpeed;
    private Transform parent;

    private void Start()
    {
        parent = transform.parent;

        transform.RotateAround(parent.position, new Vector3(0, 0, 1), Random.Range(0, 360)); // Random start angle
    }

    private void Update()
    {
        transform.RotateAround(parent.position, new Vector3(0, 0, 1), rotateSpeed * Time.deltaTime);
    }
}
