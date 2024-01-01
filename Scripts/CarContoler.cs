using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarContoler : MonoBehaviour
{
    public Rigidbody theRB;
    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4f;

    private float speedInput;

    public float turnStrength = 180f;

    private float turnInput;

    public bool grounded;

    public Transform groundRayPoint, groundRayPoint2;

    public LayerMask whatIsGround;

    public float groundRayLength = 0.75f;


    private float dragOnGround;
    public float gravityMod = 10f;

    public Transform leftFrontWheel, rightFrontWheel;

    public float maxWheelTurn = 25f;

    public AudioSource engineSound;

    public AudioSource skid;

    public float skidSpeed;

    public int nextCheckpoint;

    public int current;

    public float lapTime, bestLaptime;

    public bool isAI;
    public int currentTarget;

    private Vector3 targetPoint;

    public float aiAccelerateSpeed = 1f, aiTurnSpeed = .8f, aiReachPointRange = 5f, aiPointVariance = 3f, aiMaxTurn = 15f;

    private float aiSpeedInput, mod;




    // Start is called before the first frame update
    void Start()
    {
        theRB.transform.parent = null;
        dragOnGround = theRB.drag;

        if(isAI) {
            targetPoint = Race.instance.allcheckPoints[currentTarget].transform.position;
            RandomisAITarget();

            mod = Random.Range(.8f, 1.1f);
        }

        UIManager.instance.LapCounterText.text = current + "/" + Race.instance.totalLaps;

       
    }

    // Update is called once per frame
    void Update()
    {
        if(!Race.instance.isStart)
        {

        
        lapTime += Time.deltaTime;

        if(!isAI)
        {

        
        var ts = System.TimeSpan.FromSeconds(lapTime);
        UIManager.instance.currentLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s",ts.Minutes,ts.Seconds,ts.Milliseconds);
       
        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel;

        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }

        turnInput = Input.GetAxis("Horizontal");


            /* if (grounded && Input.GetAxis("Vertical") != 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));


            } */


        } else
        {
            targetPoint.y = transform.position.y;
            if(Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                setNext();
            }

            Vector3 targetDire = targetPoint - transform.position;

            float angle = Vector3.Angle(targetDire, transform.forward);
            Vector3 localPos = transform.InverseTransformPoint(targetPoint);

            if(localPos.x < 0f)
            {
                angle = -angle;

            }

            turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

            if(Mathf.Abs(angle) < aiMaxTurn)
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
            } else
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
            }


            speedInput = aiSpeedInput * forwardAccel * mod;
        }


        //turning wheels

        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);


        // transform.position = theRB.position;
       
        if(engineSound != null)
        {
            engineSound.pitch = 1f + ((theRB.velocity.magnitude / maxSpeed) * 2f);

        }
        if(skid != null)
        {
            if(Mathf.Abs(turnInput) > 0.5f)
            {
                skid.volume = 1f;

            } else
            {
                skid.volume = Mathf.MoveTowards(skid.volume, 0f, skidSpeed * Time.deltaTime);

            }
        }
        }

    }


    private void FixedUpdate()
    {


        grounded = false;

        RaycastHit hit;

        Vector3 normalTarget = Vector3.zero;



        if(Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = hit.normal;


        }

        if(Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = (normalTarget + hit.normal) / 2f;

        }

        if(grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        //acceleraiton

        if (grounded)
        {
            theRB.drag = dragOnGround;
            theRB.AddForce(transform.forward * speedInput * 1000f);
        } else
        {
            theRB.drag = .1f;

            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }

        if (theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;

        }
        Debug.Log(theRB.velocity.magnitude);

        transform.position = theRB.position;

        if (grounded && speedInput != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));


        }
    }
    public void CheckpointHit(int cpNumber)
    {
        //Debug.Log(cpNumber);
        if(cpNumber == nextCheckpoint)
        {
            nextCheckpoint++;


            if(nextCheckpoint == Race.instance.allcheckPoints.Length)
            {
                nextCheckpoint = 0;
                LapCompleted();


            }
        }
        if (isAI)
        {
            if(cpNumber == currentTarget)
            {
                setNext();
            }
        }
    }
    public void setNext()
    {
        currentTarget++;
        if (currentTarget >= Race.instance.allcheckPoints.Length)
        {
            currentTarget = 0;
        }
        targetPoint = Race.instance.allcheckPoints[currentTarget].transform.position;
        RandomisAITarget();
    }
    public void LapCompleted()
    {
        current++;
        if(lapTime < bestLaptime || bestLaptime == 0)
        {
            bestLaptime = lapTime;

        }
        lapTime = 0f;
        if(!isAI)
        {

        

        var ts = System.TimeSpan.FromSeconds(bestLaptime);
        UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2.000}s", ts.Minutes, ts.Seconds, ts.Minutes, ts.Milliseconds);

        UIManager.instance.LapCounterText.text = current + "/" + Race.instance.totalLaps;
        }
    }

    public void RandomisAITarget()
    {
        targetPoint = targetPoint + new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }
}

