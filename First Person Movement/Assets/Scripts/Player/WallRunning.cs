using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;

    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("References")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovement pm;
    private LedgeGrabbing lg;
    private Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        lg = GetComponent<LedgeGrabbing>();
    }

    private void Update() {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate() {
        if (pm.wallrunning) 
            WallRunningMovement();
        
    }

    private void CheckForWall() {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround() {
        if (pm.OnSlope()) return false;
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }


    private void StateMachine() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // Wallrunning
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall) {
            if(!pm.wallrunning)
                StartWallRun();

            if(wallRunTimer > 0) 
                wallRunTimer -= Time.deltaTime;

            if(wallRunTimer <= 0) {
                exitingWall = true;
                if (wallLeft || wallRight)
                    exitWallTimer = exitWallTime;
            }

            // Walljumping
            if(Input.GetKeyDown(jumpKey))
                WallJump();
        } 
        // Exiting
        else if (exitingWall) {
            if(pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        // Nothing
        else {
            if(pm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun() {
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Camera effects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);
    }

    private void WallRunningMovement() {

        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude) 
            wallForward = -wallForward;

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Upwards / downwards movement
        if(upwardsRunning)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);       
        if(downwardsRunning)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);

        // Push to the wall
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            if (!pm.grounded && !(pm.OnSlope()))
                rb.AddForce(-wallNormal * 100, ForceMode.Force);

        // Weaken the gravity
        if(useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun() {
        pm.wallrunning = false;

        // Reset camera effects
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    private void WallJump() {
        if (lg.holding || lg.exitingLedge) return;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Resetting y velocity and adding force
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
