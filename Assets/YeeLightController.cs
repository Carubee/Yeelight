using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class YeelightController : MonoBehaviour
{
    private class Yeelight
    {
        
        public string ipAddress;
        public int port = 55443;
        public TcpClient client;
        public NetworkStream stream;
        public bool isOn = false;

        public Yeelight(string ip)
        {
            ipAddress = ip;
        }

        public void Connect()
        {
            try
            {
                client = new TcpClient(ipAddress, port);
                stream = client.GetStream();
                Debug.Log($"Connected to Yeelight at {ipAddress}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to connect to Yeelight {ipAddress}: " + e.Message);
            }
        }

        public void SendCommand(string method, params object[] parameters)
        {
            if (client == null || !client.Connected)
            {
                Debug.LogError($"Not connected to Yeelight {ipAddress}!");
                return;
            }

            try
            {
                var command = new
                {
                    id = 1,
                    method = method,
                    @params = parameters
                };

                string json = JsonConvert.SerializeObject(command);
                byte[] data = Encoding.UTF8.GetBytes(json + "\r\n");
                stream.Write(data, 0, data.Length);
                Debug.Log($"Command sent to {ipAddress}: {json}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send command to Yeelight {ipAddress}: " + e.Message);
            }
        }

        public void ToggleLight()
        {
            isOn = !isOn;
            SendCommand("set_power", isOn ? "on" : "off", "smooth", 500);
            Debug.Log($"Light at {ipAddress} is now {(isOn ? "ON" : "OFF")}");
        }

        public void CloseConnection()
        {
            if (stream != null) stream.Close();
            if (client != null) client.Close();
        }
    }

    private Yeelight[] yeelights;

    void Start()
    {
        yeelights = new Yeelight[]
        {
            new Yeelight("192.168.0.9"), // เปลี่ยนเป็น IP จริงของไฟดวงที่ 1
            new Yeelight("192.168.0.149"), // เปลี่ยนเป็น IP จริงของไฟดวงที่ 2
            new Yeelight("192.168.0.175")  // เปลี่ยนเป็น IP จริงของไฟดวงที่ 3
        };

        foreach (var light in yeelights)
        {
            light.Connect();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            yeelights[0].ToggleLight();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            yeelights[1].ToggleLight();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            yeelights[2].ToggleLight();
        }
    }

    void OnApplicationQuit()
    {
        foreach (var light in yeelights)
        {
            light.CloseConnection();
        }
    }
}
