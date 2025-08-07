using UnityEngine;

public class EyeGaze : MonoBehaviour
{
    public GameObject cameraObject;
    public Vector3 collision;
    public GameObject lastHit;
    private void Start()
    {
    }
    private void Update()
    {
        var ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance: 200))
        {
            lastHit = hit.transform.gameObject;
            collision = hit.point;
        }
    }

}
