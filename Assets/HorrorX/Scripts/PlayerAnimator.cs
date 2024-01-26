using Mirror;
using System;
using System.Diagnostics;
using UnityEngine;

namespace oldfpc{
[Serializable]
public class PlayerAnimator
{
    [Header("Animator")]
    public bool Enable = true;
    public Animator animator;

    //public Transform[] animatorObjects;

    public NetworkAnimator netAnimator;

    private Transform _fpsCameraHelper;
    public float HorzAnimation;
    public float VertAnimation;
    private bool JumpAnimation;
    private bool LandAnimation;

    //int layerMask = 1 << 15;

    [Header("Inputs Animator")]
    public string MoveAnimator = "Move";
    public string HorizontalAnimator = "Horizontal";
    public string VerticalAnimator = "Vertical";
    public string IsGroundedAnimator = "isGrounded";
    public string GroundAnimator = "Ground";
    public string JumpAnimator = "Jump";
    public string CrouchAnimator = "Crouch";
    public string RayUpAnimator = "Ray";
    public string RayDownAnimator = "Ray";
    public string IsJumpingAnimator = "isJumping";
    public string LeftTurn = "LeftTurn";
    public string RightTurn = "RightTurn";
    public string CameraIdle = "CameraIdle";


    public void Init(Animator _animator , NetworkAnimator net)
    {        
        animator = _animator;
        netAnimator = net;
        AddCameraPosition();
        //layerMask = ~layerMask;
    }

    void AddCameraPosition()
    {
        _fpsCameraHelper = new GameObject().transform;
        _fpsCameraHelper.name = "_fpsCameraHelper";
        _fpsCameraHelper.SetParent(animator.GetBoneTransform(HumanBodyBones.Head));
        _fpsCameraHelper.localPosition = Vector3.zero;
    }

    public void SetCameraPosition(MouseLook mouseLook, Transform camera)
    {
        _fpsCameraHelper.localPosition = mouseLook.FPS_CameraOffset;
        camera.transform.position = _fpsCameraHelper.position;
    }

    public void Animate(MouseLook mouseLook, PlayerMove playerMove, PlayerInput playerInput)
    {
        if (mouseLook._yRot == 0)
        {
            animator.SetBool(RightTurn, false);
            animator.SetBool(LeftTurn, false);
        }

        switch (playerMove.GetPlayerStates)
        {
            case PlayerMove.PlayerStates.Idle:               
                AnimateState(0,5,1f, playerInput);
                if(mouseLook._yRot > mouseLook._yRotClampAngle)
                {                  
                    animator.SetBool(RightTurn, true);
                }
                if (mouseLook._yRot < -mouseLook._yRotClampAngle)
                {                   
                    animator.SetBool(LeftTurn, true);
                }              
                break;

            case PlayerMove.PlayerStates.Walk:              
                AnimateState(1, 5, 0.8f, playerInput);
                break;             
            case PlayerMove.PlayerStates.Run:            
                AnimateState(2, 5, 0.8f, playerInput);
                break;
                
            case PlayerMove.PlayerStates.CrouchIdle:
                AnimateState(0, 5, 1f, playerInput);
                if (mouseLook._yRot > 10)
                {
                    animator.SetBool(RightTurn, true);
                }
                if (mouseLook._yRot < -10)
                {
                    animator.SetBool(LeftTurn, true);
                }
                break;

            case PlayerMove.PlayerStates.CrouchWalk:    
                AnimateState(1, 5, 1f, playerInput);
                break;

                
            case PlayerMove.PlayerStates.Jump:
                if (JumpAnimation)
                {
                    animator.SetTrigger(JumpAnimator);
                    netAnimator.SetTrigger(JumpAnimator);
                    animator.SetBool(IsGroundedAnimator, false);
                    JumpAnimation = false;
                }
                animator.speed = 1f;
                break;
                
        }

        

       
        

        /*
        if (FPC.m_Jumping)
        {
            CharacterAnimator.SetBool(IsJumpingAnimator, true);
        }
        else
        {
            CharacterAnimator.SetBool(IsJumpingAnimator, false);
        }
        */



        if (playerMove.isGrounded)
        {         
            if (playerInput.jump.IsPressed())
            {
                JumpAnimation = true;
            }          
            animator.SetBool(IsGroundedAnimator, true);

            if(playerMove.getIsCrouch)
                animator.SetBool(CrouchAnimator, true);
        }
        else
        {
            animator.SetBool(IsGroundedAnimator, false);
            animator.SetBool(CrouchAnimator, false);         
        }

        if(!playerMove.getIsCrouch)
            animator.SetBool(CrouchAnimator, false);

        if (playerInput.move.ReadValue<Vector2>().x != 0 || playerInput.move.ReadValue<Vector2>().y != 0)
        {
            animator.SetBool(MoveAnimator, true);
        }
        else
        {
            animator.SetBool(MoveAnimator, false);
        }

        if (playerMove.sphereIsGrounded)
        {
            animator.SetBool(GroundAnimator, true);
        }
        else
        {
            animator.SetBool(GroundAnimator, false);
        }

        if (playerMove.canMove) {
            animator.SetFloat(HorizontalAnimator, HorzAnimation);
            animator.SetFloat(VerticalAnimator, VertAnimation);
        }
        else
        {
            animator.SetFloat(HorizontalAnimator, 0);
            animator.SetFloat(VerticalAnimator, 0);
        }
    }
  
    void AnimateState(float speed, float speedAnim, float animatorSpeed, PlayerInput playerInput)
    {
        HorzAnimation = Mathf.Lerp(HorzAnimation, speed * playerInput.move.ReadValue<Vector2>().x, speedAnim * Time.deltaTime);
        VertAnimation = Mathf.Lerp(VertAnimation, speed * playerInput.move.ReadValue<Vector2>().y, speedAnim * Time.deltaTime);
        animator.speed = animatorSpeed;
    }

    
}
}