using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemController : MonoBehaviour
{
    PlayerInput playerInput;


    private Vector3 worldMousePosition;

    private InputAction.CallbackContext mouseContext;

    private GameObject movementItem, movementItemParent, touchedItem;
    private Vector3 itemLocalPos, itemRotation;

    private float rotationTime = 0;
    [SerializeField]
    float rotationSpeed = 1.5f;
    private bool isRotation = false, removeItemOnce = false;

    enum RotationMode { Left, Right };
    RotationMode rotationMode;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

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
        mouseContext = context;

        if (isRotation) return;

        if (context.phase == InputActionPhase.Started)  //素材の処理
        {
            RaycastHit2D hitItem = Physics2D.Raycast(worldMousePosition, Vector2.zero);

            if (hitItem.collider != null && hitItem.collider.CompareTag("ItemObject_Piece"))
            {
                movementItem = hitItem.collider.gameObject;
                movementItemParent = movementItem.transform.parent.gameObject;

                itemLocalPos = movementItem.transform.localPosition;
                itemRotation = movementItemParent.transform.localEulerAngles;
            }
        }
    }

    public void OnRotateItem(InputAction.CallbackContext context)
    {
        if (movementItemParent == null) return;
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
        }
    }

    void HoldingItemControl()
    {
        if (movementItemParent == null) return;
        if (removeItemOnce) return;

        if (mouseContext.phase == InputActionPhase.Started || mouseContext.phase == InputActionPhase.Performed)
        {
            worldMousePosition.z = -1;
            movementItemParent.transform.position = worldMousePosition;

            touchedItem = movementItemParent;
        }
        else
        {
            worldMousePosition.z = 0;
            movementItemParent.transform.position = worldMousePosition;

            movementItem = null;
            if(!isRotation) movementItemParent = null;    //素材の回転が終わるまでmovement_Item_Parentをnullにしない
        }

        //GameObject[] ItemObjects = FindObjectsOfType<GameObject>();  //素材の重なりをいろいろする
        //for(int i = 0; i < ItemObjects.Length; i++)
        //{
        //    if (ItemObjects[i].CompareTag("ItemObjects") && ItemObjects[i] != touchedItem)
        //    {
        //        Vector3 v3 = ItemObjects[i].transform.position;
        //        v3.z = 1;
        //        ItemObjects[i].transform.position = v3;
        //    }
        //}
    }

    Vector3 oldRotation;
    private float currentVelocity;
    void SmoothlyRotate()
    {
        if (movementItemParent == null) return;  //素材をつかんでいないときmovement_Item_Parentがnullになる

        if (mouseContext.phase == InputActionPhase.Waiting)  //素材を話した瞬間
        {
            removeItemOnce = true;
        }

        if (movementItemParent.transform.localEulerAngles != itemRotation)
        {
            if (!isRotation) oldRotation = movementItemParent.transform.localEulerAngles;  //最初の1回、oldRotationに回転前の角度を格納する

            if (itemRotation.z == -60 && rotationMode == RotationMode.Right)
            {
                itemRotation.z += 360;
            }
            if (itemRotation.z == 360 && rotationMode == RotationMode.Left)
            {
                itemRotation.z -= 360;
            }

            Vector3 newRotation = oldRotation;

            newRotation.z = Mathf.SmoothDampAngle(movementItemParent.transform.localEulerAngles.z, itemRotation.z, ref currentVelocity, rotationTime, rotationSpeed * 1000);  //スムーズに回転させる

            if (Mathf.Abs(newRotation.z - itemRotation.z) < 0.1f)
            {
                //回転の終了の処理
                movementItemParent.transform.localEulerAngles = itemRotation;
                isRotation = false;
                return;
            }

            movementItemParent.transform.localEulerAngles = newRotation;
            rotationTime += Time.deltaTime;
            isRotation = true;
        }
        else if(rotationTime != 0)
        {
            rotationTime = 0;
            isRotation = false;
            if(removeItemOnce)
            {
                movementItemParent = null;
            }
            removeItemOnce = false;
        }
    }
}
