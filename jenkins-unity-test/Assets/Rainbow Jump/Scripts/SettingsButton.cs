using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainbowJump.Scripts
{

    public class SettingsButton : MonoBehaviour
    {
        public bool muted;
        public GameObject buttons;
        public GameObject muteButton;
        public GameObject unMuteButton;
        public Animator buttonsAnimator;
        public AnimationClip newAnimationClip;
        public int timesClicked = 0;
        public void SettingsButtonClick()
        {
            timesClicked++;
            if (timesClicked % 2 == 0)
            {
                buttonsAnimator.Play(newAnimationClip.name);
                Invoke("DisableButtons", 0.35f);
            }
            else
            {
                buttons.SetActive(true);
            }
        }

        private void DisableButtons()
        {
            buttons.SetActive(false);
        }

        public void UnMuteButtonClick()
        {
            muted = true;
            unMuteButton.SetActive(false);
            muteButton.SetActive(true);
            PlayerPrefs.SetInt("muted", muted ? 1 : 0);
            AudioListener.volume = 0f;
        }

        public void MuteButtonClick()
        {
            muted = false;
            unMuteButton.SetActive(true);
            muteButton.SetActive(false);
            PlayerPrefs.SetInt("muted", muted ? 1 : 0);
            AudioListener.volume = 1f;
        }

        void Start()
        {
            muted = PlayerPrefs.GetInt("muted", 0) == 1;
            if (muted)
            {
                UnMuteButtonClick();
            }
            else
            {
                MuteButtonClick();
            }
        }

    }
}
