using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class cubeRotation : MonoBehaviour
{
    public float targetPosition=0.0f;
    public float currentPosition=0.0f;

    public string dataFromAPI ="0.0";
        private float lastTime =0.0f;
    private float countTime =0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        currentPosition=360-transform.eulerAngles.z;
        Debug.Log("currentPosition: " + currentPosition);
        if (targetPosition<360 && targetPosition>=0 )
        {

        if (currentPosition > targetPosition - 0.1)
        {transform.Rotate(0,0,+Time.deltaTime*50);}
        
        if (currentPosition < targetPosition + 0.1)
        {transform.Rotate(0,0,-Time.deltaTime*50);}

        }

        countTime +=Time.deltaTime;
        if (countTime>lastTime+0.1f){
            StartCoroutine(GetData());
            lastTime=countTime;
        }


        
        
    }

    private IEnumerator GetData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://host.docker.internal:8082/api/mqtt/data/features/testVar2_Register10"))
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
                targetPosition=float.Parse(dataFromAPI);
                
            }
        }
    }
}
