using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerCollisionSphere : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    SphereCollider sphereCollider;
    public Collision collision;

    public ContactPoint cp;

    public delegate void OnLeftGroundDelegate();
    public OnLeftGroundDelegate OnLeftGround;

    public delegate void OnBecomeGroundedDelegate();
    public OnBecomeGroundedDelegate OnBecomeGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if (collision == null)
            return;

        cp = collision.GetContact(0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnBecomeGrounded?.Invoke();
        Debug.Log("Collided with: " + collision.gameObject);
        this.collision = collision;
    }

    private void OnCollisionExit(Collision collision)
    {
        OnLeftGround?.Invoke();
        Debug.Log("OnCollisionExit");
        this.collision = null;
    }
}
