using UnityEngine;

public class PlayerDrawGizmos : MonoBehaviour
{

    public float crouchUpRadius;
    public float crouchUpDistance;

    public float groundRadius;
    public float groundDistance;

    public float moveDistance;
    public Vector3 moveDirection;

    public LayerMask ignoreLayerMask;

    public Camera playerCamera;

    private void Awake()
    {
        ignoreLayerMask = ~ignoreLayerMask;
    }

    void OnDrawGizmos()
    {
        RaycastHit hitUp;
        if (Physics.SphereCast(transform.position, crouchUpRadius, transform.up, out hitUp, crouchUpDistance, ignoreLayerMask))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * crouchUpDistance);
            Gizmos.DrawWireSphere(transform.position + transform.up * crouchUpDistance, crouchUpRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * crouchUpDistance);
            Gizmos.DrawWireSphere(transform.position + transform.up * crouchUpDistance, crouchUpRadius);
        }

        RaycastHit hitDown;
        Vector3 offset = new Vector3(0, 0.5f, 0);
        if (Physics.SphereCast(transform.position + offset, groundRadius, - transform.up, out hitDown, groundDistance, ignoreLayerMask))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + offset, - transform.up * groundDistance);
            Gizmos.DrawWireSphere(transform.position + offset - transform.up * groundDistance, groundRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + offset, - transform.up * groundDistance);
            Gizmos.DrawWireSphere(transform.position + offset - transform.up * groundDistance, groundRadius);
        }

        RaycastHit hitDirection;
        Vector3 positonRay = new Vector3(transform.position.x, playerCamera.transform.position.y,transform.position.z);
        Vector3 moveDirectionRay = new Vector3(moveDirection.x,0,moveDirection.z);
        if(Physics.Raycast(positonRay, moveDirectionRay, out hitDirection, moveDistance, ignoreLayerMask))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(positonRay, moveDirectionRay * moveDistance);
        }
        else
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(positonRay, moveDirectionRay * moveDistance);
        }
    }
}
