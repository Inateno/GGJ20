using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerScript : MonoBehaviour
{
    public float maxLinearMove = 1f;
    // Start is called before the first frame update

    private float startPosZ;

    void Start()
    {
        this.startPosZ = this.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 p = this.transform.localPosition;

        if(p.z - this.startPosZ > maxLinearMove) {
            p.z = this.startPosZ + maxLinearMove;
        } else if (p.z - this.startPosZ < 0) {
            p.z = this.startPosZ;
        }

        this.transform.localPosition = p;
    }
}
