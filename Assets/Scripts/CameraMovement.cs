using UnityEngine;

/// <summary>
///     This class holds all "onClick" functions for the in-game buttons.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Quaternion _originalCamRotation;

    void Awake()
    {
        _originalCamRotation = Camera.main.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
         // Rotate view right/left
        var lookX = Input.GetAxis("Mouse X") * Time.deltaTime * 40;
        transform.Rotate(0, lookX, 0);
        
        // Rotate view up/down
        var lookY = Input.GetAxis("Mouse Y") * Time.deltaTime * 40;
        var newRotation = Camera.main.transform.localRotation * Quaternion.AngleAxis(-lookY, Vector3.right);
        if (Mathf.Abs(Quaternion.Angle(_originalCamRotation, newRotation)) < 10.0f)
        {
            Camera.main.transform.Rotate(-lookY, 0, 0);
        }


    }
}
