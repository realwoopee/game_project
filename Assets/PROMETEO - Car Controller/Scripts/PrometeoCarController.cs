/*
MESSAGE FROM CREATOR: This script was coded by Mena. You can use it in your games either these are commercial or
personal projects. You can even add or remove functions as you wish. However, you cannot sell copies of this
script by itself, since it is originally distributed as a free product.
I wish you the best for your project. Good luck!

P.S: If you need more cars, you can check my other vehicle assets on the Unity Asset Store, perhaps you could find
something useful for your game. Best regards, Mena.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{
    // IS VEHICLE ACTIVE
    //[Header("IS VEHICLE ACTIVE")]
    [SerializeField] public bool vehicleActive;

    public InputManager inputManager;

    //CAR SETUP

    //[Header("CAR SETUP")]
    [Range(20, 190)] public int maxSpeed = 90; //The maximum speed that the car can reach in km/h.

    [Range(10, 120)]
    public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.

    [Range(1, 10)] public int
        accelerationMultiplier = 2; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.

    [Range(10, 45)]
    public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.

    [Range(0.1f, 1f)] public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
    [Range(100, 600)] public int brakeForce = 350; // The strength of the wheel brakes.

    [Range(1, 10)]
    public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.

    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.

    public Vector3
        bodyMassCenter; // This is a vector that contains the center of mass of the car. I recommend to set this value
    // in the points x = 0 and z = 0 of your car. You can select the value that you want in the y axis,
    // however, you must notice that the higher this value is, the more unstable the car becomes.
    // Usually the y value goes from 0 to 1.5.

    //WHEELS

    //[Header("WHEELS")]

    /*
    The following variables are used to store the wheels' data of the car. We need both the mesh-only game objects and wheel
    collider components of the wheels. The wheel collider components and 3D meshes of the wheels cannot come from the same
    game object; they must be separate game objects.
    */
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;

    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;

    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;

    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    //PARTICLE SYSTEMS


    //[Header("EFFECTS")]

    //The following variable lets you to set up particle systems in your car
    public bool useEffects = false;

    // The following particle systems are used as tire smoke when the car drifts.
    [FormerlySerializedAs("RLWParticleSystem")] public ParticleSystem rlwParticleSystem;
    [FormerlySerializedAs("RRWParticleSystem")] public ParticleSystem rrwParticleSystem;


    // The following trail renderers are used as tire skids when the car loses traction.
    [FormerlySerializedAs("RLWTireSkid")] public TrailRenderer rlwTireSkid;
    [FormerlySerializedAs("RRWTireSkid")] public TrailRenderer rrwTireSkid;

    //SPEED TEXT (UI)


    //[Header("UI")]

    //The following variable lets you to set up a UI text to display the speed of your car.
    public bool useUI = false;
    public Text carSpeedText; // Used to store the UI object that is going to show the speed of the car.

    //SOUNDS


    //[Header("Sounds")]

    //The following variable lets you to set up sounds for your car such as the car engine or tire screech sounds.
    public bool useSounds = false;
    public AudioSource carEngineSound; // This variable stores the sound of the car engine.

    public AudioSource
        tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).

    private float _initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.

    //CONTROLS


    //[Header("CONTROLS")]

    //The following variables lets you to set up touch controls for mobile devices.
    public GameObject throttleButton;
    private PrometeoTouchInput _throttlePti;
    public GameObject reverseButton;
    private PrometeoTouchInput _reversePti;
    public GameObject turnRightButton;
    private PrometeoTouchInput _turnRightPti;
    public GameObject turnLeftButton;
    private PrometeoTouchInput _turnLeftPti;
    public GameObject handbrakeButton;
    private PrometeoTouchInput _handbrakePti;

    //CAR DATA

    [HideInInspector] public float carSpeed; // Used to store the speed of the car.
    [HideInInspector] public bool isDrifting; // Used to know whether the car is drifting or not.
    [HideInInspector] public bool isTractionLocked; // Used to know whether the traction of the car is locked or not.

    //PRIVATE VARIABLES

    /*
    IMPORTANT: The following variables should not be modified manually since their values are automatically given via script.
    */
    private Rigidbody _carRigidbody; // Stores the car's rigidbody.
    private float _steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
    private float _throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
    private float _driftingAxis;
    private float _localVelocityZ;
    private float _localVelocityX;
    private bool _deceleratingCar;

    private bool _touchControlsSetup = false;

    /*
    The following variables are used to store information about sideways friction of the wheels (such as
    extremumSlip,extremumValue, asymptoteSlip, asymptoteValue and stiffness). We change this values to
    make the car to start drifting.
    */
    private WheelFrictionCurve _fLwheelFriction;
    private float _flWextremumSlip;
    private WheelFrictionCurve _fRwheelFriction;
    private float _frWextremumSlip;
    private WheelFrictionCurve _rLwheelFriction;
    private float _rlWextremumSlip;
    private WheelFrictionCurve _rRwheelFriction;
    private float _rrWextremumSlip;

    // Start is called before the first frame update
    private void Start()
    {
        //In this part, we set the 'carRigidbody' value with the Rigidbody attached to this
        //gameObject. Also, we define the center of mass of the car with the Vector3 given
        //in the inspector.
        _carRigidbody = gameObject.GetComponent<Rigidbody>();
        _carRigidbody.centerOfMass = bodyMassCenter;

        //Initial setup to calculate the drift value of the car. This part could look a bit
        //complicated, but do not be afraid, the only thing we're doing here is to save the default
        //friction values of the car wheels so we can set an appropiate drifting value later.
        _fLwheelFriction = new WheelFrictionCurve();
        _fLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        _flWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        _fLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        _fLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        _fLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        _fLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
        _fRwheelFriction = new WheelFrictionCurve();
        _fRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        _frWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        _fRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        _fRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        _fRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        _fRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
        _rLwheelFriction = new WheelFrictionCurve();
        _rLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        _rlWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        _rLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        _rLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        _rLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        _rLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
        _rRwheelFriction = new WheelFrictionCurve();
        _rRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        _rrWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        _rRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        _rRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        _rRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        _rRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        // We save the initial pitch of the car engine sound.
        if (carEngineSound != null)
        {
            _initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        if (!useEffects)
        {
            if (rlwParticleSystem)
            {
                rlwParticleSystem.Stop();
            }

            if (rrwParticleSystem)
            {
                rrwParticleSystem.Stop();
            }

            if (rlwTireSkid)
            {
                rlwTireSkid.emitting = false;
            }

            if (rrwTireSkid)
            {
                rrwTireSkid.emitting = false;
            }
        }

        inputManager.OnHandbrakeReleased += RecoverTraction;
    }

    // Update is called once per frame
    private void Update()
    {
        //CAR DATA

        // We determine the speed of the car.
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
        _localVelocityX = transform.InverseTransformDirection(_carRigidbody.velocity).x;
        // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
        _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.velocity).z;

        //CAR PHYSICS

        /*
        The next part is regarding to the car controller. First, it checks if the user wants to use touch controls (for
        mobile devices) or analog input controls (WASD + Space).

        The following methods are called whenever a certain key is pressed. For example, in the first 'if' we call the
        method GoForward() if the user has pressed W.

        In this part of the code we specify what the car needs to do if the user presses W (throttle), S (reverse),
        A (turn left), D (turn right) or Space bar (handbrake).
        */

        if (vehicleActive)
        {
            if (inputManager.vehicleControl.y > 0)
            {
                CancelInvoke("DecelerateCar");
                _deceleratingCar = false;
                GoForward();
            }

            if (inputManager.vehicleControl.y < 0)
            {
                CancelInvoke("DecelerateCar");
                _deceleratingCar = false;
                GoReverse();
            }

            if (inputManager.vehicleControl.x < 0)
            {
                TurnLeft();
            }

            if (inputManager.vehicleControl.x > 0)
            {
                TurnRight();
            }

            if (inputManager.handbrakeHeld)
            {
                CancelInvoke("DecelerateCar");
                _deceleratingCar = false;
                Handbrake();
            }

            // if (Input.GetKeyUp(KeyCode.Space))
            // {
            //     RecoverTraction();
            // }

            if (inputManager.vehicleControl.y == 0)
            {
                ThrottleOff();
            }

            if (inputManager.vehicleControl.y == 0 && !inputManager.handbrakeHeld &&
                !_deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                _deceleratingCar = true;
            }

            if (inputManager.vehicleControl.x == 0 && _steeringAxis != 0f)
            {
                ResetSteeringAngle();
            }
        }

        CarSounds();
        CarSpeedUI();

        // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
        AnimateWheelMeshes();
    }

    // This method converts the car speed data from float to string, and then set the text of the UI carSpeedText with this value.
    public void CarSpeedUI()
    {
        if (useUI)
        {
            try
            {
                float absoluteCarSpeed = Mathf.Abs(carSpeed);
                carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
    }

    // This method controls the car sounds. For example, the car engine will sound slow when the car speed is low because the
    // pitch of the sound will be at its lowest point. On the other hand, it will sound fast when the car speed is high because
    // the pitch of the sound will be the sum of the initial pitch + the car speed divided by 100f.
    // Apart from that, the tireScreechSound will play whenever the car starts drifting or losing traction.
    public void CarSounds()
    {
        if (useSounds)
        {
            try
            {
                var engineSoundPitch =
                    _initialCarEngineSoundPitch + (Mathf.Abs(_carRigidbody.velocity.magnitude) / 25f);
                if (carEngineSound && vehicleActive)
                {
                    carEngineSound.pitch = engineSoundPitch;
                    if (!carEngineSound.isPlaying)
                        carEngineSound.Play();
                }
                if(!vehicleActive)
                    carEngineSound.Stop();

                if (tireScreechSound)
                {
                    if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                    {
                        if (!tireScreechSound.isPlaying)
                        {
                            tireScreechSound.Play();
                        }
                    }
                    else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                    {
                        tireScreechSound.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useSounds)
        {
            if (carEngineSound && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }

            if (tireScreechSound && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }
    }

    //
    //STEERING METHODS
    //

    //The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
    public void TurnLeft()
    {
        _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (_steeringAxis < -1f)
        {
            _steeringAxis = -1f;
        }

        var steeringAngle = _steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //The following method turns the front car wheels to the right. The speed of this movement will depend on the steeringSpeed variable.
    public void TurnRight()
    {
        _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (_steeringAxis > 1f)
        {
            _steeringAxis = 1f;
        }

        var steeringAngle = _steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
    // on the steeringSpeed variable.
    public void ResetSteeringAngle()
    {
        if (_steeringAxis < 0f)
        {
            _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (_steeringAxis > 0f)
        {
            _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }

        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            _steeringAxis = 0f;
        }

        var steeringAngle = _steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // This method matches both the position and rotation of the WheelColliders with the WheelMeshes.
    private void AnimateWheelMeshes()
    {
        try
        {
            Quaternion flwRotation;
            Vector3 flwPosition;
            frontLeftCollider.GetWorldPose(out flwPosition, out flwRotation);
            frontLeftMesh.transform.position = flwPosition;
            frontLeftMesh.transform.rotation = flwRotation;

            Quaternion frwRotation;
            Vector3 frwPosition;
            frontRightCollider.GetWorldPose(out frwPosition, out frwRotation);
            frontRightMesh.transform.position = frwPosition;
            frontRightMesh.transform.rotation = frwRotation;

            Quaternion rlwRotation;
            Vector3 rlwPosition;
            rearLeftCollider.GetWorldPose(out rlwPosition, out rlwRotation);
            rearLeftMesh.transform.position = rlwPosition;
            rearLeftMesh.transform.rotation = rlwRotation;

            Quaternion rrwRotation;
            Vector3 rrwPosition;
            rearRightCollider.GetWorldPose(out rrwPosition, out rrwRotation);
            rearRightMesh.transform.position = rrwPosition;
            rearRightMesh.transform.rotation = rrwRotation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    //
    //ENGINE AND BRAKING METHODS
    //

    // This method apply positive torque to the wheels in order to go forward.
    public void GoForward()
    {
        //If the forces aplied to the rigidbody in the 'x' asis are greater than
        //3f, it means that the car is losing traction, then the car will start emitting particle systems.
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // The following part sets the throttle power to 1 smoothly.
        _throttleAxis = _throttleAxis + (Time.deltaTime * 3f);
        if (_throttleAxis > 1f)
        {
            _throttleAxis = 1f;
        }

        //If the car is going backwards, then apply brakes in order to avoid strange
        //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
        //is safe to apply positive torque to go forward.
        if (_localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
            }
            else
            {
                // If the maxSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    // This method apply negative torque to the wheels in order to go backwards.
    public void GoReverse()
    {
        //If the forces aplied to the rigidbody in the 'x' asis are greater than
        //3f, it means that the car is losing traction, then the car will start emitting particle systems.
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // The following part sets the throttle power to -1 smoothly.
        _throttleAxis = _throttleAxis - (Time.deltaTime * 3f);
        if (_throttleAxis < -1f)
        {
            _throttleAxis = -1f;
        }

        //If the car is still going forward, then apply brakes in order to avoid strange
        //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
        //is safe to apply negative torque to go reverse.
        if (_localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
            }
            else
            {
                //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
    // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
    // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
    public void DecelerateCar()
    {
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        // The following part resets the throttle power to 0 smoothly.
        if (_throttleAxis != 0f)
        {
            if (_throttleAxis > 0f)
            {
                _throttleAxis = _throttleAxis - (Time.deltaTime * 10f);
            }
            else if (_throttleAxis < 0f)
            {
                _throttleAxis = _throttleAxis + (Time.deltaTime * 10f);
            }

            if (Mathf.Abs(_throttleAxis) < 0.15f)
            {
                _throttleAxis = 0f;
            }
        }

        _carRigidbody.velocity = _carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
        // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
        // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
        // also cancel the invoke of this method.
        if (_carRigidbody.velocity.magnitude < 0.25f)
        {
            _carRigidbody.velocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    // This function applies brake torque to the wheels according to the brake force given by the user.
    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
    // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
    // it is high, then you could make the car to feel like going on ice.
    public void Handbrake()
    {
        CancelInvoke("RecoverTraction");
        // We are going to start losing traction smoothly, there is were our 'driftingAxis' variable takes
        // place. This variable will start from 0 and will reach a top value of 1, which means that the maximum
        // drifting value has been reached. It will increase smoothly by using the variable Time.deltaTime.
        _driftingAxis = _driftingAxis + (Time.deltaTime);
        float secureStartingPoint = _driftingAxis * _flWextremumSlip * handbrakeDriftMultiplier;

        if (secureStartingPoint < _flWextremumSlip)
        {
            _driftingAxis = _flWextremumSlip / (_flWextremumSlip * handbrakeDriftMultiplier);
        }

        if (_driftingAxis > 1f)
        {
            _driftingAxis = 1f;
        }

        //If the forces aplied to the rigidbody in the 'x' asis are greater than
        //3f, it means that the car lost its traction, then the car will start emitting particle systems.
        if (Mathf.Abs(_localVelocityX) > 2.5f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        //If the 'driftingAxis' value is not 1f, it means that the wheels have not reach their maximum drifting
        //value, so, we are going to continue increasing the sideways friction of the wheels until driftingAxis
        // = 1f.
        if (_driftingAxis < 1f)
        {
            _fLwheelFriction.extremumSlip = _flWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontLeftCollider.sidewaysFriction = _fLwheelFriction;

            _fRwheelFriction.extremumSlip = _frWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontRightCollider.sidewaysFriction = _fRwheelFriction;

            _rLwheelFriction.extremumSlip = _rlWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearLeftCollider.sidewaysFriction = _rLwheelFriction;

            _rRwheelFriction.extremumSlip = _rrWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearRightCollider.sidewaysFriction = _rRwheelFriction;
        }

        // Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
        // and, as a consequense, the car starts to emit trails to simulate the wheel skids.
        isTractionLocked = true;
        DriftCarPS();
    }

    // This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
    // depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
    public void DriftCarPS()
    {
        if (useEffects)
        {
            try
            {
                if (isDrifting)
                {
                    rlwParticleSystem.Play();
                    rrwParticleSystem.Play();
                }
                else if (!isDrifting)
                {
                    rlwParticleSystem.Stop();
                    rrwParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((isTractionLocked || Mathf.Abs(_localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
                {
                    rlwTireSkid.emitting = true;
                    rrwTireSkid.emitting = true;
                }
                else
                {
                    rlwTireSkid.emitting = false;
                    rrwTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useEffects)
        {
            if (rlwParticleSystem != null)
            {
                rlwParticleSystem.Stop();
            }

            if (rrwParticleSystem != null)
            {
                rrwParticleSystem.Stop();
            }

            if (rlwTireSkid != null)
            {
                rlwTireSkid.emitting = false;
            }

            if (rrwTireSkid != null)
            {
                rrwTireSkid.emitting = false;
            }
        }
    }

    // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
    public void RecoverTraction()
    {
        isTractionLocked = false;
        _driftingAxis = _driftingAxis - (Time.deltaTime / 1.5f);
        if (_driftingAxis < 0f)
        {
            _driftingAxis = 0f;
        }

        //If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
        //We are going to continue decreasing the sideways friction of the wheels until we reach the initial
        // car's grip.
        if (_fLwheelFriction.extremumSlip > _flWextremumSlip)
        {
            _fLwheelFriction.extremumSlip = _flWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontLeftCollider.sidewaysFriction = _fLwheelFriction;

            _fRwheelFriction.extremumSlip = _frWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            frontRightCollider.sidewaysFriction = _fRwheelFriction;

            _rLwheelFriction.extremumSlip = _rlWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearLeftCollider.sidewaysFriction = _rLwheelFriction;

            _rRwheelFriction.extremumSlip = _rrWextremumSlip * handbrakeDriftMultiplier * _driftingAxis;
            rearRightCollider.sidewaysFriction = _rRwheelFriction;

            Invoke("RecoverTraction", Time.deltaTime);
        }
        else if (_fLwheelFriction.extremumSlip < _flWextremumSlip)
        {
            _fLwheelFriction.extremumSlip = _flWextremumSlip;
            frontLeftCollider.sidewaysFriction = _fLwheelFriction;

            _fRwheelFriction.extremumSlip = _frWextremumSlip;
            frontRightCollider.sidewaysFriction = _fRwheelFriction;

            _rLwheelFriction.extremumSlip = _rlWextremumSlip;
            rearLeftCollider.sidewaysFriction = _rLwheelFriction;

            _rRwheelFriction.extremumSlip = _rrWextremumSlip;
            rearRightCollider.sidewaysFriction = _rRwheelFriction;

            _driftingAxis = 0f;
        }
    }
}