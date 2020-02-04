using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    public ItemReceiver[] receivers;

    private bool isEnding = false;
    private bool isWin = false;
    private bool isLose = false;

    private const float WIN_PULSE_RANGE = 1.2f;
    private const float WIN_PULSE_SPEED = 0.5f;
    private const float WIN_PULSE_MINIMUM = 0.4f;
    public Light winLight;

    private const float LOSE_PULSE_RANGE = 10f;
    private const float LOSE_PULSE_SPEED = 1f;
    private const float LOSE_PULSE_MINIMUM = 0.5f;
    public Light loseLight;

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO CHECK TIMER
        if (!isEnding) {
            foreach (var receiver in receivers)
            {
                if (!receiver.isComplete) {
                    return;
                }
            }

            // not returned = fulfilled 
            isEnding = true;
            StartWinAnimation();
        }

    }

    void Update()
    {
        if (isEnding && isWin) {
            winLight.range = WIN_PULSE_MINIMUM + Mathf.PingPong(Time.time * WIN_PULSE_SPEED, WIN_PULSE_RANGE - WIN_PULSE_MINIMUM);
        }
        if (isEnding && isLose) {
            loseLight.range = LOSE_PULSE_MINIMUM + Mathf.PingPong(Time.time * LOSE_PULSE_SPEED, LOSE_PULSE_RANGE - LOSE_PULSE_MINIMUM);
        }
    }

    void StartWinAnimation() {
        //
        Debug.Log("Game ended congrats");
        winLight.gameObject.SetActive(true);
        isWin = true;
        // start winSound
        StartCoroutine(WinCoroutine());
    }

    IEnumerator WinCoroutine()
    {
        Debug.Log("Started coroutine");
        yield return new WaitForSeconds(5);
        
        // TODO show something else ?

        Debug.Log("Now load the end scene");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("WinScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Ended coroutine");
        //

    }
}
