using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class WebButton : MonoBehaviour
{
    [SerializeField] private string _targetURL = "https://www.example.com";

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Application.OpenURL(_targetURL);
    }
}
