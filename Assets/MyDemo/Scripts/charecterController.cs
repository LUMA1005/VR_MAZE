using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charecterController : MonoBehaviour
{
    public float velocity;
    private Rigidbody rigidBody;
    private void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        float horizen = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 position= transform.position;
        transform.position = position + transform.forward * velocity * vertical*Time.deltaTime + transform.right * velocity * horizen*Time.deltaTime;
        if(Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddForce(Vector3.up * 1);
        }
    }
}
