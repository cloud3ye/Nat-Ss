using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera camera1;
    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 zoomAmount;
    public Vector3 newZoom;
    public Vector3 dragStartPos;
    public Vector3 dragCurrentPos;
    public Vector3 rotateStartPos;
    public Vector3 rotateCurrentPos;

    // Start is called before the first frame update
    void Start()
    {
        var transposer = camera1.GetCinemachineComponent<CinemachineTransposer>();
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = new Vector3 (0,20,-20);
        
        transposer.m_FollowOffset = newZoom;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleMouseInput();
    }
    void HandleMouseInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
        if(Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if(plane.Raycast(ray,out entry))
            {
                dragStartPos = ray.GetPoint(entry);
            }
        }
        if(Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if(plane.Raycast(ray,out entry))
            {
                dragCurrentPos = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPos - dragCurrentPos;
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            rotateStartPos = Input.mousePosition;
        }
        if(Input.GetMouseButton(1))
        {
            rotateCurrentPos = Input.mousePosition;
            Vector3 difference = rotateStartPos - rotateCurrentPos;
            rotateStartPos = rotateCurrentPos;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x /5f));
        }
    }
    void HandleMovementInput()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (Vector3.forward * movementSpeed);
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (Vector3.back * movementSpeed);
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (Vector3.left * movementSpeed);
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (Vector3.right * movementSpeed);
        }
        if(Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if(Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        if(Input.GetKey(KeyCode.R))
        {
            newZoom -= zoomAmount;
        }
        if(Input.GetKey(KeyCode.F))
        {
            newZoom += zoomAmount;
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation,newRotation,Time.deltaTime * movementTime);
        var transposer = camera1.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, newZoom, Time.deltaTime * movementTime);
    }
}
