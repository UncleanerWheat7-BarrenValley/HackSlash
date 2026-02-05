using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public GameObject cam1, cam2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam1.transform.parent = null;
        cam2.transform.parent = null;
    }
}
