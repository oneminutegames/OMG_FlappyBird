using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MyAnimationContoller : MonoBehaviour {

    public GameObject Floor;
    public FlappyBirdController GameController;

    [Header("Activation Interal")]
    public float ActivationDelayObjects = 1.0f; // Time delay between activations
    public List<GameObject> ObjectsToActivate; // List of objects to activate

    [Header("Backgrounds")]
    public float ActivationDelayBackground = 5;
    public List<GameObject> BackgroundsToActivate; // List of objects to activate
    public List<AmbientMode> AmbientModes; // List of objects to activate

    private int BackgroundIndex = 0;

    void Update() {

        // animate
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(ActivateObjectsOverTime());
            StartCoroutine(ScaleFloor());
        }

        // change background
        if (Input.GetKeyDown(KeyCode.B)) {
            NextBackground();
        }
    }

    private void NextBackground() {
        BackgroundsToActivate[BackgroundIndex].SetActive(false);
        BackgroundIndex = (BackgroundIndex + 1) % BackgroundsToActivate.Count;
        GameObject currentBackground = BackgroundsToActivate[BackgroundIndex];
        currentBackground.SetActive(true);
        RenderSettings.ambientMode = AmbientModes[BackgroundIndex];
    }

    private IEnumerator ScaleFloor() {
        // scale floor
        for (float t = 0; t <= 1; t += Time.deltaTime) {
            Floor.transform.localScale = Vector3.Lerp(new Vector3(20, 1, 20), new Vector3(20, 1, 5), t);
            yield return null;
        }
    }

    private IEnumerator ActivateObjectsOverTime() {

        // pause game
        GameController.enabled = false;

        // first hide all objects from the list
        foreach (GameObject obj in ObjectsToActivate) {
            obj.SetActive(false); // Activate the object
        }

        // loop through objects and activate them one after another
        foreach (GameObject obj in ObjectsToActivate) {
            yield return new WaitForSeconds(ActivationDelayObjects); // Wait for the specified delay
            obj.SetActive(true); // Activate the object
        }

        // Wait before next sequence
        yield return new WaitForSeconds(1);

        // unpase game
        GameController.enabled = true;

        // Wait before next sequence
        yield return new WaitForSeconds(ActivationDelayBackground);

        // loop through backgrounds
        for (int i = 0; i < BackgroundsToActivate.Count; i++) {
            yield return new WaitForSeconds(ActivationDelayBackground); // Wait for the specified delay
            NextBackground();
        }
    }
}
