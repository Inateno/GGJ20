using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetiteFille : MonoBehaviour
{
    public AudioSource laugh;
    public Camera occulusCamera;
    Collider petiteFilleCollider;
    Transform petiteFilleTransform;
    Transform[] spawnsList;
    Transform lastSpawn;
    bool hasBeenSeen;

    public Transform target1;
    public Transform target2;
    public Transform target3;
    public Transform target4;
    public Transform target5;

    float timeSinceLastMove = 0;
    public float moveInterval = 3000f;

    void Start()
    {
        laugh = GetComponent<AudioSource>();

        petiteFilleCollider = GetComponent<Collider>();
        petiteFilleTransform = GetComponent<Transform>();

        spawnsList = new Transform[5];
        spawnsList[0] = target1;
        spawnsList[1] = target2;
        spawnsList[2] = target3;
        spawnsList[3] = target4;
        spawnsList[4] = target5;

        hasBeenSeen = false;

        Transform t = spawnsList[Random.Range(0, spawnsList.Length-1)];
        petiteFilleTransform.position = t.position;
        petiteFilleTransform.rotation = t.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastMove += Time.deltaTime * 1000;

        // Debug.Log("IsSeenByCamera Value : " + IsSeenByCamera(petiteFilleTransform.position));
        
        if (IsSeenByCamera(petiteFilleTransform.position))
        {
            hasBeenSeen = true;
        }

        if (!IsSeenByCamera(petiteFilleTransform.position) && hasBeenSeen)
        {
            MoveLittleGirl();
        }
        
    }

    bool IsSeenByCamera(Vector3 girl)
    {
        Vector3 viewPos = occulusCamera.WorldToViewportPoint(girl);

        if (viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1)
        {
            return true;
        }

        return false;
    }

    bool IsInCameraViewFrustum()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(occulusCamera);
        return GeometryUtility.TestPlanesAABB(planes, petiteFilleCollider.bounds);
    }

    void MoveLittleGirl()
    {
        Debug.Log("Moving little GIRL !!");

        int randomSpawn = Random.Range(0, spawnsList.Length-1);

        if (petiteFilleTransform.position != spawnsList[randomSpawn].position && !IsSeenByCamera(spawnsList[randomSpawn].position) && timeSinceLastMove > moveInterval)
        {
            timeSinceLastMove = 0f;
            lastSpawn = petiteFilleTransform;
            petiteFilleTransform.position = spawnsList[randomSpawn].position;
            petiteFilleTransform.rotation = spawnsList[randomSpawn].rotation;
            hasBeenSeen = false;
            laugh.Play();
        }
    }
}
