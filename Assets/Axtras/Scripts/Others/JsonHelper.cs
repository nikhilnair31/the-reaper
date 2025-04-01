using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        string newJson = "{ \"rules\": " + json + "}";
        RulesList<T> wrapper = JsonUtility.FromJson<RulesList<T>>(newJson);
        return wrapper.rules;
    }

    [Serializable]
    private class RulesList<T>
    {
        public List<T> rules;
    }
}