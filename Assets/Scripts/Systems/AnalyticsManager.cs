using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;


[Serializable]
public class EventData
{
    public string device_id;
    public string event_type;
}

[Serializable]
public class AnalyticsPayload
{
    public string api_key;
    public EventData[] events;
}

public class AnalyticsManager : MonoBehaviour
{
    private const string API_URL = "https://api2.amplitude.com/2/httpapi";
    private string API_KEY = ""; // Your API key here

    public void SendAnalyticsData(string deviceID, Action<bool, string> callback)
    {
        var payload = new AnalyticsPayload
        {
            api_key = API_KEY,
            events = new EventData[]
            {
                new EventData
                {
                    device_id = deviceID,
                    event_type = "Sign up"
                }
            }
        };



        string jsonPayload = JsonUtility.ToJson(payload);

        Debug.Log("Sending analytics data: " + jsonPayload);

        StartCoroutine(SendRequest(jsonPayload, callback));
    }

    private IEnumerator SendRequest(string jsonPayload, Action<bool, string> callback)
    {
        // Create a UnityWebRequest for the POST request
        var request = new UnityWebRequest(API_URL, "POST");

        // Set request headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "*/*");

        // Attach the JSON payload to the request
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            callback(false, request.error);
        }
        else
        {
            Debug.Log("Request successful: " + request.downloadHandler.text);
            callback(true, request.downloadHandler.text);
        }
    }
}
