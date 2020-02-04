using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReceiver : MonoBehaviour
{
    public string targetObjectID = "none";
    public GameObject holdPosition;
    public Vector3 applyRotation = new Vector3(0,0,0);
    public GameObject replaceBy;
    private Rigidbody itemRigidbody;
    public bool isComplete = false;
    public AudioSource validateSound;

    void Start()
    {
        if (replaceBy != null) {
            replaceBy.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        ItemToPlace itemScript = other.GetComponent<ItemToPlace>();
        if (itemScript != null && itemScript.objectID == targetObjectID) {
            itemRigidbody = other.GetComponent<Rigidbody>();
            itemRigidbody.rotation = holdPosition.transform.rotation;
            CapsuleCollider capCol = itemRigidbody.GetComponent<CapsuleCollider>();
            MeshCollider meshCol = itemRigidbody.GetComponent<MeshCollider>();
            BoxCollider boxCol = itemRigidbody.GetComponent<BoxCollider>();

            if (capCol != null) {
                capCol.enabled = false;
            }
            if (meshCol != null) {
                meshCol.enabled = false;
            }
            if (boxCol != null) {
                boxCol.enabled = false;
            }

            OVRGrabbable grabScript = other.GetComponent<OVRGrabbable>();

            if (grabScript != null && grabScript.grabbedBy != null) {
                grabScript.grabbedBy.ForceRelease(grabScript);
            }

            itemRigidbody.useGravity = false;
            // if (freezeAfter) {
            //     itemRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            //     // if (grabScript != null) {
            //     //     grabScript.blockRotationOverride = true;
            //     // }
            //     Rotable rotScript = other.GetComponent<Rotable>();
            //     if (rotScript != null) {
            //         rotScript.enabled = true;
            //     }
            // }
            if (replaceBy != null) {
                replaceBy.SetActive(true);
                other.gameObject.SetActive(false);
            }
            isComplete = true;

            if (validateSound != null) {
                validateSound.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        if (itemRigidbody)
        {
            // Rotate the object to remain consistent with any changes in player's rotation
            holdPosition.transform.Rotate(applyRotation);
            itemRigidbody.rotation = holdPosition.transform.rotation;

            // Find vector from current position to destination
            Vector3 toDestination = holdPosition.transform.position - itemRigidbody.transform.position;

            // Calculate force
            Vector3 force = toDestination / Time.fixedDeltaTime;

            // Remove any existing velocity and add force to move to final position
            itemRigidbody.velocity = Vector3.zero;
            itemRigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
}
