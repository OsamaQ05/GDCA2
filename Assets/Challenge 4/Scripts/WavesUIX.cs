using UnityEngine;
using TMPro;

public class WaveUIX : MonoBehaviour
{
    public TMP_Text waveText;

    void Start()
    {
        waveText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        
        waveText.text = SpawnManagerX.UItext;
    }
}
