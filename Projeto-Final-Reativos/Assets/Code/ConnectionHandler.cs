using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

using UnityEngine;

public class ConnectionHandler
{
	private SerialPort stream;
	private Thread thread;
	private string port  = "";
	private int baudrate = 9600;
	private bool looping = true;

	private Queue outputQueue;
	private Queue inputQueue;

    public ConnectionHandler(string port)
    {
        this.port = port;
    }

	public void Start()
	{
        outputQueue = Queue.Synchronized( new Queue() );
		inputQueue  = Queue.Synchronized( new Queue() );

		thread = new Thread(ThreadLoop);
		thread.Start();
	}

	public void ThreadLoop()
    {
        // Opens the connection on the serial port
        stream = new SerialPort(port, baudrate);
		stream.ReadTimeout = 50;
        MonoBehaviour.print("Porta: " + port);
        stream.Open();
        MonoBehaviour.print("Porta deps open: " + port);
        string stringResult = "";
       
        // Looping
        while (IsLooping())
		{
			// Send to Arduino
			if (outputQueue.Count != 0){

				string command = (string) outputQueue.Dequeue();
				WriteToArduino(command);
			}

			// Read from Arduino
			char charResult = ReadFromArduino(stream.ReadTimeout);

			if (charResult != 'e')
			{
				if(charResult == '\n')
				{
					inputQueue.Enqueue(stringResult);
					stringResult = "";
				}
				else
				{
					stringResult += charResult;
				}
			}
		}

		stream.Close();
	}

	public void Send(string command)
	{
		outputQueue.Enqueue(command);
	}

	public string Get()
	{
		string command = "vazio";

		if (inputQueue.Count != 0)
		{
			command = (string) inputQueue.Dequeue();
		}

		return command;
	}

	private bool IsLooping ()
	{
		lock (this)
		{
			return looping;
		}
	}

	private void WriteToArduino(string message) {
		stream.WriteLine(message);
        stream.BaseStream.Flush();
	}

	private char ReadFromArduino (int timeout = 0) {
		stream.ReadTimeout = timeout;
		try
		{
			return (char) stream.ReadByte();
		}
		catch(TimeoutException){
			return 'e';
		}
	}

	public void Kill()
	{
		lock(this){
			looping = false;
		}

		if(thread.IsAlive)
		{
			thread.Abort();
		}
	}
}
