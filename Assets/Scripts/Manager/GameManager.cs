using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    private string midiName;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public string MIDIName { get  { return midiName; }  }

    public void PlaySong(string filename)
    {
        midiName = filename;
        SceneManager.LoadScene(1);
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1.0f;

        SceneManager.LoadScene(0);
    }
}
