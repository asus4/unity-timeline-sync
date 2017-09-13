using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

using UnityEngine;
using UnityEngine.Playables;

namespace AppKit
{
    public class TimelineSync : MonoBehaviour
    {
        public enum NotifyMode
        {
            Specific,
            Multicast,
            Broadcast,
        }

        #region Public properties
        public PlayableDirector director;

        public NotifyMode notifyMode;
        public string address = "127.0.0.1";

        [Range(1024, IPEndPoint.MaxPort)]
        public int port = 9765; // default

        #endregion Public properties

        #region Private properties


        IPEndPoint remote;
        IPEndPoint local;
        UdpClient client;

        float lastSend;

        #endregion // Private properties



        #region Life cycle

        void Start()
        {
            IPAddress addr;
            switch (notifyMode)
            {
                case NotifyMode.Multicast:
                    addr = IPAddress.Parse(address);
                    if (!IsMulticastAddress(addr))
                    {
                        Debug.LogWarningFormat("{0} is not multicast addres", address);
                    }
                    break;
                case NotifyMode.Broadcast:
                    addr = IPAddress.Broadcast;
                    break;
                default:
                    addr = IPAddress.Parse(address);
                    break;
            }
            remote = new IPEndPoint(addr, port);
            local = new IPEndPoint(IPAddress.Any, port);
            client = new UdpClient(local);
            lastSend = float.MinValue;
            if (notifyMode == NotifyMode.Multicast)
            {
                client.JoinMulticastGroup(addr);
            }
        }

        void OnDestroy()
        {
            client.Close();
        }

        void OnValidate()
        {
            if (director == null)
            {
                director = GameObject.FindObjectOfType<PlayableDirector>();
            }

            // Check Multicast address
            bool isMulticast = IsMulticastAddress(IPAddress.Parse(address));
            if (notifyMode == NotifyMode.Multicast && !isMulticast)
            {
                address = "225.6.7.8";
            }
            else if (notifyMode == NotifyMode.Specific && isMulticast)
            {
                address = "127.0.0.1";
            }
        }

        void Update()
        {
            while (client.Available > 0)
            {
                // Parse value
                byte[] bytes = client.Receive(ref local);
                double time = BitConverter.ToDouble(bytes, 0);
                float timestamp = BitConverter.ToSingle(bytes, 8);
                if (lastSend != timestamp)
                {
                    director.time = time;
                }
            }
        }

        #endregion Life cycle

        #region Public methods
        public double Time
        {
            get { return director.time; }
            set
            {
                lastSend = UnityEngine.Time.time;
                // Send sync packet
                var bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(value));
                bytes.AddRange(BitConverter.GetBytes(lastSend));
                client.Send(bytes.ToArray(), bytes.Count, remote);
                // Debug.LogFormat("sync:send {0} on {1}", value, UnityEngine.Time.time);
                director.time = value;
            }
        }
        #endregion Public methods

        #region Private methods

        static bool IsMulticastAddress(IPAddress addr)
        {
            // 224.0.0.0 - 239.255.255.255
            byte[] bytes = addr.GetAddressBytes();
            return (bytes[0] & 0xF0) == 0xE0;
        }

        #endregion // Private methods
    }
}
