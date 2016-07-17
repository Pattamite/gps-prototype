using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GpsTester : MonoBehaviour 
{
	public Text GpsStatus;
	public Text UpdateDis;
	public Text Altitude;
	public Text Latitude;
	public Text Longtitude;
	public Text HoriAcc;
	public Text VertAcc;
	public Text TimeStamp;
	public Text MainStatus;
	public static float UpdateDistance = 10f;
	
	private enum State {User_off, User_on, Ini, On};
	private State current;
	private bool isRunning;
	private bool isUpdating;

	// Use this for initialization
	void Start () 
	{
		isRunning = false;
		isUpdating = false;
		current = State.User_off;
		print ("" + Input.location.isEnabledByUser);
	}
	
	// Update is called once per frame
	void Update () 
	{	
		UpdateDis.text = UpdateDistance + " m";
		if(!isRunning && !isUpdating)
		{
			isRunning = true;
			if(current == State.User_off) UserOff();
			else if(current == State.User_on) UserOn();
			else if(current == State.Ini) Initializing();
			else if(current == State.On) Operation();
		}
		//else print(":<");
	}
	
	void UserOff()
	{
		print("UserOff");
		MainStatus.text = "Location service is off.\n" + "Please open it.";
		GpsStatus.text = "Off";
		if(Input.location.isEnabledByUser == true) current = State.User_on;
		isRunning = false;
	}
	
	void UserOn()
	{
		print("UserOn");
		MainStatus.text = "Location service is on.\n";
		GpsStatus.text = "On";
		UserOn_1();
		
	}
	void UserOn_1()
	{
		print("UserOn_1");
		Input.location.Start(UpdateDistance, UpdateDistance);
		current = State.Ini;
		isRunning = false;
	}
	
	void Initializing()
	{
		print("Initializing");
		if(Input.location.status == LocationServiceStatus.Initializing)
		{
			print("Ini -> still ini");
			MainStatus.text = "Initializing";
			isRunning = false;
		}
		else if (Input.location.status == LocationServiceStatus.Running)
		{
			print("Ini -> running");
			MainStatus.text = "Initialize Successful.";
			current = State.On;
			isRunning = false;
		}
		else
		{
			print("Ini -> else");
			MainStatus.text = "Initialize Fail.";
			Input.location.Stop();
			UserOn_1();
		}
	}
	void waiting()
	{
		isRunning = false;
	}
	
	void Operation()
	{
		if(Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
		{
			MainStatus.text = "Running";
			float Alti = Input.location.lastData.altitude;
			float Lati = Input.location.lastData.latitude;
			float Longti = Input.location.lastData.longitude;
			float HoAc = Input.location.lastData.horizontalAccuracy;
			float VeAc = Input.location.lastData.verticalAccuracy;
			double TimeSt = Input.location.lastData.timestamp;
			TimeSt -= 1467763200;
			
			Altitude.text = Alti.ToString();
			Latitude.text = Lati.ToString();
			Longtitude.text = Longti.ToString();
			HoriAcc.text = HoAc.ToString();
			VertAcc.text = VeAc.ToString();
			TimeStamp.text = Mathf.Ceil((float)TimeSt).ToString();
			MainStatus.text = "pass";
			isRunning = false;
			
		}
		else
		{
			current = State.User_off;
			isRunning = false;
		}
	}
	
	public void Update(float value)
	{
		isUpdating = true;
		UpdateDistance = value;
		current = State.User_off;
		Input.location.Stop();
		isUpdating = false;
		isRunning = false;
		
	}
}
