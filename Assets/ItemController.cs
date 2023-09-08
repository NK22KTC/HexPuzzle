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
    private bool isRotation = false, removeItemOnce = false;

    enum RotationMode { Left, Right };
    RotationMode rotationMode;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        //Time.timeScale = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        HoldingItemControl();

        SmoothlyRotate();
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
    }

    public void OnRotateItem(InputAction.CallbackContext context)
    {
        if (movement_Item_Parent == null) return;
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
        if (movement_Item_Parent == null) return;
        if (removeItemOnce) return;

        if (mouseContext.phase == InputActionPhase.Started || mouseContext.phase == InputActionPhase.Performed)
        {
            worldMousePosition.z = 0;
            movement_Item_Parent.transform.position = worldMousePosition;
        }
        else
        {
            movement_Item = null;
            if(!isRotation) movement_Item_Parent = null;    //�f�ނ̉�]���I���܂�movement_Item_Parent��null�ɂ��Ȃ�
        }
    }

    Vector3 oldRotation;
    private float currentVelocity;
    void SmoothlyRotate()
    {
        if (movement_Item_Parent == null) return;  //�f�ނ�����ł��Ȃ��Ƃ�movement_Item_Parent��null�ɂȂ�

        if (mouseContext.phase == InputActionPhase.Waiting)  //�f�ނ�b�����u��
        {
            removeItemOnce = true;
        }

        if (movement_Item_Parent.transform.localEulerAngles != itemRotation)
        {
            if (!isRotation) oldRotation = movement_Item_Parent.transform.localEulerAngles;  //�ŏ���1��AoldRotation�ɉ�]�O�̊p�x���i�[����

            if (itemRotation.z == -60 && rotationMode == RotationMode.Right)
            {
                itemRotation.z += 360;
            }
            if (itemRotation.z == 360 && rotationMode == RotationMode.Left)
            {
                itemRotation.z -= 360;
            }

            Vector3 newRotation = oldRotation;

            newRotation.z = Mathf.SmoothDampAngle(movement_Item_Parent.transform.localEulerAngles.z, itemRotation.z, ref currentVelocity, rotationTime, 1000f);  //�X���[�Y�ɉ�]������

            if (Mathf.Abs(newRotation.z - itemRotation.z) < 0.1f)
            {
                //��]�̏I���̏���
                movement_Item_Parent.transform.localEulerAngles = itemRotation;
                isRotation = false;
                return;
            }

            movement_Item_Parent.transform.localEulerAngles = newRotation;
            rotationTime += Time.deltaTime;
            isRotation = true;
        }
        else if(rotationTime != 0)
        {
            rotationTime = 0;
            isRotation = false;
            if(removeItemOnce)
            {
                movement_Item_Parent = null;
            }
            removeItemOnce = false;
        }
    }
}
