using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Client;

public class _OnGUI : MonoBehaviour
{
    public int Header;
    public int LabelHeight = 30;
    public int LabelWidth = 300;
    public int HeightOffset = 20;
    public int WidthOffset = 25;

    public List<MessageArrtibute> Recv;
    private List<MessageArrtibute> Temp;
    private GUIStyle style;

    private void Start()
    {
        style = new GUIStyle();
        Recv = new List<MessageArrtibute>();
        Temp = new List<MessageArrtibute>();
        Header = LabelHeight;
        MessageArrtibute mes = new MessageArrtibute(-1, "Welcome To ...", 0, 0, true);
        Recv.Add(mes);
        style.normal.textColor = new Color(1, 0, 0, 1);
        ReceiveEvent.Instance.AddEvent(ReceiveMesEvent);
    }

    private float posY;
    public bool MyClient = false;
    public void ReceiveMesEvent(string mess)
    {
        int clientID = Convert.ToInt32(mess.Substring(0, mess.IndexOf(':')));
        string message = mess.Substring(mess.IndexOf(':') + 1);
        int index = Recv.Count - 1;
        if (Recv[index].y > Screen.height - LabelHeight)
        {
            for (int i = 0; i < Recv.Count - 1; i++)
            {
                Recv[i].y -= LabelHeight;
            }
            MessageArrtibute _mes = new MessageArrtibute(clientID, message, Recv[index].x, (Recv[index].y), MyClient);
            Recv.Add(_mes);
            MyClient = false;
            return;
        }
        MessageArrtibute mes = new MessageArrtibute(clientID, message, Recv[index].x, (Recv[index].y + LabelHeight), MyClient);
        Recv.Add(mes);
        MyClient = false;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            posY = Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            lock (Recv)
            {
                for (int i = 0; i <= Recv.Count - 1; i++)
                {
                    Recv[i].y += (int)(Input.mousePosition.y - posY);
                }
                posY = Input.mousePosition.y;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            posY = 0;
        }

    }
    public bool l = true;
    private void OnGUI()
    {
        Temp = Recv;
        for (int i = Temp.Count - 1; i >= 0; i--)
        {
            if (Recv[i].myClient)
            {
                GUI.Label(new Rect(Recv[i].x, Screen.height - Recv[i].y - LabelHeight, Header, Header), Temp[i].clientID.ToString(), style);
                GUI.Label(new Rect(Recv[i].x + WidthOffset, Screen.height - Recv[i].y - LabelHeight, LabelWidth, LabelHeight), Temp[i].message, style);
            }
            else
            {
                GUI.Label(new Rect(Recv[i].x, Screen.height - Recv[i].y - LabelHeight, Header, Header), Temp[i].clientID.ToString());
                GUI.Label(new Rect(Recv[i].x + WidthOffset, Screen.height - Recv[i].y - LabelHeight, LabelWidth, LabelHeight), Temp[i].message);
            }
        }
    }

    private void OnDestroy()
    {
        ConnServer.Instance.Close();
    }

}

public class MessageArrtibute
{
    public int clientID;
    public string message;
    public int header;
    public int x;
    public int y;
    public bool myClient = false;
    public MessageArrtibute(int clientID, string message, int x, int y, bool myClient)
    {
        this.clientID = clientID;
        this.message = message;
        this.x = x;
        this.y = y;
        this.myClient = myClient;
    }
}
