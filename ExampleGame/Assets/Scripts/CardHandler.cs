using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CardHandler : MonoBehaviour
{
    private Queue<Card> flippedCards = new Queue<Card>();
    public GameObject particles;


    void Update()
    {
        // to block raycasting when the pointer is over UI element
        if (EventSystem.current.IsPointerOverGameObject()) return;

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
        if (!cardToFlip.IsFlipped()) // flip only if it is not flipped
        {  
            cardToFlip.Flip();
            flippedCards.Enqueue(cardToFlip);

            // Only check matches if the enqueued card count is divisible by 2 in order to check them in pairs. 
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

        // midpoint for the popup score text
        Vector3 midpoint = (firstCard.transform.position + secondCard.transform.position) / 2;
        // convert it to screen coordinates
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(midpoint);


        if (firstCard.id != secondCard.id)
        {
            GameManager.Instance.addMishitInRow(); // used for penalty
            // No match, flip back
            firstCard.Flip();
            secondCard.Flip();
        }
        else
        {
            // Match found

            GameManager.Instance.addHitsInRow(); // used for combo bonus
            Debug.Log("Match!");

            GameManager.Instance.UpdateHitCount(screenPosition);

            //particle effect at matching card positions
            Instantiate(particles, firstCard.transform.position + Vector3.up, firstCard.transform.rotation);
            Instantiate(particles, secondCard.transform.position + Vector3.up, secondCard.transform.rotation);
            //destroy the cards if they matched
            Destroy(firstCard.gameObject);
            Destroy(secondCard.gameObject);

        }

        GameManager.Instance.UpdateTurnCount(screenPosition);
    }


    public void clearCardQueue()
    {
        flippedCards.Clear();
    }
}
