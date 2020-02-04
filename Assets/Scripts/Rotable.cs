using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotable : MonoBehaviour
{
    // grabbedBy
    protected OVRGrabbable grabbable;

    public float requiredAngle = 360f;
    public float currentAngle = 0f;
    public bool ended = false;
    public EnigmSolver solver;
    public AudioSource validateSound;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        grabbable.blockRotationOverride = true;
    }

    bool isGrabbed = false;
    Vector3 initialGrabberPosition;
    Vector2 initialDir;

    void FixedUpdate() {
        if (grabbable.grabbedBy != null && !isGrabbed) {
            isGrabbed = true;
            initialGrabberPosition = grabbable.grabbedBy.transform.position;
            Vector3 dir = grabbable.grabbedBy.transform.position - transform.position;
            initialDir = new Vector2(dir.z, dir.y);
        }

        if (isGrabbed && grabbable.grabbedBy == null) {
            isGrabbed = false;
        }

        if (isGrabbed && grabbable.grabbedBy != null) {

            Vector3 dir = grabbable.grabbedBy.transform.position - transform.position;
            Vector2 newDir = new Vector2(dir.z, dir.y);
            float finalAngle = Vector2.SignedAngle(initialDir, newDir);

            this.transform.Rotate(finalAngle, 0, 0);
            currentAngle += finalAngle;
            initialDir = newDir;

            if (currentAngle >= requiredAngle && !ended) {
                Solved();
            }
        }
        
    }
    void Solved() {
        ended = true;
        solver.OnPartSolved();

        if (validateSound != null) {
            validateSound.Play();
        }
    }
}
