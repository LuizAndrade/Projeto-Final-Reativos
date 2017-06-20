using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class LeitorRFID : MonoBehaviour {

	public string port = "COM4";
	public int baudrate = 9600;

	private SerialPort stream;

	public void Open(){
		stream = new SerialPort(port, baudrate);
		stream.ReadTimeout = 50;
		stream.Open();
	}

	/*StartCoroutine(AsynchronousReadFromArduino(
		(string s) => Debug.Log(s),     // Callback
		() => Debug.LogError("Error!"), // Error callback
		10f)                             // Timeout (seconds)
	);*/

	public void WriteToArduino(string message) {
		stream.WriteLine(message);
        stream.BaseStream.Flush();
	}

	public string ReadFromArduino (int timeout = 0) {
		stream.ReadTimeout = timeout;
		try {
			return stream.ReadLine();
		}
		catch (TimeoutException) {
			return null;
		}
	}

	public void Update(){
		WriteToArduino("PING");
	}

	public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
	    DateTime initialTime = DateTime.Now;
	    DateTime nowTime;
	    TimeSpan diff = default(TimeSpan);

	    string dataString = null;

	    do {
	        try {
	            dataString = stream.ReadLine();
	        }
	        catch (TimeoutException) {
	            dataString = null;
	        }

	        if (dataString != null)
	        {
	            callback(dataString);
	            yield return null;
	        } else
	            yield return new WaitForSeconds(0.05f);

	        nowTime = DateTime.Now;
        	diff = nowTime - initialTime;

    	} while (diff.Milliseconds < timeout);

    	if (fail != null)
        	fail();
    	yield return null;
	}
	public void Close(){
		stream.Close();
	}
}
