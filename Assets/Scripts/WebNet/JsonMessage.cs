using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class JsonMessage : IMessage
{
    public void FromData(byte[] d)
    {
        string json = System.Text.Encoding.UTF8.GetString(d);
        Newtonsoft.Json.JsonConvert.PopulateObject(json, this);
    }
    public void ToData(out byte[] d)
    {
        d = Serialize(this);
    }

    public byte[] GetBytes()
    {
        return Serialize(this);
    }
    public static byte[] Serialize<T>(T obj) where T : JsonMessage
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }
    public static T Deserialize<T>(byte[] data) where T : JsonMessage
    {
        if (data == null) return null;
        string json = System.Text.Encoding.UTF8.GetString(data);

        T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        return t;
    }
}
