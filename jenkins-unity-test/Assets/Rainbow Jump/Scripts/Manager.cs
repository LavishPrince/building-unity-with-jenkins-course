using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RainbowJump.Scripts
{
    public class Manager : MonoBehaviour
    {
        public string androidUrl;
        public string iOSUrl;

        public Player playerMovement;
        public TrailRenderer playerTrail;
        public Spawner spawner;
        public GameObject mainCamera;

        public Transform playerTransform;
        public Text scoreText;
        public Text highScoreText; // reference to the UI text for displaying the high score

        public float score = 0f;
        private float highScore = 0f; // variable to hold the high score

        public bool gameOver = false;

        public SpriteRenderer playerSprite;

        public GameObject playerScore;
        public GameObject deathParticle;
        public GameObject gameOverUI;
        public GameObject tapToPlayUI;
        public GameObject tapToStartBtn;
        public GameObject settingsButton;
        public GameObject settingsButtons;
        public SettingsButton settingsButtonScript;

        public AudioClip tapSound;
        public AudioClip deathSound;
        public AudioClip buttonSound;
        private AudioSource audioSource;


        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 144;
            playerMovement.enabled = false;
            playerMovement.rb.simulated = false;

            audioSource = GetComponent<AudioSource>();

            // Load the high score from PlayerPrefs and display it in the UI
            highScore = PlayerPrefs.GetFloat("HighScore", 0f);
            highScoreText.text = "High Score: " + highScore.ToString("0");
        }

        // Update is called once per frame
        void Update()
        {
            if (gameOver == true)
            {
                playerMovement.enabled = false;
                playerMovement.rb.simulated = false;
                playerScore.SetActive(false);
                playerSprite.enabled = false;
                tapToStartBtn.SetActive(false);
                gameOverUI.SetActive(true);
                deathParticle.SetActive(true);
                gameOver = false;

                // Save the high score if it's greater than the current high score
                if (score > highScore)
                {
                    highScore = score;
                    PlayerPrefs.SetFloat("HighScore", highScore);
                    highScoreText.text = "High Score: " + highScore.ToString("0");
                }
            }

            // Update the score to be the highest player's y position in the current game session
            if (playerTransform.position.y > score)
            {
                score = playerTransform.position.y;
            }

            // Display the score as the highest player's y position minus 3
            scoreText.text = (score).ToString("0");
        }

        public void TapToStart()
        {
            settingsButtonScript.timesClicked = 0;
            PlayTapSound();
            playerMovement.rb.simulated = true;
            playerMovement.rb.velocity = Vector2.up * playerMovement.jumpForce;
            playerScore.SetActive(true);
            tapToPlayUI.SetActive(false);
            settingsButton.SetActive(false);
            settingsButtons.SetActive(false);
            tapToStartBtn.SetActive(false);
            playerMovement.enabled = true;
        }

        public void RestartGame()
        {
            settingsButton.SetActive(true);
            tapToPlayUI.SetActive(true);
            tapToStartBtn.SetActive(true);
            playerSprite.enabled = true;
            deathParticle.SetActive(false);
            gameOverUI.SetActive(false);
            playerMovement.rb.simulated = false;
            playerMovement.transform.position = new Vector3(0f, -3f, 0f);
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            spawner.DestroyAllObstacles();
            spawner.InitializeObstacles();
            score = 0f;

            playerTrail.Clear();
        }

        public void PlayTapSound()
        {
            audioSource.PlayOneShot(tapSound);
        }

        public void PlayDeathSound()
        {
            audioSource.PlayOneShot(deathSound);
        }

        public void PlayButtonSound()
        {
            audioSource.PlayOneShot(buttonSound);
        }

        public void OpenURL()
        {
#if UNITY_ANDROID
        Application.OpenURL(androidUrl);
#elif UNITY_IOS
        Application.OpenURL(iOSUrl);
#endif
        }
    }
}