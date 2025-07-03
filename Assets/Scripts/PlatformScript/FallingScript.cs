using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class FallingScript : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    public float fallDelay = 0.5f;
    public float fallDuration = 0.5f;
    public float fallDistance = 10f;
    public float respawnDelay = 1f;
    public float respawnDistanceBelow = 0.3f; 

    public float vibrationDuration = 0.5f;
    public float vibrationMagnitude = 0.05f;

    private Vector3 originalPosition;
    private Vector3 farAwayPosition = new Vector3(0, -1000, 0); // Pindahkan object ke tempat jauh
    private bool isFalling = false;

    void Start()
    {
        if (tilemap == null)
        {
            tilemap = GetComponentInChildren<Tilemap>();
        }

        originalPosition = tilemap.transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            isFalling = true;
            StartCoroutine(VibrateAndFall());
        }
    }

    IEnumerator VibrateAndFall()
    {
        float elapsedTime = 0f;
        while (elapsedTime < vibrationDuration)
        {
            Vector3 randomOffset = originalPosition + (Vector3)Random.insideUnitCircle * vibrationMagnitude;
            tilemap.transform.position = randomOffset;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset posisi
        tilemap.transform.position = originalPosition;

        // Fall delay
        yield return new WaitForSeconds(fallDelay);
        StartCoroutine(FallCoroutine());
    }

    IEnumerator FallCoroutine()
    {
        Vector3 startPosition = tilemap.transform.position;
        Vector3 endPosition = startPosition + Vector3.down * fallDistance;
        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            tilemap.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tilemap.transform.position = endPosition;

        // Memindahkan object saat jatuh
        tilemap.transform.position = farAwayPosition;

        // Respawn
        Invoke(nameof(RespawnTilemap), respawnDelay);
    }

    void RespawnTilemap()
    {
        Vector3 respawnPosition = originalPosition + Vector3.down * respawnDistanceBelow;
        tilemap.transform.position = respawnPosition;
        StartCoroutine(MoveToOriginalPosition(respawnPosition, originalPosition));
    }

    IEnumerator MoveToOriginalPosition(Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0f;

        // Animasi pendek balik ke tempat awal
        while (elapsedTime < fallDuration)
        {
            tilemap.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tilemap.transform.position = endPosition;
        isFalling = false;
    }

}
