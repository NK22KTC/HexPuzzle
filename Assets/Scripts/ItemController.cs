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

        if (mouseContext.phase != InputActionPhase.Canceled)  //��Ƒ�̏���
        {
            int mask = 1 << 7;  //Layer7��Workbench��ݒ�
            RaycastHit2D hitWorkbench = Physics2D.Raycast(worldMousePosition, Vector2.zero, Camera.main.farClipPlane, mask);
            if (hitWorkbench.collider != null)
            {
                HexInfomation info_origin = hitWorkbench.collider.GetComponent<HexInfomation>();
                Debug.Log("R : " + info_origin.r + ", S : " + info_origin.s + ", Q : " + info_origin.q);

                for(int i = 1;  i < info_origin.r; i++)
                {
                    HexInfomation info_i = movementItemChilds[i].GetComponent<HexInfomation>();  //�܂��f�ނ̈ʒu�����擾����
                    Debug.Log("Q : " + info_i.q + ", R : " + info_i.r + ", S : " + info_i.s);

                    HexInfomation judgeInfo = new HexInfomation();
                    judgeInfo.q = info_origin.q + info_i.q;
                    judgeInfo.r = info_origin.r + info_i.r;
                    judgeInfo.s = info_origin.s + info_i.s;
                    Debug.Log("Q : " + judgeInfo.q + ", R : " + judgeInfo.r + ", S : " + judgeInfo.s);

                    foreach (GameObject workbenchChild in workbenchChilds)  //��Ƒ�̈ʒu���Ɣ�ׂ�
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
                Debug.Log(canFittingNum + "��");
                if(canFittingNum == movementItemChilds.Length)
                {
                    Debug.Log("�u���܂�");
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
                rotationMode = RotationMode.Left;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                //���v���ɉ�]������
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
            if(!isRotation) movementItemParent = null;    //�f�ނ̉�]���I���܂�movement_Item_Parent��null�ɂ��Ȃ�
        }

        //GameObject[] ItemObjects = FindObjectsOfType<GameObject>();  //�f�ނ̏d�Ȃ�����낢�낷��
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
        if (movementItemParent == null) return;  //�f�ނ�����ł��Ȃ��Ƃ�movement_Item_Parent��null�ɂȂ�

        if (mouseContext.phase == InputActionPhase.Waiting)  //�f�ނ�b�����u��
        {
            removeItemOnce = true;
        }

        if (movementItemParent.transform.localEulerAngles != itemRotation)
        {
            if (!isRotation) oldRotation = movementItemParent.transform.localEulerAngles;  //�ŏ���1��AoldRotation�ɉ�]�O�̊p�x���i�[����

            if (itemRotation.z == -60 && rotationMode == RotationMode.Right)
            {
                itemRotation.z += 360;
            }
            if (itemRotation.z == 360 && rotationMode == RotationMode.Left)
            {
                itemRotation.z -= 360;
            }

            Vector3 newRotation = oldRotation;

            newRotation.z = Mathf.SmoothDampAngle(movementItemParent.transform.localEulerAngles.z, itemRotation.z, ref currentVelocity, rotationTime, rotationSpeed * 1000);  //�X���[�Y�ɉ�]������

            if (Mathf.Abs(newRotation.z - itemRotation.z) < 0.1f)
            {
                //��]�̏I���̏���
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
