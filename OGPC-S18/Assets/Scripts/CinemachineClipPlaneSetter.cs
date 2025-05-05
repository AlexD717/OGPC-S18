using Unity.Cinemachine;
using UnityEngine;

public class CinemachineClipPlaneSetter : MonoBehaviour
{
    [SerializeField] private float nearClipPlane;
    private CinemachineCamera cinemachine;

    void Start()
    {
        cinemachine = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        cinemachine.Lens.NearClipPlane = nearClipPlane;
    }
}