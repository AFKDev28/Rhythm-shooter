using UnityEngine;
using System.IO;

public class FileUIController : MonoBehaviour
{
    private string midifolder = Application.streamingAssetsPath;
    [SerializeField] private MIDIFileButton buttonPrefab;
    [SerializeField] private GameObject content;


    // Start is called before the first frame update
    void Start()
    {
        string[] path = Directory.GetFiles(midifolder, "*.mid");

        foreach (var path2 in path)
        {
            MIDIFileButton newButton = Instantiate(buttonPrefab);
            newButton.SetFileName(Path.GetFileName(path2));
             newButton.transform.SetParent(content.transform, false);

        }

    }

}
