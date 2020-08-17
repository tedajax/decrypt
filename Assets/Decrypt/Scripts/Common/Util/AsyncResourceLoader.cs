using System;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceLoader
{
    void Load<T>(string path, Action<T> callback) where T : UnityEngine.Object;
}


public class AsyncResourceLoader : IResourceLoader
{
    private Dictionary<string, ResourceRequest> requests = new Dictionary<string, ResourceRequest>(32);

    public void Load<T>(string path, Action<T> callback) where T : UnityEngine.Object
    {
        ResourceRequest request;
        if (requests.TryGetValue(path, out request))
        {
            request.completed += (AsyncOperation op) =>
            {
                callback?.Invoke(request.asset as T);
            };
        }
        else
        {
            request = Resources.LoadAsync<T>(path);
            requests.Add(path, request);
            request.completed += (AsyncOperation op) =>
            {
                requests.Remove(path);
                callback?.Invoke(request.asset as T);
            };
        }
    }
}