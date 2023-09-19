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

    //�Ō�Ɏ����Ă����f�ނ�����ϐ�
    GameObject heldItem;

    //ChackInstallingToWorkbench���ł̂ݎg��
    HexInfomation judgeInfo;

    internal bool canDoFitting = false;

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

        ChackInstallingToWorkbench();
    }

    // �}�E�X���W���X�V���ꂽ���ɒʒm����R�[���o�b�N�֐�
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        worldMousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void OnHoldItem(InputAction.CallbackContext context)
    {
        mouseContext = context;
        if (isRotation) return;

        if (context.phase == InputActionPhase.Started)  //�f�ނ̏���
        {
            RaycastHit2D hitItem = Physics2D.Raycast(worldMousePosition, Vector2.zero);

            if (hitItem.collider != null && hitItem.collider.CompareTag("ItemObject_Piece"))
            {
                movementItem = hitItem.collider.gameObject;
                movementItemParent = movementItem.transform.parent.gameObject;

                movementItemChilds = movementItemParent.transform.GetComponentsInChildren<Transform>();  //index0 �ɐe�I�u�W�F�N�g������

                itemRotation = movementItemParent.transform.localEulerAngles;

                heldItem = movementItemParent;

                movementItemParent.GetComponent<ItemInfomation>().isFitting = false;
                for (int i = 1; i < movementItemChilds.Length; i++)
                {
                    HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();  //��Ƒ�ɂ͂܂��Ă��������m�F
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

    void ChackInstallingToWorkbench()  //��Ƒ�ɑf�ނ�u���邩���m�F����
    {
        if (movementItemParent == null) return;

        if (mouseContext.phase != InputActionPhase.Canceled)  //��Ƒ�̏���
        {
            int canFittingNum = 0;

            int mask = 1 << 7;  //Layer7��Workbench��ݒ�
            RaycastHit2D _hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
            if (_hitWorkbench.collider != null)
            {
                HexInfomation info_origin = _hitWorkbench.collider.GetComponent<HexInfomation>();

                for (int i = 1; i < workbenchChilds.Length; i++)
                {
                    HexInfomation info_w = workbenchChilds[i].GetComponent<HexInfomation>();

                    for (int j = 1; j < movementItemChilds.Length; j++)
                    {
                        HexInfomation info_i = movementItemChilds[j].GetComponent<HexInfomation>();  //�f�ނ̈ʒu�����擾����
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
                    //Debug.Log("�u���܂�");
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

    void PutOnWorkbench()  //��Ƒ�ɑf�ނ�u��
    {
        if (heldItem == null) return;
        if (movementItemParent == null) return;
        int mask = 1 << 7;  //Layer7��Workbench��ݒ�
        RaycastHit2D _hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
        if (!_hitWorkbench)
        {
            canDoFitting = false;
        }

        if (mouseContext.phase == InputActionPhase.Waiting && canDoFitting)  //�f�ނ𗣂�������̏���������
        {
            Vector3 fitPos = _hitWorkbench.transform.position;
            fitPos.z = movementItemParent.transform.position.z;
            movementItemParent.transform.position = fitPos;

            movementItemParent.GetComponent<ItemInfomation>().isFitting = true;

            HexInfomation info_origin = _hitWorkbench.collider.GetComponent<HexInfomation>();
            for (int i = 1; i < movementItemChilds.Length; i++)
            {
                HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();  //�܂��f�ނ̈ʒu�����擾����
                judgeInfo.q = info_origin.q + info_i.q;
                judgeInfo.r = info_origin.r + info_i.r;
                judgeInfo.s = info_origin.s + info_i.s;

                for (int j = 1; j < workbenchChilds.Length; j++)  //��Ƒ�̈ʒu���Ɣ�ׂ�
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
            if(!isRotation && !canDoFitting) movementItemParent = null;    //�f�ނ̉�]���I���܂�movement_Item_Parent��null�ɂ��Ȃ�
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
