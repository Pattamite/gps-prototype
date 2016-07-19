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
	public int waitTime = 20;
	
	
	//private bool isRunning;
	//private bool isUpdating;
	private enum State {Ini_pass, Ini_failed, Ini_not};
	private State current;
	
	void Start()
	{
		
		StartCoroutine("Operation");
	}
	
	void Update()
	{
		
	}
	
	IEnumerator Operation()
	{
		while (true)
		{
			current = State.Ini_failed;
			yield return StartCoroutine("UserOff");
			Debug.Log("Method : UserOff DONE");
			yield return StartCoroutine("UserOn");
			Debug.Log("Method : UserOn DONE");
			yield return StartCoroutine("Initializing");
			Debug.Log("Method : Initializing DONE");
			if(current == State.Ini_pass)
			{
				Debug.Log("Ini Successful");
				yield return StartCoroutine("Working");
			}
			else
			{
				Debug.LogWarning("Ini Failed");
			}
			yield return null;
		}
		
	}
	
	IEnumerator UserOff()
	{
		Debug.Log("Method : UseOff START");
		UpdateDis.text = UpdateDistance + " m";
		GpsStatus.text = "Off";
		MainStatus.text = "Location service is off.\n" + "Please open it.";
		while (Input.location.isEnabledByUser == false)
        {
            yield return null;
            //Debug.Log("Waiting");
        }
		yield return null;
	}
	
	IEnumerator UserOn()
	{
		Debug.Log("Method : UserOn START");
		MainStatus.text = "Location service is on.";
		GpsStatus.text = "On";
		Input.location.Start(UpdateDistance, UpdateDistance);
		yield return null;
	}
	
	IEnumerator Initializing()
	{
		Debug.Log("Method : Initializing START");
		MainStatus.text = "Initializing";
		
		int i = 0;
		while(i <= waitTime && Input.location.status == LocationServiceStatus.Initializing)
		{
			i++;
			MainStatus.text = "Initializing " + (waitTime-i).ToString();
			yield return new WaitForSeconds(1.0f);
		}
		
		if(Input.location.status == LocationServiceStatus.Running)
		{
			current = State.Ini_pass;
		}
		else
        {
            current = State.Ini_failed;
        }
		yield return null;
	}
	
	IEnumerator Working()
	{
		Debug.Log("Method : Working START");
		while (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
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
			
			yield return new WaitForSeconds(1.0f);
		}
		current = State.Ini_not;
		Input.location.Stop();
		yield return null;
	}
	
	public void ChangeUpdateDistance(float value)
	{
		StopAllCoroutines();
		UpdateDistance = value;
		StartCoroutine("Operation");
	}
}
