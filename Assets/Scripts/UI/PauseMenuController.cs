using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public void BackToMainMenu() => GameManager.Instance.BackToMainMenu();

    public void PauseGame() => GameManager.Instance.PauseGame();

    public void ResumeGame() => GameManager.Instance.ResumeGame();
}
