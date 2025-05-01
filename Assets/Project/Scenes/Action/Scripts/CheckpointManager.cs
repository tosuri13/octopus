using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] Material checkedMat;
    [SerializeField] Material uncheckedMat;

    MeshRenderer mr;
    Material[] mats;

    bool isChecked = false;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mats = mr.sharedMaterials;

        mats[1] = uncheckedMat;
        mr.sharedMaterials = mats;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isChecked && other.CompareTag("Player"))
        {
            mats[1] = checkedMat;
            mr.sharedMaterials = mats;

            OctopusManager om = other.GetComponent<OctopusManager>();
            om.SetRespawnPoint(transform.position + Vector3.up * 3);

            isChecked = true;
        }
    }
}
