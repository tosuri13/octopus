using UnityEngine;

public class OctopusManager : MonoBehaviour
{
    [SerializeField] OctopusStats stats;

    Rigidbody _rigidbody;

    float _chargeStartTime = 0.0f;
    bool _isGround = false;
    bool _isStickable = false;
    bool _isSticking = false;
    Vector3 _stickNormal = Vector3.zero;
    Vector3 _stickPoint = Vector3.zero;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Quaternion rotateBasis = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        Vector3 direction = rotateBasis * new Vector3(h, 0, v).normalized;

        if (Input.GetMouseButtonDown(0))
        {
            _chargeStartTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_isGround || _isSticking)
            {
                float chargeTime = Time.time - _chargeStartTime;
                float jumpPower = Mathf.Clamp(chargeTime * stats.chargeRate, stats.minJumpPower, stats.maxJumpPower);

                Vector3 velocity = (Vector3.up + direction * stats.jumpMovePower) * jumpPower;
                Vector3 spinAxis = Vector3.Cross(Vector3.up, direction);

                _isSticking = false;
                _rigidbody.isKinematic = false;

                _rigidbody.AddForce(velocity, ForceMode.Acceleration);
                _rigidbody.AddTorque(spinAxis * stats.jumpSpinPower, ForceMode.Acceleration);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (_isStickable)
            {
                _isSticking = true;
                _rigidbody.isKinematic = true;
            }
        }

        if (direction != Vector3.zero)
        {
            if (!_isGround)
            {
                _rigidbody.AddForce(direction * stats.movePower, ForceMode.Acceleration);
            }

            if (_rigidbody.angularVelocity.magnitude < stats.maxSpinSpeed)
            {
                Vector3 spinAxis = Vector3.Cross(Vector3.up, direction);
                _rigidbody.AddTorque(spinAxis * stats.spinPower, ForceMode.Acceleration);
            }
        }

        if (_isSticking)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _stickNormal);

            Vector3 pivot = _stickPoint;
            Vector3 targetPosition = pivot + targetRotation * (transform.position - pivot);

            transform.SetPositionAndRotation(
                Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    Time.deltaTime * 50f
                ),
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation * transform.rotation,
                    Time.deltaTime * 50f
                )
            );
        }
    }

    void OnCollisionStay(Collision collision)
    {
        _isGround = true;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(transform.up, contact.normal) < stats.maxStickableAngle)
            {
                _isStickable = true;
                _stickNormal = contact.normal;
                _stickPoint = contact.point;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        _isGround = false;
        _isStickable = false;
    }
}
