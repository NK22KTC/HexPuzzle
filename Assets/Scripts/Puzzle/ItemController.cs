using UnityEngine;
using UnityEngine.InputSystem;

public class ItemController : MonoBehaviour
{
    PlayerInput playerInput;

    private Vector3 worldMousePosition;

    private InputAction.CallbackContext mouseContext;

    GameObject workbench;  //�B���q
    private Transform[] workbenchChilds;

    private GameObject movementItem;
    private GameObject touchedItem;
    internal GameObject movementItemParent;
    private Transform[] movementItemChilds;
    private Vector3 itemRotation;

    private float rotationTime = 0;
    [SerializeField]
    float rotationSpeed = 1.5f;
    private bool isRotation = false;  //�f�ނ���]���Ă��邩
    private bool removeItemOnce = false;

    //�Ō�Ɏ����Ă����f�ނ�����ϐ�
    GameObject lastHoldItem;

    //ChackInstallingToWorkbench���ł̂ݎg��
    //�f�ނ�B���q�ɂ͂߂�邩�̔���p
    HexInfomation judgeInfo;

    internal bool canFit = false;

    enum RotationMode { Left, Right };
    RotationMode rotationMode;

    //SmoothlyRotate���ł݂̂Ŏg���ϐ�
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

    // �}�E�X���W���X�V���ꂽ���ɒʒm����R�[���o�b�N�֐�
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        worldMousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnHoldItem(InputAction.CallbackContext context)  //�f�ނ����ޏ��� (InputSystem�̃R�[���o�b�N�ŏ���)
    {
        mouseContext = context;
        if (isRotation) return;
        if (context.phase != InputActionPhase.Started) { return; }
 
        RaycastHit2D hitItem = Physics2D.Raycast(worldMousePosition, Vector2.zero, 0, layerMask: 64);

        if (hitItem.collider == null) { return; }



        if (hitItem.collider.CompareTag("ItemObject_Piece"))  //�f�ނ��N���b�N�����玝���グ��
        {
            movementItem = hitItem.collider.gameObject;
            movementItemParent = movementItem.transform.parent.gameObject;
        }
        else if (hitItem.collider.CompareTag("ItemSample"))  //�f�ނ̃T���v�����N���b�N�����炻��̎����𐶐����Ď����グ��A
        {
            ItemInfomation info = hitItem.transform.parent.GetComponent<ItemInfomation>();
            GameObject newObject = Instantiate(info.instantiateObject);
            movementItemParent = newObject;

            GameManager.instance.ScoreAndMoneyUpdate(info.PieceNum * -5, false);
        }

        SoundManager.instance.PlaySE(0);  //�f�ނ����ނƂ��̌��ʉ���炷

        movementItemChilds = movementItemParent.transform.GetComponentsInChildren<Transform>();  //�����Ă���f�ނ̃s�[�X��S�ČĂяo��
        itemRotation = movementItemParent.transform.localEulerAngles;
        lastHoldItem = movementItemParent;

        movementItemParent.GetComponent<ItemInfomation>().isFitting = false;
        CheckItemFitted();
    }

    private void CheckItemFitted()  //�B���q�ɂ͂܂��Ă��������m�F�A�͂܂��Ă������������
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

    void CheckInstallingToWorkbench()  //��Ƒ�ɑf�ނ�u���邩���m�F����
    {
        if (movementItemParent == null) return;
        if (mouseContext.phase == InputActionPhase.Canceled) { return; }

        //�����Ă���f�ނ̃s�[�X���B���q�ɂ͂߂�鐔
        //���ꂪ�����Ă���f�ނ̃s�[�X���Ɠ������ɂ͂߂�
        int canFittingCount = 0;

        int mask = 1 << 7;  //Workbench���C���[
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
                HexInfomation infoItem = movementItemChilds[j].GetComponent<HexInfomation>();  //�f�ނ̘Z�p���W�̏����擾����
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

        if (canFittingCount == movementItemChilds.Length - 1)  //�f�ނ̃s�[�X���Ɠ����Ƃ��͒u��
        {
            //Debug.Log("�u���܂�");
            canFit = true;
        }
        else
        {
            canFit = false;
        }

    }

    void PutOnWorkbench()  //��Ƒ�ɑf�ނ�u��
    {
        if (lastHoldItem == null) return;
        if (movementItemParent == null) return;
        int mask = 1 << 7;  //Layer7��Workbench��ݒ�
        RaycastHit2D hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
        if (!hitWorkbench)
        {
            canFit = false;
        }

        if (mouseContext.phase == InputActionPhase.Waiting && canFit)  //�f�ނ𗣂�������̏���������
        {
            Vector3 fitPos = hitWorkbench.transform.position;
            fitPos.z = movementItemParent.transform.position.z;
            movementItemParent.transform.position = fitPos;

            movementItemParent.GetComponent<ItemInfomation>().isFitting = true;

            SoundManager.instance.PlaySE(2);

            HexInfomation infoOrigin = hitWorkbench.collider.GetComponent<HexInfomation>();
            for (int i = 1; i < movementItemChilds.Length; i++)
            {
                HexInfomation infoItem = movementItemChilds[i].GetComponent<HexInfomation>();  //�f�ނ̈ʒu�����擾����

                //
                judgeInfo.q = infoOrigin.q + infoItem.q;
                judgeInfo.r = infoOrigin.r + infoItem.r;
                judgeInfo.s = infoOrigin.s + infoItem.s;

                for (int j = 1; j < workbenchChilds.Length; j++)  //��Ƒ�̈ʒu���Ɣ�ׂ�
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
                //�����v���ɉ�]������
                itemRotation.z = (int)itemRotation.z + 60;
                if (itemRotation.z == 360)
                {
                    itemRotation.z -= 360;
                }
                rotationMode = RotationMode.Left;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                //���v���ɉ�]������
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
            if(!isRotation && !canFit) movementItemParent = null;    //�f�ނ̉�]���I���܂�movement_Item_Parent��null�ɂ��Ȃ�

            SoundManager.instance.PlaySE(1);
        }
    }

    void SmoothlyRotate()
    {
        if (movementItemParent == null) return;  //�f�ނ�����ł��Ȃ��Ƃ�movement_Item_Parent��null�ɂȂ�

        if (mouseContext.phase == InputActionPhase.Waiting)  //�f�ނ�b�����u��
        {
            removeItemOnce = true;
        }

        if (movementItemParent.transform.localEulerAngles != itemRotation)
        {
            if (!removeItemOnce)
            {
                if (!isRotation) oldRotation = movementItemParent.transform.localEulerAngles;  //�ŏ���1��AoldRotation�ɉ�]�O�̊p�x���i�[����
            }

            Vector3 newRotation = oldRotation;

            newRotation.z = Mathf.SmoothDampAngle(movementItemParent.transform.localEulerAngles.z, itemRotation.z, ref currentVelocity, rotationTime, rotationSpeed * 1000);  //�X���[�Y�ɉ�]������

            if (Mathf.Abs(newRotation.z - itemRotation.z) < 0.1f || removeItemOnce)
            {
                //��]�̏I���̏���
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
