using ScObjects;
using TMPro;
using UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SteelKaputScript : MonoBehaviour
{
    [SerializeField] private GameObject text;
    [SerializeField] private float text_timer;
    [SerializeField] private float screen_timer;
    [SerializeField] private MachineScObject kaput;
    [SerializeField] private WeaponScObject kaputweapon;

    private void Start()
    {
        CurrentValues.SetCurrentPlayerMachine(kaput);
        CurrentValues.SetCurrentPlayerWeapon(kaputweapon);
    }

    private void Update()
    {
        if (text_timer > 0)
            text_timer -= Time.deltaTime;
        if (text_timer <= 0)
            text.SetActive(true);
        if (screen_timer > 0)
            screen_timer -= Time.deltaTime;
        if (screen_timer <= 0)
            SceneManager.LoadScene("Game scene");
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Game scene");
        }
    }
}
