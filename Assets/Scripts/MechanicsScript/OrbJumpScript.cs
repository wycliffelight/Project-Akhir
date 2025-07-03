using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbJumpScript : MonoBehaviour
{
    public float cooldownTime = 0.7f;
    public float detectionRadius = 0.65f;
    public LayerMask playerLayer;
    public Vector3 scaleIncrease = new Vector3(0.1f, 0.1f, 0.1f);
    public float animationDuration = 0.2f;

    private bool isOnCooldown = false;
    private Transform playerTransform;

    void Update()
    {
        if (!isOnCooldown && PlayerNearby() && Input.GetButton("Jump"))
        {
            StartCoroutine(ActivateOrb());
        }
    }

    // Method mengembalikan bool
    private bool PlayerNearby()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hit != null)
        {
            playerTransform = hit.transform;
            return true;
        }
        return false;
    }

    IEnumerator ActivateOrb()
    {
        isOnCooldown = true;

        // Animasi scaling
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale + scaleIncrease;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;

        // Trigger jump
        if (playerTransform != null)
        {
            PlayerScript playerScript = playerTransform.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                playerScript.JumpFromOrb();
            }
        }

        // Cooldown
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    // Untuk utility
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
