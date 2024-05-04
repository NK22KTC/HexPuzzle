using UnityEngine;
using UnityEngine.InputSystem;

public class ItemController : MonoBehaviour
{
    PlayerInput playerInput;

    private Vector3 worldMousePosition;

    private InputAction.CallbackContext mouseContext;

    GameObject workbench;  //錬金窯
    private Transform[] workbenchChilds;

    private GameObject movementItem;
    private GameObject touchedItem;
    internal GameObject movementItemParent;
    private Transform[] movementItemChilds;
    private Vector3 itemRotation;

    private float rotationTime = 0;
    [SerializeField]
    float rotationSpeed = 1.5f;
    private bool isRotation = false;  //素材が回転しているか
    private bool removeItemOnce = false;

    //最後に持っていた素材を入れる変数
    GameObject lastHoldItem;

    //ChackInstallingToWorkbench内でのみ使う
    //素材を錬金窯にはめれるかの判定用
    HexInfomation judgeInfo;

    internal bool canFit = false;

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

        CheckInstallingToWorkbench();
    }

    // マウス座標が更新された時に通知するコールバック関数
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        worldMousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnHoldItem(InputAction.CallbackContext context)  //素材をつかむ処理 (InputSystemのコールバックで処理)
    {
        mouseContext = context;
        if (isRotation) return;
        if (context.phase != InputActionPhase.Started) { return; }
 
        RaycastHit2D hitItem = Physics2D.Raycast(worldMousePosition, Vector2.zero, 0, layerMask: 64);

        if (hitItem.collider == null) { return; }



        if (hitItem.collider.CompareTag("ItemObject_Piece"))  //素材をクリックしたら持ち上げる
        {
            movementItem = hitItem.collider.gameObject;
            movementItemParent = movementItem.transform.parent.gameObject;
        }
        else if (hitItem.collider.CompareTag("ItemSample"))  //素材のサンプルをクリックしたらそれの実物を生成して持ち上げる、
        {
            ItemInfomation info = hitItem.transform.parent.GetComponent<ItemInfomation>();
            GameObject newObject = Instantiate(info.instantiateObject);
            movementItemParent = newObject;

            GameManager.instance.ScoreAndMoneyUpdate(info.PieceNum * -5, false);
        }

        SoundManager.instance.PlaySE(0);  //素材をつかむときの効果音を鳴らす

        movementItemChilds = movementItemParent.transform.GetComponentsInChildren<Transform>();  //持っている素材のピースを全て呼び出す
        itemRotation = movementItemParent.transform.localEulerAngles;
        lastHoldItem = movementItemParent;

        movementItemParent.GetComponent<ItemInfomation>().isFitting = false;
        CheckItemFitted();
    }

    private void CheckItemFitted()  //錬金窯にはまっていたかを確認、はまっていたら解除する
    {
        for (int i = 1; i < movementItemChilds.Length; i++)
        {
            HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();
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

    void SetWorkbench()
    {
        workbench = GameObject.FindWithTag("Workbench");
        workbenchChilds = workbench.GetComponentsInChildren<Transform>();
    }

    void CheckInstallingToWorkbench()  //作業台に素材を置けるかを確認する
    {
        if (movementItemParent == null) return;
        if (mouseContext.phase == InputActionPhase.Canceled) { return; }

        //持っている素材のピースが錬金窯にはめれる数
        //これが持っている素材のピース数と同じ時にはめる
        int canFittingCount = 0;

        int mask = 1 << 7;  //Workbenchレイヤー
        RaycastHit2D hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
        if (hitWorkbench.collider == null)
        {
            for (int i = 1; i < workbenchChilds.Length; i++)
            {
                HexInfomation info_w = workbenchChilds[i].GetComponent<HexInfomation>();
                info_w.canFitting = false;
            }
            canFit = false;
            return;
        }

        HexInfomation infoOrigin = hitWorkbench.collider.GetComponent<HexInfomation>();

        for (int i = 1; i < workbenchChilds.Length; i++)
        {
            HexInfomation infoWB = workbenchChilds[i].GetComponent<HexInfomation>();  //

            for (int j = 1; j < movementItemChilds.Length; j++)
            {
                HexInfomation infoItem = movementItemChilds[j].GetComponent<HexInfomation>();  //素材の六角座標の情報を取得する
                judgeInfo.q = infoOrigin.q + infoItem.q;
                judgeInfo.r = infoOrigin.r + infoItem.r;
                judgeInfo.s = infoOrigin.s + infoItem.s;

                if (judgeInfo.q == infoWB.q && judgeInfo.r == infoWB.r && judgeInfo.s == infoWB.s && !infoWB.isFitting)
                {
                    infoItem.canFitting = true;
                    infoWB.canFitting = true;
                    canFittingCount++;
                    break;
                }
                else
                {
                    infoItem.canFitting = false;
                    infoWB.canFitting = false;
                }
            }
        }

        if (canFittingCount == movementItemChilds.Length - 1)  //素材のピース数と同じときは置く
        {
            //Debug.Log("置けます");
            canFit = true;
        }
        else
        {
            canFit = false;
        }

    }

    void PutOnWorkbench()  //作業台に素材を置く
    {
        if (lastHoldItem == null) return;
        if (movementItemParent == null) return;
        int mask = 1 << 7;  //Layer7にWorkbenchを設定
        RaycastHit2D hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
        if (!hitWorkbench)
        {
            canFit = false;
        }

        if (mouseContext.phase == InputActionPhase.Waiting && canFit)  //素材を離した直後の処理を書く
        {
            Vector3 fitPos = hitWorkbench.transform.position;
            fitPos.z = movementItemParent.transform.position.z;
            movementItemParent.transform.position = fitPos;

            movementItemParent.GetComponent<ItemInfomation>().isFitting = true;

            SoundManager.instance.PlaySE(2);

            HexInfomation infoOrigin = hitWorkbench.collider.GetComponent<HexInfomation>();
            for (int i = 1; i < movementItemChilds.Length; i++)
            {
                HexInfomation infoItem = movementItemChilds[i].GetComponent<HexInfomation>();  //素材の位置情報を取得する

                //
                judgeInfo.q = infoOrigin.q + infoItem.q;
                judgeInfo.r = infoOrigin.r + infoItem.r;
                judgeInfo.s = infoOrigin.s + infoItem.s;

                for (int j = 1; j < workbenchChilds.Length; j++)  //作業台の位置情報と比べる
                {
                    HexInfomation infoWB = workbenchChilds[j].GetComponent<HexInfomation>();

                    if (judgeInfo.q == infoWB.q && judgeInfo.r == infoWB.r && judgeInfo.s == infoWB.s)  //
                    {
                        infoItem.isFitting = true;
                        infoWB.isFitting = true;
                        infoItem.fittingTarget = infoWB.gameObject;
                        infoWB.fittingTarget = infoItem.gameObject;
                        infoItem.canFitting = false;
                        break;
                    }

                }
                infoItem.canFitting = false;
            }

            movementItemParent = null;
            movementItemChilds = new Transform[0];

        }
        canFit = false;
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
            GameObject piece = Items[i].gameObject;
            if (piece.CompareTag("ItemObject_Piece"))
            {
                if (Items[i].isFitting)
                {
                    piece.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
                else if (piece.transform.parent.gameObject != lastHoldItem)
                {
                    piece.GetComponent<SpriteRenderer>().sortingOrder = 2;
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
            if(!isRotation && !canFit) movementItemParent = null;    //素材の回転が終わるまでmovement_Item_Parentをnullにしない

            SoundManager.instance.PlaySE(1);
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
