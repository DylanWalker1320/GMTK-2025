
using UnityEngine;
using UnityEngine.InputSystem;

public class Reticle : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    [SerializeField] private float reticleDistance = 1.56f;
    [SerializeField] private bool gamepadIsAiming;
    public Vector3 aimDirection {get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        aimDirection = Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        gamepadIsAiming = false;

        // Gamepad Aiming

        if (Gamepad.current != null && PlayerMovement._playerInput.currentControlScheme == "Controller")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Vector2 stickInput = Gamepad.current.rightStick.ReadValue();
            if (stickInput.magnitude > 0.2f)
            {
                aimDirection = new Vector3(stickInput.x, stickInput.y, 0f).normalized;
                gamepadIsAiming = true;
            }
        }


        // Mouse Aiming
        else if (!gamepadIsAiming && Mouse.current != null && PlayerMovement._playerInput.currentControlScheme == "Keyboard")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = transform.parent.position.z;
            Vector3 aim = mousePos - transform.parent.position; // rotation

            if(aim.magnitude > 0.01f)
            {
                aimDirection = aim.normalized;
                // transform.position = transform.parent.position + direction;
            }
        }

        // reticle positioning
        transform.position = transform.parent.position + aimDirection * reticleDistance;

        // reticle rotation

        float rotZ = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg; // gives angle in radians using aim
        transform.rotation = Quaternion.Euler(0, 0, rotZ);   
    }
}
