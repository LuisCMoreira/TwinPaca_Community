using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class cubeColor : MonoBehaviour
{
    public bool booleanValue = false;
    private Renderer objectRenderer;

    private string dataFromAPI = "false";
    private float lastTime = 0.0f;
    private float countTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        countTime += Time.deltaTime;
        if (countTime > lastTime + 0.1f)
        {
            StartCoroutine(GetData());
            lastTime = countTime;
        }

        // Check boolean value and change color accordingly
        if (booleanValue)
        {
            objectRenderer.material.color = Color.red;
        }
        else
        {
            objectRenderer.material.color = Color.green;
        }
    }

    private IEnumerator GetData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://host.docker.internal:8082/api/mqtt/data/features/testVar1_Coil1"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                dataFromAPI = www.downloadHandler.text;
                Debug.Log("Data from API: " + dataFromAPI);

                // Assuming the API returns a boolean as a string ("true" or "false")
                booleanValue = bool.Parse(dataFromAPI);
            }
        }
    }
}
