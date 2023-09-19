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
    private Transform[] workbenchChilds;

    private GameObject movementItem, touchedItem;
    internal GameObject movementItemParent;
    private Transform[] movementItemChilds;
    private Vector3 itemRotation;

    private float rotationTime = 0;
    [SerializeField]
    float rotationSpeed = 1.5f;
    private bool isRotation = false, removeItemOnce = false;

    //最後に持っていた素材を入れる変数
    GameObject heldItem;

    //ChackInstallingToWorkbench内でのみ使う
    HexInfomation judgeInfo;

    internal bool canDoFitting = false;

    enum RotationMode { Left, Right };
    RotationMode rotationMode;

    //SmoothlyRotate内でのみで使う変数
    Vector3 oldRotation;
    private float currentVelocity;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        judgeInfo = GetComponent<HexInfomation>();
        SetWorkbench();
    }

    void Update()
    {
        if(workbench == null)
        {
            SetWorkbench();
        }

        ChangeLayer();

        HoldingItemControl();

        if (isRotation)
        {
            SmoothlyRotate();
        }

        PutOnWorkbench();

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

                movementItemChilds = movementItemParent.transform.GetComponentsInChildren<Transform>();  //index0 に親オブジェクトが入る

                itemRotation = movementItemParent.transform.localEulerAngles;

                heldItem = movementItemParent;

                movementItemParent.GetComponent<ItemInfomation>().isFitting = false;
                for (int i = 1; i < movementItemChilds.Length; i++)
                {
                    HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();  //作業台にはまっていたかを確認
                    if (info_i.fittingTarget != null)
                    {
                        HexInfomation info_w = info_i.fittingTarget.GetComponent<HexInfomation>();
                        info_i.isFitting = false;
                        info_i.fittingTarget = null;
                        info_w.isFitting = false;
                        info_w.fittingTarget = null;
                    }
                }
            }
        }
    }

    void SetWorkbench()
    {
        workbench = GameObject.FindWithTag("Workbench");
        workbenchChilds = workbench.GetComponentsInChildren<Transform>();
    }

    void ChackInstallingToWorkbench()  //作業台に素材を置けるかを確認する
    {
        if (movementItemParent == null) return;

        if (mouseContext.phase != InputActionPhase.Canceled)  //作業台の処理
        {
            int canFittingNum = 0;

            int mask = 1 << 7;  //Layer7にWorkbenchを設定
            RaycastHit2D _hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
            if (_hitWorkbench.collider != null)
            {
                HexInfomation info_origin = _hitWorkbench.collider.GetComponent<HexInfomation>();

                for (int i = 1; i < workbenchChilds.Length; i++)
                {
                    HexInfomation info_w = workbenchChilds[i].GetComponent<HexInfomation>();

                    for (int j = 1; j < movementItemChilds.Length; j++)
                    {
                        HexInfomation info_i = movementItemChilds[j].GetComponent<HexInfomation>();  //素材の位置情報を取得する
                        judgeInfo.q = info_origin.q + info_i.q;
                        judgeInfo.r = info_origin.r + info_i.r;
                        judgeInfo.s = info_origin.s + info_i.s;

                        if(judgeInfo.q == info_w.q && judgeInfo.r == info_w.r && judgeInfo.s == info_w.s && !info_w.isFitting)
                        {
                            info_i.canFitting = true;
                            info_w.canFitting = true;
                            canFittingNum++;
                            break;
                        }
                        else
                        {
                            info_i.canFitting = false;
                            info_w.canFitting = false;
                        }
                    }
                }

                if(canFittingNum == movementItemChilds.Length-1)
                {
                    //Debug.Log("置けます");
                    canDoFitting = true;
                }
                else
                {
                    canDoFitting = false;
                }
            }
            else
            {
                for (int i = 1; i < workbenchChilds.Length; i++)
                {
                    HexInfomation info_w = workbenchChilds[i].GetComponent<HexInfomation>();
                    info_w.canFitting = false;
                }
                canDoFitting = false;
            }
        }
    }

    void PutOnWorkbench()  //作業台に素材を置く
    {
        if (heldItem == null) return;
        if (movementItemParent == null) return;
        int mask = 1 << 7;  //Layer7にWorkbenchを設定
        RaycastHit2D _hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
        if (!_hitWorkbench)
        {
            canDoFitting = false;
        }

        if (mouseContext.phase == InputActionPhase.Waiting && canDoFitting)  //素材を離した直後の処理を書く
        {
            Vector3 fitPos = _hitWorkbench.transform.position;
            fitPos.z = movementItemParent.transform.position.z;
            movementItemParent.transform.position = fitPos;

            movementItemParent.GetComponent<ItemInfomation>().isFitting = true;

            HexInfomation info_origin = _hitWorkbench.collider.GetComponent<HexInfomation>();
            for (int i = 1; i < movementItemChilds.Length; i++)
            {
                HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();  //まず素材の位置情報を取得する
                judgeInfo.q = info_origin.q + info_i.q;
                judgeInfo.r = info_origin.r + info_i.r;
                judgeInfo.s = info_origin.s + info_i.s;

                for (int j = 1; j < workbenchChilds.Length; j++)  //作業台の位置情報と比べる
                {
                    HexInfomation info_w = workbenchChilds[j].GetComponent<HexInfomation>();

                    if (judgeInfo.q == info_w.q && judgeInfo.r == info_w.r && judgeInfo.s == info_w.s)
                    {
                        info_i.isFitting = true;
                        info_w.isFitting = true;
                        info_i.fittingTarget = info_w.gameObject;
                        info_w.fittingTarget = info_i.gameObject;
                        info_i.canFitting = false;
                        break;
                    }

                }
                info_i.canFitting = false;
            }

            movementItemParent = null;
            movementItemChilds = new Transform[0];

        }
        canDoFitting = false;
        removeItemOnce = false;
    }

    void ChangeLayer()
    {
        if (movementItemChilds != null)
        {
            for (int i = 1; i < movementItemChilds.Length; i++)
            {
                if (mouseContext.phase == InputActionPhase.Performed)
                {
                    movementItemChilds[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
                }
                else if (mouseContext.phase == InputActionPhase.Waiting)
                {
                    movementItemChilds[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
                }
            }
        }


        HexInfomation[] Items = FindObjectsOfType<HexInfomation>();

        for (int i = 0; i < Items.Length; i++)
        {
            GameObject _gameObject = Items[i].gameObject;
            if (_gameObject.CompareTag("ItemObject_Piece"))
            {
                if (Items[i].isFitting)
                {
                    _gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
                else if (_gameObject.transform.parent.gameObject != heldItem)
                {
                    _gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
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
                if (itemRotation.z == 360)
                {
                    itemRotation.z -= 360;
                }
                rotationMode = RotationMode.Left;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                //時計回りに回転させる
                itemRotation.z = (int)itemRotation.z - 60;
                if (itemRotation.z == -60)
                {
                    itemRotation.z += 360;
                }
                rotationMode = RotationMode.Right;
            }

            for(int i = 1; i < movementItemChilds.Length; i++)
            {
                HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();

                if(rotationMode == RotationMode.Right)
                {
                    (info_i.q, info_i.r, info_i.s) = (-info_i.r, -info_i.s, -info_i.q);
                }
                else
                {
                    (info_i.q, info_i.r, info_i.s) = (-info_i.s, -info_i.q, -info_i.r);
                }
                //info_i.hexRotation = (int)itemRotation.z;
            }
            isRotation = true;
        }
    }

    void HoldingItemControl()
    {
        //Debug.Log("movementItemParent is " + movementItemParent);
        //Debug.Log("removeItemOnce is " + removeItemOnce);

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
            if(!isRotation && !canDoFitting) movementItemParent = null;    //素材の回転が終わるまでmovement_Item_Parentをnullにしない
        }
    }

    void SmoothlyRotate()
    {
        if (movementItemParent == null) return;  //素材をつかんでいないときmovement_Item_Parentがnullになる

        if (mouseContext.phase == InputActionPhase.Waiting)  //素材を話した瞬間
        {
            removeItemOnce = true;
        }

        if (movementItemParent.transform.localEulerAngles != itemRotation)
        {
            if (!removeItemOnce)
            {
                if (!isRotation) oldRotation = movementItemParent.transform.localEulerAngles;  //最初の1回、oldRotationに回転前の角度を格納する

                //if (itemRotation.z == -60 && rotationMode == RotationMode.Right)
                //{
                //    itemRotation.z += 360;
                //}
                //if (itemRotation.z == 360 && rotationMode == RotationMode.Left)
                //{
                //    itemRotation.z -= 360;
                //}
            }

            Vector3 newRotation = oldRotation;

            newRotation.z = Mathf.SmoothDampAngle(movementItemParent.transform.localEulerAngles.z, itemRotation.z, ref currentVelocity, rotationTime, rotationSpeed * 1000);  //スムーズに回転させる

            if (Mathf.Abs(newRotation.z - itemRotation.z) < 0.1f || removeItemOnce)
            {
                //回転の終了の処理
                movementItemParent.transform.localEulerAngles = itemRotation;
                rotationTime = 0;
                isRotation = false;
                return;
            }

            movementItemParent.transform.localEulerAngles = newRotation;
            rotationTime += Time.deltaTime;
            isRotation = true;
            //Invoke("SmoothlyRotate", Time.deltaTime);
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
