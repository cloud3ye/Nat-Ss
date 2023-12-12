using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{
    private Camera mainCamera;
    private CursorControlls controls;
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;
    public CinemachineVirtualCamera vcaml;
    public CinemachineVirtualCamera vcamr;
    private bool mcamera = true;
    private bool leftcam = false;
    private bool rightcam = false;
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.tag == "Creature")
                {
                    SetCanvas(hit);
                    SetCamera(hit);
                    SwitchPiority();
                }
            }
        }
        if(Input.GetKey(KeyCode.Escape))
        {
            if(vcam2 != null)
            {
                SwitchPiority();
            }
            
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            SwitchCamleft();
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            SwitchCamright();
        }
    }

    private void SetCanvas(RaycastHit hit)
    {
        if(canvas == null)
        {
            canvas = hit.collider.gameObject.GetComponentInChildren<Canvas>(includeInactive: true).gameObject;
        }
    }
    private void SetCamera(RaycastHit hit)
    {
        if(vcam2 == null)
        {
            vcam2 = hit.collider.gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        }
    }
     private void SwitchPiority()
    {
        if (mcamera)
        {
            vcam1.Priority = 9;
            vcam2.Priority = 10;
            canvas.SetActive(true);
        }
        else
        {
            vcam1.Priority = 10;
            vcam2.Priority = 9;
            canvas.SetActive(false);
            canvas = null;
            vcam2 = null;
        }
        mcamera = !mcamera;
    }
    private void SwitchCamleft()
    {
        if(leftcam)
        {
            vcam1.Priority = 10;
            vcaml.Priority = 9;
        }
        else
        {
            vcam1.Priority = 9;
            vcaml.Priority = 10;
        }
        leftcam = !leftcam;
        
    }
    private void SwitchCamright()
    {
        if(rightcam)
        {
            vcam1.Priority = 10;
            vcamr.Priority = 9;
        }
        else
        {
            vcam1.Priority = 9;
            vcamr.Priority = 10;
        }
        rightcam = !rightcam;
        
    }

}
