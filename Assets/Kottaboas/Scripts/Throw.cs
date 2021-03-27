using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the way liquad drips are thrown and animation
public class Throw : MonoBehaviour
{
    private Vector3 pointToTravel;
    private Rigidbody rb;
    private ThrowRadius tr;

    public Animator animate;

    //Controls how far the liquad drips will be thrown
    public float power = 1.0f;

    private bool launch = false;
    public bool Launch { get => launch; set => launch = value; }
    public Rigidbody Rb { get => rb; set => rb = value; }

    // Start is called before the first frame update
    void Start()
    {
        tr = gameObject.GetComponent<ThrowRadius>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        pointToTravel = new Vector3(tr.trajectory.GetPosition(0).x, tr.trajectory.GetPosition(0).y, tr.trajectory.GetPosition(0).z);
        power = Mathf.Clamp(power, 0.5f, 2.0f);
        pointToTravel = (pointToTravel * power);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            launch = true;
            animate.SetBool("isFlinged", true);

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.CompareTag("TragjectSystem"))
                    transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            power -= 0.1f;
        }else if (Input.GetKeyUp(KeyCode.E))
        {
            power += 0.1f;
        }
    }

    private void FixedUpdate()
    {
        if (launch)
        {
            rb.useGravity = true;
            rb.MovePosition(transform.position + pointToTravel * Time.fixedDeltaTime);
        }
    }
}
