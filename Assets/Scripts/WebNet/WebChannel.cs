using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class WebChannel
{
    public class SendStruct : IDisposable
    {
        public UnityWebRequest request = null;
        public System.Action<string, byte[]> callback = null;

        public void Dispose()
        {
            request.Dispose();
            callback = null;
        }
    }
    List<SendStruct> ssPool = null;

    public WebChannel()
    {
        ssPool = new List<SendStruct>();
    }
    public void Send(string url, byte[] data, System.Action<string, byte[]> callback)
    {
        if (callback == null)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, data);
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.timeout = 30;
            request.SendWebRequest();
            return;
        }
        SendStruct ss = new SendStruct();
        ssPool.Add(ss);
        ss.request = UnityWebRequest.Put(url, data);
        ss.request.method = UnityWebRequest.kHttpVerbPOST;
        ss.request.timeout = 15;
        ss.callback = callback;
        ss.request.SendWebRequest();
    }
    public void Get(string url, System.Action<string, byte[]> callback)
    {
        SendStruct ss = new SendStruct();
        ssPool.Add(ss);
        ss.request = UnityWebRequest.Get(url);
        ss.request.timeout = 600;
        ss.callback = callback;
        ss.request.SendWebRequest();
    }

    public void OnUpdate()
    {
        for (int i = 0; i < ssPool.Count; i++)
        {
            SendStruct ss = ssPool[i];

            if (ss.request.isDone)
            {

                ssPool.RemoveAt(i);
                i--;
                if (ss.request.isHttpError || ss.request.isNetworkError)
                {
                    ss.callback(ss.request.error, null);
                }
                else
                {
                    ss.callback(null, ss.request.downloadHandler.data);
                }
                ss.Dispose();
            }
        }
    }
    public void Clear()
    {
        for (int i = 0; i < ssPool.Count; i++)
        {
            ssPool[i].Dispose();
        }
        ssPool.Clear();
    }
}
