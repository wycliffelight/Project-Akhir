using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeManager : MonoBehaviour
{
    private readonly int[] cheatCode1 = { 0, 0, 1, 1, 2, 3 }; // Up, Up, Down, Down, Left, Right | Auto jump
    private readonly int[] cheatCode2 = { 2, 2, 3, 3, 0, 1 }; // Left, Left, Right, Right, Up, Down | Infinite jump

    private int[] currentCode;
    private int currentIndex1 = 0;
    private int currentIndex2 = 0;
    private bool isCheatCode1Active = false;
    private bool isCheatCode2Active = false;

    [SerializeField] PlayerScript playerScript;
    private Coroutine cheatCode1Coroutine;
    private Coroutine cheatCode2Coroutine;

    void Start()
    {
        currentCode = cheatCode1;
    }

    void Update()
    {
        // Input Keyboard PC
        if (Input.GetKeyDown(KeyCode.W))
        {
            CheckCheatCode(0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            CheckCheatCode(1); 
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            CheckCheatCode(2); 
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            CheckCheatCode(3);
        }

        // Input Controller Xbox/Ps
        float dpadX = Input.GetAxis("DPadX");
        float dpadY = Input.GetAxis("DPadY");
        float stickX = Input.GetAxis("Horizontal");
        float stickY = Input.GetAxis("Vertical");

        if (dpadY > 0.5f || stickY > 0.5f) // Up
        {
            CheckCheatCode(0);
        }
        else if (dpadY < -0.5f || stickY < -0.5f) // Down
        {
            CheckCheatCode(1);
        }
        else if (dpadX < -0.5f || stickX < -0.5f) // Left
        {
            CheckCheatCode(2);
        }
        else if (dpadX > 0.5f || stickX > 0.5f) // Right
        {
            CheckCheatCode(3);
        }
    }

    void CheckCheatCode(int input)
    {
        if (input == cheatCode1[currentIndex1])
        {
            currentIndex1++;
            if (currentIndex1 >= cheatCode1.Length)
            {
                ToggleCheatCode1();
                currentIndex1 = 0;
            }
        }
        else
        {
            currentIndex1 = 0;
        }

        if (input == cheatCode2[currentIndex2])
        {
            currentIndex2++;
            if (currentIndex2 >= cheatCode2.Length)
            {
                ToggleCheatCode2();
                currentIndex2 = 0;
            }
        }
        else
        {
            currentIndex2 = 0;
        }
    }

    void ActivateCheatCode()
    {
        if (currentCode == cheatCode1)
        {
            Debug.Log("Cheat Code 1 Activated");
            isCheatCode1Active = !isCheatCode1Active;

            if (isCheatCode1Active)
            {
                if (cheatCode1Coroutine == null)
                {
                    cheatCode1Coroutine = StartCoroutine(EnforceJumpToConsumeTrue());
                }
            }
            else
            {
                if (cheatCode1Coroutine != null)
                {
                    StopCoroutine(cheatCode1Coroutine);
                    cheatCode1Coroutine = null;
                }
            }

            Debug.Log("Cheat Code 1 toggled: " + isCheatCode1Active);
        }
        else if (currentCode == cheatCode2)
        {
            Debug.Log("Cheat Code2 Activated");
            isCheatCode2Active = !isCheatCode2Active;

            if (isCheatCode2Active)
            {
                if (cheatCode2Coroutine == null)
                {
                    cheatCode2Coroutine = StartCoroutine(EnforceCoyoteUsableTrue());
                }
            }
            else
            {
                if (cheatCode2Coroutine != null)
                {
                    StopCoroutine(cheatCode2Coroutine);
                    cheatCode2Coroutine = null;
                }
            }

            Debug.Log("Cheat Code 1 toggled: " + isCheatCode2Active);
        }

        currentCode = cheatCode1;
    }

    void ToggleCheatCode1()
    {
        Debug.Log("Cheat Code 1 Activated!");
        isCheatCode1Active = !isCheatCode1Active;

        if (isCheatCode1Active)
        {
            if (cheatCode1Coroutine == null)
            {
                cheatCode1Coroutine = StartCoroutine(EnforceJumpToConsumeTrue());
            }
        }
        else
        {
            if (cheatCode1Coroutine != null)
            {
                StopCoroutine(cheatCode1Coroutine);
                cheatCode1Coroutine = null;
            }
        }

        Debug.Log("Cheat Code 1 toggled: " + isCheatCode1Active);
    }

    void ToggleCheatCode2()
    {
        Debug.Log("Cheat Code 2 Activated!");
        isCheatCode2Active = !isCheatCode2Active;

        if (isCheatCode2Active)
        {
            if (cheatCode2Coroutine == null)
            {
                cheatCode2Coroutine = StartCoroutine(EnforceCoyoteUsableTrue());
            }
        }
        else
        {
            if (cheatCode2Coroutine != null)
            {
                StopCoroutine(cheatCode2Coroutine);
                cheatCode2Coroutine = null;
            }
        }

        Debug.Log("Cheat Code 2 toggled: " + isCheatCode2Active);
    }


    IEnumerator EnforceJumpToConsumeTrue()
    {
        while (isCheatCode1Active)
        {
            PlayerScript.jumpToConsume = true;
            yield return null;
        }
    }

    IEnumerator EnforceCoyoteUsableTrue()
    {
        while (isCheatCode2Active)
        {
            PlayerScript.coyoteUsable = true;
            yield return null;
        }
    }

}
