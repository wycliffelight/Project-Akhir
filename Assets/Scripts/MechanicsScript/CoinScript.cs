using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    PlayerScript PlayerComponent;
    public float animationDuration = 0.3f; 
    public float moveDistance = 0.7f; 

    void Start()
    {
        PlayerComponent = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerComponent.coin++;
            StartCoroutine(PlayCollectAnimation());
        }
    }

    IEnumerator PlayCollectAnimation()
    {
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + new Vector3(0, moveDistance, 0);
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        Destroy(gameObject);
    }

}
