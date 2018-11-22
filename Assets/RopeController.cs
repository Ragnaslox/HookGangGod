using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class RopeController : MonoBehaviour

{
    private SpringJoint2D rope;
    private int ropeFrameCount;

    public GameObject ropeShooter;
    public GameObject anchorPoint;

    public int maxRopeFrameCount;
    public LineRenderer lineRenderer;
    public float jointFrequency = .3f;
    public float minDistance = 0.6f;
    public float damping = 0.5f;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        if (Input.GetButton("Jump"))
        {
            Retract();
        }
    }
    /*
     * thePlayer shoots the Visual rope but an anchor point outside of the player stops it from colliding with the hitbox of the player
     * also this way we have collision ingame which makes it more fun
     */
    private void LateUpdate()
    {
        if (rope != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, ropeShooter.transform.position);
            lineRenderer.SetPosition(1, rope.connectedAnchor);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (rope != null)
        {
            ropeFrameCount++;
            if (ropeFrameCount > maxRopeFrameCount)
            {
                GameObject.DestroyImmediate(rope);
                ropeFrameCount = 0;
            }
        }
    }

    /*Fires a Rope where the attached player wil swing on using a Springjoint2D
     first we get the mouseposition and the position of the ropecaster.
     the direction of the rope is the substract of the two before
     then we check if we hit a place where the rope can attach to
     and cast the rope*/
    private void Fire()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = anchorPoint.transform.position;
        Vector3 direction = mousePosition - position;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, Mathf.Infinity);

        if (hit.collider != null)
        {
            SpringJoint2D newRope = ropeShooter.AddComponent<SpringJoint2D>();
            newRope.enableCollision = false;
            newRope.autoConfigureDistance = true;
            newRope.enableCollision = true;
            newRope.frequency = jointFrequency; 
            newRope.connectedAnchor = hit.point;
            newRope.anchor = anchorPoint.transform.position;
            newRope.dampingRatio = damping;
            newRope.enabled = true;


            GameObject.DestroyImmediate(rope);
            rope = newRope;
            ropeFrameCount = 0;
        }
    }

    private void Retract()
    {
        if (rope != null&&rope.distance>minDistance)
        {
            rope.autoConfigureDistance = false;
           
            rope.distance -= 0.1f;
        }
    }
}