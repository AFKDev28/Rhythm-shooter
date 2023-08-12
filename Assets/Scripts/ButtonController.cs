using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI fileName;

    public void SetFileName(string fileName)
    {
        this.fileName.text = fileName;
    }

    public void StartMIDIFile()
    {
        GameManager.Instance.PlaySong(fileName.text);
    }
}
