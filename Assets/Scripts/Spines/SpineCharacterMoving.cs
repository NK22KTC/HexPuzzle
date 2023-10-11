using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;

public class SpineCharacterMoving : MonoBehaviour
{
    Button button;
    public enum CharacterState { EnterStore, Wait, ExitStore };
    public CharacterState characterState = CharacterState.EnterStore;

    public float moveSpeed = 3f;
    public bool finishMixing = false;

    [SerializeField]
    private string animName = "idol";  //再生するアニメーションの名前
    private SkeletonAnimation skeletonAnimation = default;  //ゲームオブジェクトに設定されているSkeletonAnimation
    private Spine.AnimationState spineAnimationState = default;  //Spineアニメーションを適用するために必要なAnimationState

    // Start is called before the first frame update
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.AnimationState;

        button = FindObjectOfType<Button>();
        button.onClick.AddListener(() => OnChangeState());
        FindObjectOfType<Button>().onClick.AddListener(() => GameManager.instance.InstantiateCharacter());

        PlayAnimation("walk");
    }

    public void PlayAnimation(string animName)
    {
        // アニメーション「testAnimationName」を再生
        spineAnimationState.SetAnimation(0, animName, true);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeCharacterMove();
    }

    public void OnChangeState()
    {
        characterState = CharacterState.ExitStore;
        PlayAnimation("walk");
        button.onClick.RemoveAllListeners();
    }

    void ChangeCharacterMove()
    {
        if(characterState == CharacterState.EnterStore)
        {
            if(transform.position.x < 0)
            {
                characterState = CharacterState.Wait;
                PlayAnimation("idol");
            }

            Vector3 pos = transform.position;
            pos.x -= Time.deltaTime * moveSpeed;
            transform.position = pos;
        }
        else if(characterState == CharacterState.Wait)
        {
            if (finishMixing)
            {
                characterState = CharacterState.ExitStore;
            }
        }
        else
        {
            Vector3 pos = transform.position;
            pos.x -= Time.deltaTime * moveSpeed;
            transform.position = pos;

            if(pos.x < -16.5f)
            {
                Destroy(gameObject);
            }
        }
    }
}
