using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlappyBirdController : MonoBehaviour {

    public GameObject Bird;
    public GameObject PipePrefab;
    public GameObject WingsLeft;
    public GameObject WingsRight;
    public Text ScoreText;
    public float Gravity = 30;
    public float Jump = 10;
    public float PipeSpawnInterval = 2;
    public float PipesSpeed = 5;

    private float VerticalSpeed;
    private float PipeSpawnCountdown;
    private GameObject PipesHolder;
    private int PipeCount;
    private int Score;

    // Start is called before the first frame update
    void Start() {

        // reset score
        Score = 0;
        ScoreText.text = "SCORE: " + Score.ToString();

        // reset pipes
        PipeCount = 0;
        Destroy(PipesHolder);
        PipesHolder = new GameObject("PipesHolder");
        PipesHolder.transform.parent = this.transform;

        // reset bird
        VerticalSpeed = 0;
        Bird.transform.position = Vector3.up * 5;

        // reset time
        PipeSpawnCountdown = 0;
    }

    // Update is called once per frame
    void Update() {

        // STEP 1 - Movement
        VerticalSpeed += -Gravity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space)) {
            VerticalSpeed = 0;
            VerticalSpeed += Jump;
        }

        Bird.transform.position += Vector3.up * VerticalSpeed * Time.deltaTime;

        // STEP 2 - Pipes
        PipeSpawnCountdown -= Time.deltaTime;

        if (PipeSpawnCountdown <= 0) {
            PipeSpawnCountdown = PipeSpawnInterval;

            // create pipe
            GameObject pipe = Instantiate(PipePrefab);
            pipe.transform.parent = PipesHolder.transform;
            pipe.transform.name = (++PipeCount).ToString();

            // pipe position
            pipe.transform.position += Vector3.right * 12;
            pipe.transform.position += Vector3.up * Mathf.Lerp(4, 9, Random.value);
        }

        // move pipes left
        PipesHolder.transform.position += Vector3.left * PipesSpeed * Time.deltaTime;


        // STEP 4 - Bird animation

        // nose dive
        float speedTo01Range = Mathf.InverseLerp(-10, 10, VerticalSpeed);
        float noseAngle = Mathf.Lerp(-30, 30, speedTo01Range);
        Bird.transform.rotation = Quaternion.Euler(Vector3.forward * noseAngle) * Quaternion.Euler(Vector3.up * 20);

        // wings
        float flapSpeed = (VerticalSpeed > 0) ? 30 : 5;
        float angle = Mathf.Sin(Time.time * flapSpeed) * 45;
        WingsLeft.transform.localRotation = Quaternion.Euler(Vector3.left * angle);
        WingsRight.transform.localRotation = Quaternion.Euler(Vector3.right * angle);

        // STEP 5 - Score
        foreach (Transform pipe in PipesHolder.transform) {

            // when pipe has passed the bird
            if (pipe.position.x < 0) {
                int pipeId = int.Parse(pipe.name);
                if (pipeId > Score) {
                    Score = pipeId;
                    ScoreText.text = "SCORE: " + Score.ToString();
                }
            }

            // when pipe is offscreen
            if (pipe.position.x < -12) {
                Destroy(pipe.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collider) {

        // STEP 3 - Collision
        Start();
    }
}
