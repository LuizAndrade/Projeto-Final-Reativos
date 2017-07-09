using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
	private ConnectionHandler con;
	private string oldId = "";
	public Transform blueEyesDragon;
	public Transform darkMagician;
	private int xPos = 0;
	private float speed = 5;
	private bool summoned = false;
    private string port = "";
    private bool showGUI = true;

    void OnGUI()
    {
        if (showGUI)
        {
            GUI.contentColor = Color.yellow;
            GUI.Label(new Rect((Screen.width / 2) - 50, (Screen.height / 2) - 10, 100, 20), "Digite a Serial");
            port = GUI.TextField(new Rect((Screen.width / 2) - 100, (Screen.height / 2) + 20, 200, 20), port, 25);

            if (GUI.Button(new Rect((Screen.width / 2) - 25, (Screen.height / 2) + 40, 50, 30), "Ok"))
            {
                StartConnection();
                showGUI = false;
            }
        }
    }

	void Update()
	{
        if (con != null)
        {
            SummonMonster(con.Get());

            if (summoned)
            {
                MoveCamera();
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector3.right * Time.deltaTime * speed * -1);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * Time.deltaTime * speed);
            }
        }
	}

	private void StartConnection()
	{
		con = new ConnectionHandler(port);
		con.Start();
	}

	private void SummonMonster(string monsterId)
	{
		if(monsterId != oldId && monsterId != "vazio")
		{
			Vector3 pos;

			switch (monsterId)
			{
				case "2297012136":
					pos = new Vector3(10 * xPos, 1.5f, 1);
					Instantiate(blueEyesDragon, pos, Quaternion.Euler(0, -80, -15));
					oldId = monsterId;
					break;
				case "1099148229":
					pos = new Vector3(10 * xPos, 0, 0);
					Instantiate(darkMagician, pos, Quaternion.Euler(0, 180, 0));
					oldId = monsterId;
					break;
			}

			xPos++;
			summoned = true;
		}
	}

	private void MoveCamera()
	{
		Vector3 targetPos = new Vector3(10 * (xPos - 1), 0, 0);
		float localSpeed = 3;

		if (transform.position.x <= targetPos.x)
		{
			transform.Translate(Vector3.right * Time.deltaTime * localSpeed);
		}
		else
		{
			summoned = false;
		}
	}

	public void OnApplicationQuit()
	{
        if(con != null)
		    con.Kill();
	}
}
