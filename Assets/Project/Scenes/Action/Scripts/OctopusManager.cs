using UnityEngine;
using System.Linq;

public class OctopusManager : MonoBehaviour
{
    [SerializeField] OctopusStats stats;

    Rigidbody rb;

    bool isGround = false;
    float chargeTime = 0f;
    bool isCharging = false;

    bool isSticking = false;
    bool isStickable = false;
    Vector3 stickPoint = Vector3.zero;
    Vector3 stickNormal = Vector3.zero;
    float stickAngularSpeed = 0.0f;

    Vector3 direction = Vector3.zero;
    Vector3 respawnPoint = Vector3.zero;
    Vector3[] suctions;

    void Start()
    {
        Application.targetFrameRate = 120;

        rb = GetComponent<Rigidbody>();
        BoxCollider box = GetComponent<BoxCollider>();

        Vector3 suctionCenter = box.center - new Vector3(0, box.size.y * 0.5f, 0);
        float suctionRadius = box.size.x / 2 * stats.SUCTION_RADIUS_RATE;
        suctions = Enumerable.Range(0, stats.SUCTION_COUNT).Select(i =>
        {
            float angle = i * Mathf.PI * 2f / stats.SUCTION_COUNT;
            float x = Mathf.Cos(angle) * suctionRadius;
            float z = Mathf.Sin(angle) * suctionRadius;

            return suctionCenter + new Vector3(x, 0, z);
        }).ToArray();
    }

    void Update()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Quaternion rotateBasis = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        direction = rotateBasis * new Vector3(inputH, 0, inputV);

        if (Input.GetMouseButtonDown(0))
        {
            chargeTime = 0f;
            isCharging = true;
        }

        if (Input.GetMouseButton(0))
        {
            if (isCharging)
            {
                chargeTime = Mathf.Min(chargeTime + Time.deltaTime, stats.MAX_CHARGE_TIME);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isGround || isSticking)
            {
                isSticking = false;
                rb.isKinematic = false;

                float jumpPower = Mathf.InverseLerp(0, stats.MAX_CHARGE_TIME, chargeTime) * stats.CHARGE_RATE;
                Vector3 jumpDirection = (Vector3.up + direction * stats.JUMP_MOVE_POWER).normalized;
                rb.AddForce(jumpDirection * jumpPower, ForceMode.Impulse);

                float spinPower = Mathf.Clamp01(direction.magnitude) * stats.JUMP_SPIN_POWER;
                Vector3 spinDirection = Vector3.Cross(Vector3.up, direction).normalized;
                rb.AddTorque(spinDirection * spinPower, ForceMode.Impulse);
            }

            isCharging = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (isStickable)
            {
                isSticking = true;
                rb.isKinematic = true;
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (respawnPoint != Vector3.zero)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                transform.position = respawnPoint;
            }
        }
    }

    void FixedUpdate()
    {
        if (direction != Vector3.zero)
        {
            if (!isGround)
            {
                rb.AddForce(direction.normalized * stats.AIR_MOVE_POWER, ForceMode.Acceleration);
            }

            if (rb.angularVelocity.magnitude < stats.MAX_SPEN_SPEED)
            {
                float spinPower = Mathf.Clamp01(direction.magnitude) * stats.SPIN_POWER;
                Vector3 spinDirection = Vector3.Cross(Vector3.up, direction).normalized;
                rb.AddTorque(spinDirection * spinPower, ForceMode.Acceleration);
            }
        }

        if (isSticking)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, stickNormal);
            Vector3 targetPosition = stickPoint + targetRotation * (transform.position - stickPoint);

            float minStickSpeed = 5f;
            float maxStickSpeed = 15f;
            float stickSpeed = Mathf.Lerp(
                minStickSpeed,
                maxStickSpeed,
                Mathf.InverseLerp(0f, 10f, stickAngularSpeed)
            );

            transform.SetPositionAndRotation(
                Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    Time.fixedDeltaTime * stickSpeed
                ),
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation * transform.rotation,
                    Time.fixedDeltaTime * stickSpeed
                )
            );
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGround = true;

        if (!isSticking)
        {
            int validRayCount = 0;
            Vector3 averageHitPoint = Vector3.zero;
            Vector3 averageHitNormal = Vector3.zero;

            foreach (Vector3 suction in suctions)
            {
                Vector3 suctionPos = transform.TransformPoint(suction);
                Ray suctionRay = new(suctionPos, -transform.up);

                if (Physics.Raycast(suctionRay, out RaycastHit hit, stats.SUCTION_RAY_LENGTH))
                {
                    if (Vector3.Angle(hit.normal, transform.up) <= stats.MAX_STICKABLE_ANGLE)
                    {
                        averageHitPoint += hit.point;
                        averageHitNormal += hit.normal;
                        validRayCount++;
                    }
                }
            }

            if (0 < validRayCount)
            {
                stickPoint = averageHitPoint / validRayCount;
                stickNormal = (averageHitNormal / validRayCount).normalized;
                stickAngularSpeed = rb.angularVelocity.magnitude;

                isStickable = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGround = false;
        isStickable = false;
    }

    public void SetRespawnPoint(Vector3 position)
    {
        respawnPoint = position;
    }
}
