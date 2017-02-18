using UnityEngine;

public class AimManager : MonoBehaviour
{
    public Vector3 _mouseCoords;
    public float MouseSensitivity;
    public RectTransform CrosshairTransform;
    public Vector3 MouseToCrosshairOffset;
    public Plane Plane2D;

    public Vector3 AimAtPosition;

    // Use this for initialization
    private void Start()
    {
        CrosshairTransform = GameObject.Find("Crosshair").GetComponent<RectTransform>();
        Plane2D = new Plane(Vector3.zero, Vector3.right, Vector3.up);
        MouseSensitivity = 1;
        MouseToCrosshairOffset = new Vector3(CrosshairTransform.sizeDelta.x/2, CrosshairTransform.sizeDelta.y/2, 0);
    }

    private float _rayDistance;
    // Update is called once per frame
    private void Update()
    {
        _mouseCoords = Input.mousePosition;

        CrosshairTransform.anchoredPosition = _mouseCoords - MouseToCrosshairOffset;

        Ray ray = Camera.main.ScreenPointToRay(_mouseCoords);
        if (Plane2D.Raycast(ray, out _rayDistance)) AimAtPosition = ray.GetPoint(_rayDistance);
    }
}