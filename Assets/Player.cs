using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    Rigidbody rigid;
    Vector3 velocity = new Vector3();
    List<GameObject> speakers = new List<GameObject>();

    public float playerX;

    void Start() {
        rigid = GetComponent<Rigidbody>();
        playerX = rigid.position.x;
        
        /* Find all speakers and sort them by their x component */
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Speaker");
        for (int i = 0; i < gameObjects.Length; i++) {
            speakers.Add(gameObjects[i]);
        }
        speakers.Sort(new SpeakerComparer());

        /* Sync audio samples */
        AudioSource firstSpeaker = speakers[0].GetComponent<AudioSource>();
        foreach(GameObject speaker in speakers) {
            AudioSource audioSource = speaker.GetComponent<AudioSource>();
            audioSource.timeSamples = firstSpeaker.timeSamples;
        }
    }

    class SpeakerComparer : IComparer<GameObject> {
        public int Compare(GameObject a, GameObject b) {
            Transform aTransform = a.GetComponent<Transform>();
            Transform bTransform = b.GetComponent<Transform>();

            return aTransform.position.x.CompareTo(bTransform.position.y);
        }
    }

    // Update is called once per frame
    void Update()  {
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0).normalized * 5;
    }

    private void FixedUpdate()  {
        rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime);
        playerX = rigid.position.x;

        /* If we are past a speaker object, swap it to 2d sound so we always hear it */
        foreach(GameObject speaker in speakers) {
            Transform speakerTransform = speaker.GetComponent<Transform>();
            AudioSource audioSource = speaker.GetComponent<AudioSource>();
            audioSource.spatialBlend = speakerTransform.position.x >= playerX ? 1 : 0;
        }
    }
}
