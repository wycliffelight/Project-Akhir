using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineScript : MonoBehaviour
{
    public GameObject EndScene;

    void OnTriggerEnter2D(Collider2D other)
    {
        EndScene.SetActive(false);
        if (other.transform.tag == "Player")
        {
            OnFinish();
        }
    }

    public void OnFinish()
    {
        EndScene.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            EndScene.SetActive(false);
        }
    }
}
