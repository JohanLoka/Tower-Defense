using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //All variables stored here
    #region Variables
    [Header("Components")]
    public Camera cam;                  //Main camera of this Camera Object
    public GameObject rotor;            //Rotor of this Camera object
    public GameObject tilter;           //Tilter of this Camera object

    [Space(10)]

    public bool mobileMovement = true;  //If true, then mobile input is active
    public bool editorMovement = true;  //If true, then editor input is active

    [Space(10)]

    public float smoot = 0.04f;         //Lower number is smooter and slower movement

    [Space(20)]

    //----------------------------------------------------------

    [Header("Movement")]
    public float mobileMoveSpeed = 7f;  //Movement speed for android. I dont recommmend set very high number                   
    public float editorMoveSpeed = 7f;  //Movement speed for editor and windows player. I dont recommmend set very high number 

    [Space(10)]

    public LayerMask terrainLayer;      //Camera move by ray on collision with this layer 

    [Space(10)]
    
    [Header("Clamped Positions of Camera")]
    public float minXpos = 0;       //Minimal position of camera on X axes
    public float maxXpos = 0;       //Maximal position of camera on X axes
    public float minYpos = 0;       //Minimal position of camera on Y axes
    public float maxYpos = 0;       //Maximal position of camera on Y axes

    private bool scanOldRay = true;             //Used for getting delta position for android drag
    private bool editorScanOldRay = true;       //Used for getting delta position for windows drag

    private Vector3 oldRayPos = new Vector3(0, 0, 0);       //Used for getting delta position for android drag
    private Vector3 editorOldRayPos = new Vector3(0, 0, 0); //Used for getting delta position for windows drag
    [Space(20)]

    //----------------------------------------------------------

    [Header("Rotation")]
    public float mobileRotSpeed = 2.4f;     //Rotation speed for android
    public float editorRotSpeed = 4f;       //Rotation speed for windows

    private float mRotX = 0;                //Used for Calculating rotation of camera on X axes for windows
    private float mRotY = 0;                //Used for Calculating rotation of camera on Y axes for windows
    [Space(20)]

    //----------------------------------------------------------

    [Header("Zoom")]
    public float mobileZoomSpeed = 1.3f;    //Zoom speed for android
    public float editorZoomSpeed = 4f;      //Zoom speed for windows

    [Space(10)]

    public float minZoom = 30f;             //This is how close can be camera
    public float maxZoom = 100f;            //This is how far can be camera

    [Space(10)]

    public float minMovement = 0f;     //Minimal deltaMagnitudeDifference of finger to start zooming for android 
    public float maxMovementY = 1000;   //If you dont like zooming while rotating, then set lower number(somethink like 10), but after you will be able to zoom anly by moving on X axes. If your delta position of fingers on Y axes will be higher then this number, than user will not zoom
    [Space(20)]

    //----------------------------------------------------------

    [Header("Tilt")]
    public float mobileTiltSpeed = 4f;      //Tilt speed for android
    public float editorTiltSpeed = 1f;      //Tilt speed for windows

    [Space(10)]

    public float minRot = 30f;              //Minimal angel of tilt
    public float maxRot = 90f;              //Maximal angel of tilt

    private float rotX;                     //Camera tilt on start

    private Vector2 oldMidPoint = new Vector2(0, 0);        //Used for calculating deltaPosition of mid point between 2 fingers

    private bool scanOld = true;            //Used for calculating deltaPosition of mid point between 2 fingers

    #endregion

    //----------------------------------------------------------

    private void Start()
    {
        rotX = tilter.transform.rotation.x;     //Scan camera tilt on start
    }

    private void Update()
    {
        //If you dont need one of platforms, just remove them from update

        MobileCameraMovement();                 //Call all functions for android
        EditorCameraMovement();                 //Call all functions for windows  (IF RUNNING BOTH AT ONCE, THAN TOUCH INPUT GET LITTLE BIT CRAZY!!!)
    }

    //----------------------------------------------------------

    private void MobileCameraMovement()
    {
        if (mobileMovement)
        {
            MobileCameraDrag();                    //Call mobile drag function
            MobileCameraZoom();                    //Call mobile zoom function
            MobileCameraTilt();                    //Call mobile tilt function
            MobileCameraRotate();                  //Call mobile rotate function
        }
    }        //Call all functions for mobile

    private void MobileCameraDrag()
    {
        Touch touch0;                   //touch0 will store informations about your touch

        if (Input.touchCount == 1 )     //Called if only one finger is touching screen
        {
            touch0 = Input.GetTouch(0); //Save informatiouns about user touch in variable touch0

            Ray ray = cam.ScreenPointToRay(touch0.position);    //New ray from middle of camera to position of touch
            RaycastHit hit;                                     //In hit will be stored informations about object that ray hit

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))     //Called if infinity long ray hit something what is layer of terrainLayer 
            {
                if (scanOldRay)                 //This will be called once you touch screen, to save your old ray hit position needed for calculating deltaPosition
                {
                    oldRayPos = hit.point;      //Store actual ray hit position in oldRayPos variable
                    scanOldRay = false;         //scan old ray will be no more needed so it will turn it self off
                }

                Vector3 deltaRayPosition = hit.point - oldRayPos;       //Calculation of how much did ray move (deltaPosition)
                deltaRayPosition.y = 0f;                                //We dont want to move camera on Y axes, so we will 0 value

                transform.Translate(-deltaRayPosition * smoot * mobileMoveSpeed);       //Move our camera in oposit direction that ray did move
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minXpos, maxXpos), 0, Mathf.Clamp(transform.position.z, minYpos, maxYpos));      //Locking position of camera, that you can drag out of map
            }
        }
        
        else if(Input.touchCount > 1)       //Called if mor than 1 finger touch the screen
        {
            touch0 = new Touch();           //Restarting informations about your touch, without it if you touch screen with more fingers camera will start glitching
            scanOldRay = true;              //We will need to scan oldRay position after again
        }

        else if (Input.touchCount == 0)     //Called if no finger touch the screen
        {
            scanOldRay = true;                  //We will need to scan oldRay position after again
            oldRayPos = new Vector3(0, 0, 0);   //Resets oldRayPosition
        }
    }            //Function CameraDrag for mobile

    private void MobileCameraZoom()
    {
        if (Input.touchCount > 1)              //Called if more than one finger touches screen
        {
            Touch touch0 = Input.GetTouch(0);   //Store informations about first touch
            Touch touch1 = Input.GetTouch(1);   //Store informations about second touch

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;     //Store previous position of first touch
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;     //Store previous position of second touch

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;    //Calculate previous magnitude between touches, needed for calculating delta magnitude
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;    //Calculate magnitude between touches, needed for calculating delta magnitude

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;           //Calculate delta position of touches magnitude

            if (deltaMagnitudeDiff > minMovement || deltaMagnitudeDiff < -minMovement)  //Called if delta position is in range of minMovement to start zoom
            {
                if ((touch0.deltaPosition.y < maxMovementY && touch0.deltaPosition.y > -maxMovementY) || (touch1.deltaPosition.y < maxMovementY && touch1.deltaPosition.y > -maxMovementY)) //Called if movements on Y axes are in range of maxMovement to start zoom, more about maxMovementY variable in variables commentaries
                {
                    deltaMagnitudeDiff = (deltaMagnitudeDiff / Screen.height) * 1080;   //Calculate deltaMagnitudeDiff same for every screen resolution, so speed of zoom is same on every device

                    cam.transform.Translate(0, 0, -deltaMagnitudeDiff * smoot * mobileZoomSpeed);   //Moves camera by inverted deltaMagnitudeDiff
                    cam.transform.localPosition = new Vector3(0, 0, Mathf.Clamp(cam.transform.localPosition.z, -maxZoom, -minZoom)); //Lock local position of camera, so you cant zoom to far
                }
            }
        }
    }            //Function CameraZoom for mobile

    private void MobileCameraTilt()
    {
        if (Input.touchCount > 1)              //If more then 2 fingers touch screen
        {
            Touch touch0 = Input.GetTouch(0);   //Store informations about first touch
            Touch touch1 = Input.GetTouch(1);   //Store informations about second touch

            if ((touch0.deltaPosition.y > 0 && touch1.deltaPosition.y > 0) || (touch0.deltaPosition.y < 0 && touch1.deltaPosition.y < 0))       //If both fingers are moving in same direction on Y axes
            {
                Vector2 midPoint = (touch0.position + touch1.position) / 2;             //Calculate position of middle between fingers

                if (scanOld)                //This will call once you touch screen, to save your oldMidPoint position needed for calculating deltaPosition
                {
                    oldMidPoint = midPoint; //Store mid point in oldMidPoint
                    scanOld = false;        //scan old ray will be no more needed so it will turn it self off
                }

                midPoint = (touch0.position + touch1.position) / 2;     //Calculate new midPoint

                Vector2 deltaMidPoint = midPoint - oldMidPoint;         //Calculate delta position of midPoint
                oldMidPoint = midPoint;                                 //Store new value in oldMidPoint

                deltaMidPoint.y = (deltaMidPoint.y / Screen.height) * 1080; //Calculate delta position same for every screen resolution, so speed of tilt is same on every device

                rotX += deltaMidPoint.y * smoot * mobileTiltSpeed;      //calculate value of camera tilt
                rotX = Mathf.Clamp(rotX, -maxRot, -minRot);             //Clamp maximal and minimal tilt of camera
                tilter.transform.localEulerAngles = new Vector3(-rotX, 0, 0);   //change tilt
            }
        }

        else if (Input.touchCount == 0)
        {
            scanOld = true;     //We will need to scan olMid position after again
        }
    }            //Function CameraTilt for mobile

    private void MobileCameraRotate()
    {
        if (Input.touchCount > 1)              //Called if more the one finger touch screen
        {
            Touch touch0 = Input.GetTouch(0);   //Store informations about first touch
            Touch touch1 = Input.GetTouch(1);   //Store informations about second touch

            float deltaRotate;                  //Variable of deltaRotations

            Touch leftTouch = new Touch();    //Store informations about touch that is more left
            Touch rightTouch = new Touch();   //Store informations about touch that is more right

            if (touch0.position.x < touch1.position.x)  //if X position of touch0 is lower tahn X position of touch1
            {
                leftTouch = touch0;                     //then touch0 is left touch
                rightTouch = touch1;                    //and touch1 is right touch
            }
            else if (touch0.position.x > touch1.position.x) //if X position of touch0 is higher tahn X position of touch1
            {
                leftTouch = touch1;                     //then touch1 is left touch
                rightTouch = touch0;                    //and touch0 is right touch
            }

            if (leftTouch.deltaPosition.y >= 0 && rightTouch.deltaPosition.y <= 0)      //Called when delta positions are diferent. This rotation is clockwise
            {
                deltaRotate = ((Mathf.Abs(leftTouch.deltaPosition.y) + Mathf.Abs(rightTouch.deltaPosition.y)) / Screen.height) * 1080; //Calculate Abs value of both delta position and alculate delta rotation same for every screen resolution, so speed of rotation is same on every device
                rotor.transform.Rotate(0, -deltaRotate * smoot * mobileRotSpeed, 0);    //Rotate rotor of Camera object
            }

            else if (leftTouch.deltaPosition.y <= 0 && rightTouch.deltaPosition.y >= 0) //Called when delta positions are diferent. This rotation is counter clockwise
            {
                deltaRotate = ((Mathf.Abs(leftTouch.deltaPosition.y) + Mathf.Abs(rightTouch.deltaPosition.y)) / Screen.height) * 1080;  //Calculate Abs value of both delta position and alculate delta rotation same for every screen resolution, so speed of rotation is same on every device
                rotor.transform.Rotate(0, deltaRotate * smoot * mobileRotSpeed, 0);     //Rotate rotor of Camera object
            }
        }
    }          //Function CameraRotate for mobile

    //----------------------------------------------------------

    private void EditorCameraMovement()
    {
        EditorCameraDrag();                //Call windows drag function
        EditorCameraZoom();                //Call windows zoom function
        EditorCameraTilt();                //Call windows tilt function
        EditorCameraRotate();              //Call windows rotate function
    }        //Call all functions for editor

    private void EditorCameraDrag()
    {
        if (Input.GetMouseButton(0) && Input.touchCount == 0)            //Called while user holdin left mouse button
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);    //New ray from middle of camera to position of mouse
            RaycastHit hit;                                         //In hit will be stored informations about object that ray hit

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))     //Called if infinity long ray hit something what is layer of terrainLayer 
            {
                if (editorScanOldRay)   //This will be called once you click, to save your old ray hit position needed for calculating deltaPosition
                {
                    editorOldRayPos = hit.point;    //Store actual ray hit position in oldRayPos variable
                    editorScanOldRay = false;       //scan old ray will be no more needed so it will turn it self off
                }

                Vector3 deltaRayPosition = hit.point - editorOldRayPos; //Calculation of how much did ray move (deltaPosition)
                deltaRayPosition.y = 0f;                                //We dont want to move camera on Y axes, so we will 0 value

                transform.Translate(-deltaRayPosition * smoot * editorMoveSpeed);       //Move our camera in oposit direction that ray did move
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minXpos, maxXpos), 0, Mathf.Clamp(transform.position.z, minYpos, maxYpos));      //Locking position of camera, that you can drag out of map

            }
        }
        if (Input.GetMouseButtonUp(0))          //Called when user release left mouse button
        {
            editorScanOldRay = true;                    //We will need to scan oldRay position after again
            editorOldRayPos = new Vector3(0, 0, 0);     //Resets oldRayPosition
        }
    }            //Function CameraDrag for editor

    private void EditorCameraZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && cam.transform.localPosition.z > -maxZoom)     //Called when mouse scrollWheel axis < 0 and if localPosition is higher then -maxZoom
        {
            cam.transform.Translate(Vector3.back * editorZoomSpeed);        //Move camera back
        }

        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && cam.transform.localPosition.z < -minZoom) //Called when mouse scrollWheel axis < 0 and if localPosition is higher then -maxZoom
        {
            cam.transform.Translate(Vector3.forward * editorZoomSpeed);     //Move camera forward
        }
    }            //Function CameraDrag for editor

    private void EditorCameraTilt()
    {
        if (Input.GetMouseButton(1) && Input.touchCount == 0)              //Called when Left mouse button pressed
        {
            mRotY += Input.GetAxis("Mouse Y") * smoot * editorTiltSpeed * 100f;    //Calculate rotation by Mouse Y axis
            mRotY = Mathf.Clamp(mRotY, -maxRot, -minRot);                          //Clamp maximal and minimal tilt of camera
            tilter.transform.localEulerAngles = new Vector3(-mRotY, 0, 0);         //change tilt
        }
    }            //Function CameraDrag for editor

    private void EditorCameraRotate()
    {
        if (Input.GetMouseButton(1) && Input.touchCount == 0)              //Called when right mouse button pressed
        {
            mRotX += Input.GetAxis("Mouse X");    //Calculate rotation by Mouse X axis
            rotor.transform.Rotate(0, mRotX * smoot * editorRotSpeed * 100f, 0);    //Rotate rotor
            mRotX = 0;                            //Zero rotation value
        }
    }          //Function CameraDrag for editor

    //----------------------------------------------------------
}