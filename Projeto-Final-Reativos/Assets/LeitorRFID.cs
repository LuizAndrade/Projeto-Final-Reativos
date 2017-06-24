using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class LeitorRFID : MonoBehaviour {

	private Thread thread;

	private Queue outputQueue;
	private Queue inputQueue;

	public string port  = "COM4";
	public int baudrate = 9600;
	public bool looping = true;
	public int i = 0;
	public int j = 0;

	private SerialPort stream;

	void Start(){
		StartThread();
	}

	void Update(){
		if (j == 0){
			SendToArduino("AAA");
			j = 1;
		}else{
			SendToArduino("PING");
			j = 0;
		}
		Console.WriteLine(ReadFromArduino(stream.ReadTimeout));
		//  GetFromArduino();
	}

	public void StartThread(){
		outputQueue = Queue.Synchronized( new Queue() );
		inputQueue  = Queue.Synchronized( new Queue() );

		// Debug.Log("1");

		thread = new Thread(ThreadLoop);
		thread.Start();
	}

	public void ThreadLoop(){
		// Opens the connection on the serial port
		stream = new SerialPort(port, baudrate);
		stream.ReadTimeout = 50;
		stream.Open();
		// Debug.Log("2");

		// Looping
		while (IsLooping())
		{
			if (i<99){
				i++;
			}else{
				Debug.Log("Parei kkk");
				StopThread();
			}
			// Debug.Log("saindo da fila: ");
			// Debug.Log(outputQueue.Count);
			// Send to Arduino
			if (outputQueue.Count != 0){

				string command = (string) outputQueue.Dequeue();
				// Debug.Log("enviado\n");
				WriteToArduino(command);
			}

			// Read from Arduino
			int result = ReadFromArduino(stream.ReadTimeout);
			Debug.Log(result);

			if (result != -1){
				inputQueue.Enqueue(result);
			}
			// Debug.Log("entrando na fila:");

		}
		stream.Close();
		thread.Abort();
	}

	public void WriteToArduino(string message) {
		stream.WriteLine(message);
        stream.BaseStream.Flush();
	}
	public void SendToArduino(string command){
		outputQueue.Enqueue(command);
	}

	public void GetFromArduino(){
		int result = stream.ReadByte();
		inputQueue.Enqueue(result);
	}

	public int ReadFromArduino (int timeout = 0) {
		/*TODO:
		Unity nao recebe do arduino
		fazer ler com o ReadByte() e
		empilhar na inputQueue.
		*/
		stream.ReadTimeout = timeout;
		try{
			return stream.ReadByte();
		}catch(TimeoutException){
			return -1;
		}

	}

	public bool IsLooping (){
		lock (this){
			return looping;
		}
	}

	public void StopThread(){
		lock(this){
			looping = false;
		}
	}
	public void Close(){
		stream.Close();
	}
}
