using UnityEngine;
using UnityEngine.UI;
using Client;

public class SendMessage : MonoBehaviour {

    public InputField inputMessage;

    private void Awake()
    {
        ConnServer.Initial("127.0.0.1", 9002);
        ConnServer.Instance.Conn();
        Input.imeCompositionMode = IMECompositionMode.On;   // 设置为可以输入中文
    }

    void Start () {
       
	}

    public void cw(string str)
    {
        Debug.Log(str);
    }

    public void SendButton()
    {
        string message = inputMessage.text;
        if (string.IsNullOrEmpty(message))
            return;
        Camera.main.transform.GetComponent<_OnGUI>().MyClient = true;
        Camera.main.transform.GetComponent<_OnGUI>().ReceiveMesEvent("-1:"+inputMessage.text);
        ConnServer.Instance.SendMessage(message);
    }

}
