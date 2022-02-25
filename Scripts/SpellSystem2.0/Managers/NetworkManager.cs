using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public Action awaitCallbacks;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void SendSignal(Signal signal)
    {
        // If client send signal to server
        ReceiveSignal(signal);
    }
    public void ReceiveSignal(Signal signal)
    {
        // If client execute the await callback
        signal.callback?.Invoke();
    }
    public void AwaitSignal()
    {

    }
}
