using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallAbility : MonoBehaviour
{


    [SerializeField]
    private ParticleSystem energyExplosion;

    [SerializeField]
    int positionsToCollect = 6;

    private Positions[] vectorsForRecall;

    
    private Queue<Positions> collectedPositions = new Queue<Positions>();

    [SerializeField]
    private float moveDuration = 1.25f;

    [SerializeField]
    private Ease moveEase = Ease.Linear;

    private void Awake()
    {
        vectorsForRecall = new Positions[positionsToCollect];
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetPositions());
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
        this.GetComponent<PlayerMovement>().enabled = false;
        this.GetComponentInChildren<MouseLook>().enabled = false;

        // Play explosion particles effect on ability activation position
        energyExplosion.transform.position = transform.position;
        energyExplosion.Play();

        //Move the collected positions from the queue to array ready for the animation
        for (int i = positionsToCollect - 1; i >= 0; i--)
        {
            vectorsForRecall[i] = collectedPositions.Dequeue();
        }

        
        Sequence recallSequence = DOTween.Sequence();

        float moveDurationPerPosition = moveDuration / float.Parse(positionsToCollect.ToString());

        //Do the animation by appending all the positions collected
        for (int i = 0; i < positionsToCollect; i++)
        {
            recallSequence.Append(transform.DOLocalMove(vectorsForRecall[i].Position, moveDurationPerPosition).SetEase(moveEase));

            recallSequence.Join(transform.DOLocalRotate(vectorsForRecall[i].Rotation, moveDurationPerPosition));
        }

        

        //Stop the explosion because its annoyin...
        recallSequence.AppendCallback(() => energyExplosion.Stop());
        recallSequence.AppendCallback(() => this.GetComponent<PlayerMovement>().enabled = true);
        recallSequence.AppendCallback(() => this.GetComponentInChildren<MouseLook>().enabled = true);

    }

    private IEnumerator GetPositions()
    {
        while (true)
        {
            //Every time it collects the current position in the Queue
            collectedPositions.Enqueue(new Positions(transform.position, transform.rotation.eulerAngles));
            Debug.Log("enqueued");

            //If our positions in the queue are more than 6 we need to get rid of the old positions because I DON'T WANNA OVERFLOW MY MEMORY lol xD
            if (collectedPositions.Count == positionsToCollect + 1)
            {
                collectedPositions.Dequeue();
                Debug.Log("dequeued");
            }

            //This dude waits for half a sec so ye it's pretty helpful
            yield return new WaitForSeconds(3f / positionsToCollect);
            Debug.Log($"waited for {3f / positionsToCollect} secs");
        }

    }
}
