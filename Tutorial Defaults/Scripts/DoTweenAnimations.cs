using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class DoTweenAnimations : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem energyExplosion;

    private Positions[] vectorsForRecall = new Positions[30];

    [SerializeField]
    private Queue<Positions> collectedPositions = new Queue<Positions>();

    [ SerializeField]
    private float moveDuration = 1.0f;

    [SerializeField]
    private Ease moveEase = Ease.Linear;

    private FirstPersonController fpsController;

    Rigidbody rb;

    [SerializeField]
    Camera mCamera;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetPositions());
        rb = GetComponent<Rigidbody>();
        fpsController = gameObject.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Recall();
        }
    }

    private void Recall()
    {
        fpsController.recall = true;
        // Play explosion particles effect on ability activation position
        energyExplosion.transform.position = transform.position;
        energyExplosion.Play();

        //Move the collected positions from the queue to array ready for the animation
        for (int i = 29; i >= 0; i--)
        {
            vectorsForRecall[i] = collectedPositions.Dequeue();
        }

        //Disable FPS Controller Scripts because it breaks the recall animation (i dont knoe why... xD)
        //gameObject.GetComponent<FirstPersonController>().enabled = false;
        
        rb.isKinematic = false;

        
        Sequence recallSequence = DOTween.Sequence();


        //Do the animation by appending all the positions collected
        for (int i = 0; i < 30; i++)
        {
            recallSequence.Append(transform.DOLocalMove(vectorsForRecall[i].Position, moveDuration).SetEase(moveEase));
            
            recallSequence.Join(transform.DOLocalRotate(vectorsForRecall[i].Rotation, moveDuration));
        }

        //Get back the FPS Controller because i wanna move lol xD
        //recallSequence.AppendCallback(() => gameObject.GetComponent<FirstPersonController>().enabled = true);

        
        //Stop the explosion because its annoyin...
        recallSequence.AppendCallback(() => energyExplosion.Stop());
        
        recallSequence.AppendCallback(() => rb.isKinematic = true);

        recallSequence.OnComplete(() => rb.DOLookAt(vectorsForRecall[29].Position, 0.2f));
    }

    //Method who get the last 6 positions in the last 3 seconds the player was... and we get 6 positions, 1 each half second because i wanna smooth animation
    private IEnumerator GetPositions()
    {
        //Yes it's an infinite loop STOP LAUGHING...
        while (true)
        {
            //Every time it collects the current position in the Queue
            collectedPositions.Enqueue(new Positions(transform.position, transform.rotation.eulerAngles));
            Debug.Log("enqueued");

            //If our positions in the queue are more than 6 we need to get rid of the old positions because I DON'T WANNA OVERFLOW MY MEMORY lol xD
            if (collectedPositions.Count == 31)
            {
                collectedPositions.Dequeue();
                Debug.Log("dequeued");
            }

            //This dude waits for half a sec so ye it's pretty helpful
            yield return new WaitForSeconds(0.1f);
            Debug.Log("waited for 0.5 secs");
        }
        
    }
}
