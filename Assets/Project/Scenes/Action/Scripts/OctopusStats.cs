using UnityEngine;

[CreateAssetMenu(fileName = "Octopus", menuName = "Octopus/Stats")]
public class OctopusStats : ScriptableObject
{
    [Header("チャージ")]
    public float CHARGE_RATE = 6f;
    public float MAX_CHARGE_TIME = 0.5f;

    [Header("ジャンプ")]
    public float JUMP_MOVE_POWER = 0.5f;
    public float JUMP_SPIN_POWER = 1f;

    [Header("移動")]
    public float AIR_MOVE_POWER = 5;

    [Header("回転")]
    public float SPIN_POWER = 15f;
    public float MAX_SPEN_SPEED = 3f;

    [Header("張り付き")]
    public int SUCTION_COUNT = 8;
    public float SUCTION_RADIUS_RATE = 0.85f;
    public float SUCTION_RAY_LENGTH = 0.1f;
    public float MAX_STICKABLE_ANGLE = 30f;
}
