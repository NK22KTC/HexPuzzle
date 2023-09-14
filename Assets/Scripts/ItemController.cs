using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemController : MonoBehaviour
{
    PlayerInput playerInput;


    private Vector3 worldMousePosition;

    private InputAction.CallbackContext mouseContext;

    GameObject workbench;
    [SerializeField]
    private GameObject[] workbenchChilds;

    private GameObject movementItem, movementItemParent, touchedItem;
    private Transform[] movementItemChilds;
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

        ChackInstallingToWorkbench();
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

                movementItemChilds = movementItemParent.transform.GetComponentsInChildren<Transform>();
                foreach (Transform child in movementItemChilds)
                {
                    Debug.Log(child.name);
                }

                itemLocalPos = movementItem.transform.localPosition;
                itemRotation = movementItemParent.transform.localEulerAngles;
            }
        }
    }

    //void SetWorkbench()
    //{
    //    if(workbench == null)
    //    {

    //    }
    //}

    void ChackInstallingToWorkbench()
    {
        if (movementItemParent == null) return;

        if (mouseContext.phase != InputActionPhase.Canceled)  //作業台の処理
        {
            int mask = 1 << 7;  //Layer7にWorkbenchを設定
            RaycastHit2D hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
            if (hitWorkbench.collider != null)
            {
                HexInfomation info_origin = hitWorkbench.collider.GetComponent<HexInfomation>();
                Debug.Log("R : " + info_origin.r + ", S : " + info_origin.s + ", Q : " + info_origin.q);

                for(int i = 1;  i < info_origin.r; i++)
                {
                    HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();  //まず素材の位置情報を取得する
                    Debug.Log("Q : " + info_i.q + ", R : " + info_i.r + ", S : " + info_i.s);

                    HexInfomation judgeInfo = new HexInfomation();
                    judgeInfo.q = info_origin.q + info_i.q;
                    judgeInfo.r = info_origin.r + info_i.r;
                    judgeInfo.s = info_origin.s + info_i.s;
                    Debug.Log("Q : " + judgeInfo.q + ", R : " + judgeInfo.r + ", S : " + judgeInfo.s);

                    foreach (GameObject workbenchChild in workbenchChilds)  //作業台の位置情報と比べる
                    {
                        HexInfomation info_w = workbenchChild.GetComponent<HexInfomation>();
                        Debug.Log("R : " + info_w.r + ", S : " + info_w.s + ", Q : " + info_w.q);
                        Debug.Log("R : " + judgeInfo.r + ", S : " + judgeInfo.s + ", Q : " + judgeInfo.q);

                        if (judgeInfo.q == info_w.q && judgeInfo.r == info_w.r && judgeInfo.s == info_w.s)
                        {
                            Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                            info_i.canFitting = true;
                        }
                    }
                }

                int canFittingNum = 0;
                for (int i = 1; i < info_origin.r; i++)
                {
                    HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();
                    if (info_i.canFitting)
                    {
                        canFittingNum++;
                    }
                }
                Debug.Log(canFittingNum + "個");
                if(canFittingNum == movementItemChilds.Length)
                {
                    Debug.Log("置けます");
                }
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
