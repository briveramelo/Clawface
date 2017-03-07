using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UncleDonnie : MonoBehaviour {

	[SerializeField] List<AudioClip> _sounds;


    [SerializeField] float _soundFrequencyMin;
    [SerializeField] float _soundFrequencyMax;

    private void Awake() {
        StartCoroutine (Speak());
    } 

    IEnumerator Speak () {
        while (true) {

            GetComponent<AudioSource>().PlayOneShot (_sounds[Random.Range (0, _sounds.Count)]);

            yield return new WaitForSeconds (Random.Range (_soundFrequencyMin, _soundFrequencyMax));
        }
    }
}
