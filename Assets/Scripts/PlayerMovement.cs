
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float mouseSensitivity = 50;
    public float speed = 5f;

    public Transform cameraTransform;
    public GameObject sphereIndicator; 
    private float xRotation;
    private Vector3 hitPos;
    private float deformationTime;
    private float radius;
    private Camera _camera;
    
    RaycastHit hit;


    void Start()
    {
        _camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        radius = sphereIndicator.transform.localScale.x;
    }

    void Update()
    {
        Move();
        RotateCamera();
        HandleScrollInput();

        hitRay();
        
    }

    void handleMouseClick()
    {
        if (Time.time > deformationTime)
        {

            if (Input.GetMouseButton(0))
            {
                deformationTime = Time.time + 0.1f;
                GameManager.Instance.placeTerrain(hitPos); 
            }
            else if (Input.GetMouseButton(1))
            {
                deformationTime = Time.time + 0.1f;
                GameManager.Instance.removeTerrain(hitPos);
            }
        }
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical"); 
        float moveY = Input.GetKey(KeyCode.Space) ?  1  : Input.GetKey(KeyCode.LeftShift) ? -1 : 0;
        Vector3 move = new Vector3(moveX, moveY, moveZ);
        transform.Translate(move * (speed * Time.deltaTime));
    }

    void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            radius += scroll * 10f;
            radius = Mathf.Clamp(radius, 1, 5);

            sphereIndicator.transform.localScale = Vector3.one * radius;
        }
    }

    void hitRay()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        

        if (Physics.Raycast(ray, out hit))
        {
            sphereIndicator.SetActive(true);
            hitPos = sphereIndicator.transform.position = hit.point;
            handleMouseClick();
        }
        else
        {
            sphereIndicator.SetActive(false);
        }
    }

}
