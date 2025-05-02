using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] Material checkedMat;
    [SerializeField] Material uncheckedMat;

    MeshRenderer mr;
    Material[] mats;
    ParticleSystem checkEffectPS;
    ParticleSystem.MainModule sparkEffectPSM;

    bool isChecked = false;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mats = mr.sharedMaterials;

        mats[1] = uncheckedMat;
        mr.sharedMaterials = mats;

        GameObject sparkEffect = transform.Find("SparkEffect").gameObject;
        sparkEffectPSM = sparkEffect.GetComponent<ParticleSystem>().main;
        sparkEffectPSM.startColor = uncheckedMat.color;

        GameObject checkEffect = transform.Find("CheckEffect").gameObject;
        checkEffectPS = checkEffect.GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isChecked && other.CompareTag("Player"))
        {
            mats[1] = checkedMat;
            mr.sharedMaterials = mats;

            checkEffectPS.Play();
            sparkEffectPSM.startColor = checkedMat.color;

            OctopusManager om = other.GetComponent<OctopusManager>();
            om.SetRespawnPoint(transform.position + Vector3.up * 3);

            isChecked = true;
        }
    }
}
