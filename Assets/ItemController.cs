using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemController : MonoBehaviour
{
    PlayerInput playerInput;

    
    private Vector2 screenMousePosition;
    private Vector3 worldMousePosition;

    private InputAction.CallbackContext mouseContext;

    private GameObject movement_Item, movement_Item_Parent;
    private Vector3 itemLocalPos, itemRotation;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        HoldingItemControl();
    }

    // マウス座標が更新された時に通知するコールバック関数
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        worldMousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnHoldItem(InputAction.CallbackContext context)
    {
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

        if(context.started)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                //反時計回りに回転させる
                Debug.Log("aaaaaa");
                itemRotation.z += 60;
                if (itemRotation.z <= 0) itemRotation.z += 360;

                movement_Item_Parent.transform.localEulerAngles = itemRotation;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                //時計回りに回転させる
                Debug.Log("dddddd");
                itemRotation.z -= 60;
                if (itemRotation.z >= 360) itemRotation.z -= 360;

                movement_Item_Parent.transform.localEulerAngles = itemRotation;
            }
        }
    }

    void HoldingItemControl()
    {
        if (movement_Item_Parent == null) return;

        if (mouseContext.phase is InputActionPhase.Started or InputActionPhase.Performed)
        {
            worldMousePosition.z = 0;
            //movement_Item_Parent.transform.position = worldMousePosition - itemLocalPos;
            float itemAngle = movement_Item_Parent.transform.localEulerAngles.z;
            Debug.Log(itemAngle);
            movement_Item_Parent.transform.position = worldMousePosition;
        }
        else
        {
            movement_Item = null;
            movement_Item_Parent = null;
        }
    }
}
