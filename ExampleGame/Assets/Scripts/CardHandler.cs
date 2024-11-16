using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour
{
    private Queue<Card> flippedCards = new Queue<Card>();
    public GameObject particles;


    void Update()
    {
        // Handle input for both desktop and mobile
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.touchCount > 0)
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Card"))
            {
                Card hitCard = hit.collider.GetComponent<Card>();
                if (hitCard != null)
                {
                    FlipCard(hitCard);
                }
            }
        }
    }

    public void FlipCard(Card cardToFlip)
    {
        if (!cardToFlip.isFlipped)
        {
            cardToFlip.Flip();
            flippedCards.Enqueue(cardToFlip);

            if (flippedCards.Count % 2 == 0)
            {
                StartCoroutine(CheckMatch());
            }
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1.0f);  // Wait to allow the player to see the cards

        var firstCard = flippedCards.Dequeue();
        var secondCard = flippedCards.Dequeue();

        if (firstCard.id != secondCard.id)
        {
            // No match, flip back
            firstCard.Flip();
            secondCard.Flip();
        }
        else
        {
            // Match found
            Debug.Log("Match!");
            Instantiate(particles, firstCard.transform.position + Vector3.up, firstCard.transform.rotation);
            Instantiate(particles, secondCard.transform.position + Vector3.up, secondCard.transform.rotation);
            Destroy(firstCard.gameObject);
            Destroy(secondCard.gameObject);

        }
    }
}
