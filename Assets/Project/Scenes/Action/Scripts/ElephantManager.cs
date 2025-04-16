using UnityEngine;

public class ElephantManager : MonoBehaviour
{
    // TODO: 後でScriptable Objectに修正したい
    private readonly float MIN_JUMP_POWER = 100f;
    private readonly float MAX_JUMP_POWER = 300f;
    private readonly float CHARGE_RATE = 600f;

    private readonly float GROUND_MOVE_POWER = 0.5f;
    private readonly float AIR_MOVE_POWER = 0.2f;

    private readonly float GROUND_SPIN_POWER = 100f;
    private readonly float AIR_MAX_SPIN_SPEED = 3f;
    private readonly float AIR_SPIN_POSER = 1f;

    private Rigidbody _rigidbody;
    private float _chargeStartTime = 0.0f;
    private bool _isGround = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;

        if (_isGround)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _chargeStartTime = Time.time;
            }

            if (Input.GetMouseButtonUp(0))
            {
                float chargeTime = Time.time - _chargeStartTime;
                float jumpPower = Mathf.Clamp(chargeTime * CHARGE_RATE, MIN_JUMP_POWER, MAX_JUMP_POWER);

                // Debug.Log(jumpPower);

                _rigidbody.AddForce((Vector3.up + direction * GROUND_MOVE_POWER) * jumpPower, ForceMode.Acceleration);

                if (direction != Vector3.zero)
                {
                    Vector3 spinAxis = Vector3.Cross(Vector3.up, direction);
                    _rigidbody.AddTorque(spinAxis * GROUND_SPIN_POWER, ForceMode.Acceleration);
                }
            }
        }

        if (!_isGround)
        {
            if (direction != Vector3.zero)
            {
                _rigidbody.AddForce(direction * AIR_MOVE_POWER, ForceMode.Acceleration);
            }
        }

        // Debug.Log(_rigidbody.angularVelocity.magnitude);
        if (_rigidbody.angularVelocity.magnitude < AIR_MAX_SPIN_SPEED)
        {
            Vector3 spinAxis = Vector3.Cross(Vector3.up, direction);
            _rigidbody.AddTorque(spinAxis * AIR_SPIN_POSER, ForceMode.Acceleration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        _isGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        _isGround = false;
    }
}
