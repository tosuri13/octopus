using UnityEngine;

[CreateAssetMenu(fileName = "Octopus", menuName = "Octopus/Stats")]
public class OctopusStats : ScriptableObject
{
    [Header("ジャンプ")]
    public float minJumpPower = 100f;
    public float maxJumpPower = 300f;
    public float chargeRate = 600f;

    [Header("移動")]
    public float movePower = 0.2f;
    public float jumpMovePower = 0.5f;

    [Header("回転")]
    public float spinPower = 1f;
    public float jumpSpinPower = 300f;
    public float maxSpinSpeed = 3f;

    [Header("張り付き")]
    public float maxStickableAngle = 30f;
}
