using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemController : MonoBehaviour
{
    PlayerInput playerInput;


    private Vector3 worldMousePosition;

    private InputAction.CallbackContext mouseContext;

    private GameObject movement_Item, movement_Item_Parent;
    private Vector3 itemLocalPos, itemRotation;

    private float rotationTime = 0;
    private bool isRotation = false;

    enum RotationMode { Left, Right };
    RotationMode rotationMode;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        HoldingItemControl();

        SmoothlyRotate();
    }

    // マウス座標が更新された時に通知するコールバック関数
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        worldMousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnHoldItem(InputAction.CallbackContext context)
    {
        if (isRotation) return;

        if (context.phase == InputActionPhase.Started)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldMousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                movement_Item = hit.collider.gameObject;
                movement_Item_Parent = movement_Item.transform.parent.gameObject;

                itemLocalPos = movement_Item.transform.localPosition;
                itemRotation = movement_Item_Parent.transform.localEulerAngles;
            }
        }

        mouseContext = context;
    }

    public void OnRotateItem(InputAction.CallbackContext context)
    {
        if (movement_Item_Parent == null) return;
        if (isRotation) return;

        if(context.started)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                //反時計回りに回転させる
                itemRotation.z = (int)itemRotation.z + 60;
                rotationMode = RotationMode.Left;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                //時計回りに回転させる
                itemRotation.z = (int)itemRotation.z - 60;  
                rotationMode = RotationMode.Right;
            }

            if (itemRotation.z > 360)
            {
                itemRotation.z -= 360;
            }
            else if (itemRotation.z < 0)
            {
                itemRotation.z += 360;
            }
        }
    }

    void HoldingItemControl()
    {
        if (movement_Item_Parent == null) return;

        if (mouseContext.phase == InputActionPhase.Started || mouseContext.phase == InputActionPhase.Performed)
        {
            worldMousePosition.z = 0;
            movement_Item_Parent.transform.position = worldMousePosition;
        }
        else
        {
            movement_Item = null;
            if(!isRotation) movement_Item_Parent = null;
        }
    }

    Vector3 oldRotation;
    private float currentVelocity;
    void SmoothlyRotate()
    {
        if (movement_Item_Parent == null) return;

        if (movement_Item_Parent.transform.localEulerAngles != itemRotation)
        {
            if (!isRotation) oldRotation = movement_Item_Parent.transform.localEulerAngles;

            Vector3 newRotation = oldRotation;

            newRotation.z = Mathf.SmoothDampAngle(movement_Item_Parent.transform.localEulerAngles.z, itemRotation.z, ref currentVelocity, rotationTime, 1000f);
            Debug.Log("Approximately : " + Mathf.Approximately(newRotation.z, itemRotation.z));  //ここバグる
            if (Mathf.Approximately(newRotation.z, itemRotation.z))
            {
                movement_Item_Parent.transform.localEulerAngles = itemRotation;

                if (movement_Item_Parent.transform.localEulerAngles.z <= 0)
                {
                    movement_Item_Parent.transform.localEulerAngles
                        = new Vector3(0, 0, 360);
                }
                else if (movement_Item_Parent.transform.localEulerAngles.z >= 360)
                {
                    movement_Item_Parent.transform.localEulerAngles
                        = new Vector3(0, 0, 0);
                } 

                isRotation = false;
                return;
            }
            else
            {
                Debug.Log("newRotation.z : " + newRotation.z);
                Debug.Log("itemRotation.z : " + itemRotation.z);
            }

            if(rotationMode == RotationMode.Left && newRotation.z >= 360)
            {
                newRotation.z = 0;
            }
            else if(rotationMode == RotationMode.Right && newRotation.z <= 0)
            {
                newRotation.z = 0;
            }

            movement_Item_Parent.transform.localEulerAngles = newRotation;
            rotationTime += Time.deltaTime;
            isRotation = true;
        }
        else if(rotationTime != 0)
        {
            rotationTime = 0;
            isRotation = false;
        }
    }
}
