using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGrab : MonoBehaviour
{
    FixedJoint2D fixedJoint;
    public GameObject grabLeg;
    public bool isGrounded = false;
    public LayerMask ground;
    public GameObject groundCheckPoint;
    // Start is called before the first frame update
    void Start()
    {
        fixedJoint = grabLeg.GetComponent<FixedJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.transform.position, 0.1f, ground);
        if (Input.GetMouseButton(0) && isGrounded)
        {
            fixedJoint.enabled = true;
        }
        else
            fixedJoint.enabled = false;
    }
}
