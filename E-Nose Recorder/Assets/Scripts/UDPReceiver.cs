using UnityEngine;
using System.Collections;
 
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
//using System.IO;

public class UDPReceiver : MonoBehaviour
{ 
    public int port;
    public string lastPacket;
    public bool newPacket = false;
    public bool connected = false;
    public List<String> packetBuffer = new List<String>();

    private IPEndPoint anyIP;
    private UdpClient client;
    private Thread listener;
    private Logger logger;

    // Start is called before the first frame update
    void Start()
    {
        anyIP = new IPEndPoint(IPAddress.Any, port);
        client = new UdpClient(anyIP);
        logger = transform.GetComponent<Logger>();
        listener = new Thread(new ThreadStart(translater));
        listener.Start();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void translater()
    {
        Byte[] data = new byte[0];
        while (true)
        {
            try
            {            
                data = client.Receive(ref anyIP);
                packetBuffer.Add(Encoding.ASCII.GetString(data));
                lastPacket = Encoding.ASCII.GetString(data);
                connected = true;
                newPacket = true;
            }
            catch (Exception err)
            {       
                client.Close();
                Debug.Log(err);
                return;
            }           
        }
    }

    public void ResetBuffer()
    {
        packetBuffer = new List<String>();
    }
}
