using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public Rigidbody sphere;

    public Transform carModel;
    public Transform carNormal;

    private float speed;
    private float currentSpeed;

    private float rotate;
    private float currentRotate;

    [Header("CarSettings")]
    [SerializeField] private float acceleration;
    [SerializeField] private float steering;
    [SerializeField] private float gravity;

    public LayerMask layerMask;

    public Transform frontWheels;
    public Transform rearWheels;

    private void Update()
    {
        GetInput();

        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        Debug.Log("speed = " + speed);
        Debug.Log("current speed = " + currentSpeed);
    }

    private void FixedUpdate()
    {
        Movement();

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        carNormal.up = Vector3.Lerp(carNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        carNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    private void Movement()
    {
        sphere.AddForce(carModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), 5.0f * Time.deltaTime);
    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }

    private void GetInput()
    {
        if(Input.GetAxis(VERTICAL) != 0)
        {
            speed = acceleration;
        }

        if(Input.GetAxis(HORIZONTAL) != 0)
        {
            int dir = Input.GetAxis(HORIZONTAL) > 0 ? 1 : -1;
            float amount = Mathf.Abs(Input.GetAxis(HORIZONTAL));
            Steer(dir, amount);
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, 12.0f * Time.deltaTime);
        speed = 0;
        currentRotate = Mathf.Lerp(currentRotate, rotate, 4.0f * Time.deltaTime);
        rotate = 0;
    }
}
