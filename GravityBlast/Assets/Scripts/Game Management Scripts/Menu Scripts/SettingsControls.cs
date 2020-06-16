using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsControls : MonoBehaviour {

    [SerializeField] Player_Movement playerMovement;
    [SerializeField] Player_BlastMechanics playerBlastMechanics;
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider volumeSlider;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Toggle invertLookToggle;
    [SerializeField] Toggle autoJumpToggle;

    private void Start () {
        //playerMovement = GameManager.gm.player.GetComponent<Player_Movement>();
        //playerBlastMechanics = GameManager.gm.player.GetComponent<Player_BlastMechanics>();
        float masterVolume = 0f;
        audioMixer.GetFloat("MasterVolume", out masterVolume);
        volumeSlider.value = masterVolume;
    }

    private void Awake () {
        float masterVolume = 0f;
        audioMixer.GetFloat("MasterVolume", out masterVolume);
        volumeSlider.value = masterVolume;
        //ScreenResolution
        //invertLookToggle.isOn = playerMovement.invertVerticalInput;
        //autoJumpToggle.isOn = playerBlastMechanics.autoJumpBeforeGroundedRocketJump;
    }

    public void SetMasterVolume (float v) {
        audioMixer.SetFloat("MasterVolume", v);
    }

    public void ToggleInvertLook (bool b) {
        playerMovement.invertVerticalInput = b;
    }

    public void ToggleAutoJump (bool b) {
        playerBlastMechanics.autoJumpBeforeGroundedRocketJump = b;
    }

}
