using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public bool record = true;
    public string fileName = "default_name.csv";
    public int recordTimeSeconds = 600;
    public bool orderPackets = true;
    
    private UDPReceiver receiver;
    private StreamWriter writer;
    private float timeNow;

    // Start is called before the first frame update
    void Start()
    {
        receiver = transform.GetComponent<UDPReceiver>();
        if (record) writer = new StreamWriter(fileName, true);
        Debug.Log("Recording: " + fileName);
    }

    // Update is called once per frame
    void Update()
    {
        // if logging in real time
        if (receiver.newPacket & !orderPackets)
        {
            if (record)
            {
                timeNow = Time.time;
                
                if (timeNow < recordTimeSeconds)
                {
                   writer.WriteLine(timeNow + "," + receiver.lastPacket);
                   receiver.newPacket = false;
                }

                if (timeNow > recordTimeSeconds)
                {
                    writer.Close();
                    record = false;
                    Debug.Log("Time Left: 0");
                }
                else
                {
                    Debug.Log("Time Left: " + (recordTimeSeconds - timeNow));
                }
            }                  
        }
        // if reordering packets
        if(orderPackets && record)
        {
            timeNow = Time.time;

            if (timeNow > recordTimeSeconds)
            {
                record = false;
                reorder();
                Debug.Log("Time Left: 0");
            }
            else
            {
                Debug.Log("Time Left: " + (recordTimeSeconds - timeNow));
            }
        }
    }

    public void reorder()
    {
        SortedDictionary<int, String> tmp = new SortedDictionary<int, string>();

        List<String> buf = receiver.packetBuffer;
        receiver.ResetBuffer();

        String[] vals;
        int[] timeSent = new int[buf.Count];
        String[] sensorValBuf = new String[buf.Count];

        int c = 0;

        foreach(String packet in buf)
        {
            vals = packet.Split(","[0]);
            // extract first element (time) for reordering then put in buffer
            String sensorVals = "";
            for(int i = 1; i < vals.Length; ++i)
            {
                sensorVals += vals[i];
                sensorVals += ",";
            }
            // store the time in ms
            timeSent[c] = int.Parse(vals[0]);
            sensorValBuf[c] = sensorVals;
            ++c;
        }

        int[] oldTimeSent = timeSent;

        for(int i = 0; i < timeSent.Length; ++i)
        {
            try
            {
                tmp.Add(oldTimeSent[i], sensorValBuf[i]);
            } catch(Exception ex)
            {

            }
        }

        Array.Sort(timeSent);

        foreach (KeyValuePair<int, String> kvp in tmp)
        {
             Debug.Log(kvp.Key.ToString() + "," + kvp.Value);
            // write time in seconds 
             writer.WriteLine(((kvp.Key - timeSent[0]) / 1000f).ToString() + "," + kvp.Value);
        }
        Debug.Log("Saved recording!");
        writer.Close();
    }
}   