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

	private SerialPort stream;

	void Start(){
		StartThread();
	}

	void Update(){
		SendToArduino("PING");
		Console.WriteLine(ReadFromArduino(stream.ReadTimeout));
	}

	public void StartThread(){
		outputQueue = Queue.Synchronized( new Queue() );
		inputQueue  = Queue.Synchronized( new Queue() );

		thread = new Thread(ThreadLoop);
		thread.Start();
	}

	public void ThreadLoop(){
		// Opens the connection on the serial port
		stream = new SerialPort(port, baudrate);
		stream.ReadTimeout = 50;
		stream.Open();

		// Looping
		while (IsLooping())
		{
			// Send to Arduino
			if (outputQueue.Count != 0){
				string command = (string) outputQueue.Dequeue();
				WriteToArduino(command);
			}

			// Read from Arduino
			string result = ReadFromArduino(stream.ReadTimeout);
			if (result != null){
				inputQueue.Enqueue(result);
			}

		}
		stream.Close();
	}

	public void WriteToArduino(string message) {
		stream.WriteLine(message);
        stream.BaseStream.Flush();
	}
	public void SendToArduino(string command){
		outputQueue.Enqueue(command);
	}

	public string ReadFromArduino (int timeout = 0) {
		if(inputQueue.Count == 0){
			return null;
		}
		return (string) inputQueue.Dequeue();
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
