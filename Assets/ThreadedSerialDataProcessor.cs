using System;
using System.Text;
using System.IO.Ports;
using UnityEngine;
using Leap.Unity;

public class ThreadedSerialDataProcessor : MonoBehaviour {
  //Default serial port name; unused since we just grab the last COM port anyway
  string portName = "COM8";
  SerialPort _serialPort;


  //Define our thread-safe Queue for getting data from the COM port reading thread to the main thread
  ProduceConsumeBuffer<string> SerialData = new ProduceConsumeBuffer<string>(128);

  //Misc. Utility Objects
  int curIndex = 0;
  byte[] SerialBuffer = new byte[300];
  byte[] buffer = new byte[4096];
  Action kickoffRead = null;

  void Start() {
    // Defaults to last port in list (most chance to be our controller receiver's port)
    string[] portNames = SerialPort.GetPortNames();
    portName = portNames[portNames.Length - 1];

    //Have to add the prefix on Windows; probably won't work on any other platform
    Debug.Log(@"\\.\" + portName);
    _serialPort = new SerialPort(@"\\.\" + portName, 115200);
    _serialPort.ReadTimeout = 1;
    _serialPort.DataBits = 8;
    _serialPort.Parity = Parity.None;
    _serialPort.StopBits = StopBits.One;
    _serialPort.WriteTimeout = 1;

    //These had no effect
    //_serialPort.DtrEnable = true;
    //_serialPort.RtsEnable = true;

    _serialPort.Open();
    if (_serialPort.IsOpen) {
      Debug.Log("Opened " + portName);

      //Flush the data that is currently sitting in the Serial Port
      _serialPort.DiscardInBuffer();

      //Begin reading from the serial port asynchronously in another thread
      kickoffRead = delegate {
        while (_serialPort.IsOpen) {
          _serialPort.BaseStream.BeginRead(buffer, 0, buffer.Length,
          delegate (IAsyncResult ar) {
            try {
              int actualLength = _serialPort.BaseStream.EndRead(ar);
              byte[] received = new byte[actualLength];
              Buffer.BlockCopy(buffer, 0, received, 0, actualLength);
              receiveData(received);
            } catch (TimeoutException) {
              //Timeout exceptions occur every time you finish reading data
            } catch (InvalidOperationException) {
              //InvalidOperation exceptions occur every time I close the COM port
            } catch (Exception exc) {
              //Something new?
              Debug.Log(exc);
            }
          }, null);
        }
      };
      kickoffRead.BeginInvoke(null, null);
    }
  }

  //Every time we receive a block of raw data off the byte stream
  //Attempt to group it into complete packets and ship them off to the main thread
  void receiveData(byte[] received) {
    for (int i = 0; i < received.Length; i++) {
      //If we hit our two stop bytes (\ and n (13 and 10))
      if (curIndex != 0 && received[i] == 10 && SerialBuffer[curIndex - 1] == 13) {
        string thing = Encoding.ASCII.GetString(SerialBuffer, 0, curIndex-1);
        SerialData.TryEnqueue(ref thing);

        //Clear the data we currently have in our buffer
        for (int j = 0; j <= curIndex; j++) {
          SerialBuffer[j] = 0;
        }
        curIndex = 0;
      } else {
        //Add the current byte to our running buffer of bytes
        if (curIndex < SerialBuffer.Length) {
          SerialBuffer[curIndex++] = received[i];
        } else {
          string error = "Buffer Overflow!";
          SerialData.TryEnqueue(ref error);
        }
      }
    }
  }

  //During the Unity frame (on the main thread) dequeue all of the packets we got
  void Update() {
    string data;
    while (SerialData.TryDequeue(out data)) {
      Debug.Log(int.Parse(data));
    }
  }

  //Kill our COM port thread and our COM port
  void OnApplicationQuit() {
    try {
      kickoffRead.EndInvoke(null);
    } catch (System.Runtime.Remoting.RemotingException exc) {
      Debug.Log("Serial Polling Thread Shutting down\n" + exc.ToString());
    } catch (Exception exc) {
      Debug.Log(exc.ToString());
    }
    _serialPort.Close();
    if (!_serialPort.IsOpen) {
      Debug.Log("Closed " + portName);
    } else {
      Debug.Log("Couldn't close " + portName);
    }
  }
}