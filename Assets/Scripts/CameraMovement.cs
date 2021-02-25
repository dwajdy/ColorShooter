using UnityEngine;

/// <summary>
///     This class holds camera movement for the "FPS camera effect".
/// </summary>
public class CameraMovement : MonoBehaviour
{
    // ##############
    // ## Privates ##
    // ##############
    private Quaternion _originalCamRotation;

    // ###############
    // ## Constants ##
    // ###############
    private const float MOVEMENT_MULTIPLIER = 40.0f;

    // ###############
    // ## Methods   ##
    // ###############

    // for our project, we don't care about current camera rotation, we start from Quaternion.identity (no rotation state).
    void Awake()
    {
        _originalCamRotation = Quaternion.identity;
    }

    /// Will move the camera based on mouse movement.
    void Update()
    {
         // Rotate view right/left
        var lookX = Input.GetAxis("Mouse X") * Time.deltaTime * MOVEMENT_MULTIPLIER;
        transform.Rotate(0, lookX, 0);
        
        // Rotate view up/down
        var lookY = Input.GetAxis("Mouse Y") * Time.deltaTime * MOVEMENT_MULTIPLIER;
        var newRotation = Camera.main.transform.localRotation * Quaternion.AngleAxis(-lookY, Vector3.right);

        //constraint movement to prevent excessive rotations.
        if (Mathf.Abs(Quaternion.Angle(_originalCamRotation, newRotation)) < 10.0f) 
        {
            Camera.main.transform.Rotate(-lookY, 0, 0);
        }

    }
}
