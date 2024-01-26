using System;
using UnityEngine;

namespace oldfpc
{
public class HeadIK : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject target;

    [SerializeField] private float targetPosition = 10f;
  
    [SerializeField, Range(0, 1)] private float m_IKWeight = 1f;
    [SerializeField, Range(0, 1)] private float m_BodyWeight = 0.1f;
    [SerializeField, Range(0, 1)] private float m_HeadWeight = 0.5f;
    [SerializeField, Range(0, 1)] private float m_EyesWeight = 1f;
    [SerializeField, Range(0, 1)] private float m_ClampWeight = 0.7f;

    private float _targetPosition;
    private bool notNullCamera;
    private bool notNullAnimator;

    public void Start()
    {
        _targetPosition = targetPosition;
        notNullCamera = GetCamera();
        notNullAnimator = GetAnimator();  
        if(notNullCamera) CreateIKTarget();
    }

    private bool GetCamera()
    {
        if(!m_Camera)
        m_Camera = GetComponent<FirstPersonController>()?.playerCamera ??  GetComponent<Camera>() ?? throw new NullReferenceException(nameof(m_Camera), null);
        return m_Camera ? true : false;
    }

    private bool GetAnimator()
    {
        if(!m_Animator)
        m_Animator = GetComponent<FirstPersonController>()?.animator  ?? GetComponent<Animator>() ?? throw new NullReferenceException(nameof(m_Animator), null);
        return m_Animator ? true : false;
    }

    private void CreateIKTarget()
    {
        target = new GameObject("HeadIKPosition");
        target.transform.SetParent(m_Camera.transform);
        SetTargetPosition();
    }

    private void Update()
    {
       if(targetPosition != _targetPosition)
       {
            SetTargetPosition();
            _targetPosition = targetPosition;
       }
    }

    private void SetTargetPosition()
    {
        target.transform.localPosition = new Vector3(0, 0, _targetPosition);
    }

    void OnAnimatorIK()
    {
        if (notNullCamera && notNullAnimator)
        {
            m_Animator.SetLookAtWeight(m_IKWeight, m_BodyWeight, m_HeadWeight, m_EyesWeight, m_ClampWeight);
            m_Animator.SetLookAtPosition(target.transform.position);
        }
    }
}
}