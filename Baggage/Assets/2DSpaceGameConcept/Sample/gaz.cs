using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gaz : MonoBehaviour
{
   


    public static int gaz1;
    public static int gaz2;

    //public GameObject Duman;

    private Rigidbody2D rb2D;
    private float thrust = 0.08f;

    public GameObject jet_left;
    public GameObject jet_right;

    private int duruyor;
    public static bool izin;

    public static bool oyunbitti;

    private void Awake()
    {
	    izin = true;
    }

    void Start()
    {
       
        rb2D = this.GetComponent<Rigidbody2D>();
        InvokeRepeating("HizHesapla", 0.2f, 0.1f);
    }

    void HizHesapla()
    {
        string temp = this.GetComponent<Rigidbody2D>().velocity.ToString();
        ////Debug.Log(temp);
        //temp = temp.Replace("(", "");
        //temp = temp.Replace(")", "");
        ////Debug.Log(temp);
        //string[] param1 = temp.Split(new string[] { "," }, StringSplitOptions.None);

        //_Global.phy_x = float.Parse(param1[0]);
        //_Global.phy_y = float.Parse(param1[1]);
    }

    void FixedUpdate()
    {
        int ne = 0;
      
        
            if (Input.GetKey(KeyCode.LeftArrow))
            {
  
                ne += 1;
                rb2D.AddForce(-transform.right * (thrust / 1.7f), ForceMode2D.Impulse);
                jet_right.SetActive(true);
	            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z+0.1f);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ne += 1;
                rb2D.AddForce(transform.right * (thrust / 1.7f), ForceMode2D.Impulse);

                jet_left.SetActive(true);
	            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - 0.1f);
            }
            if (ne == 2 || ne == 1)
            {
                rb2D.AddForce(transform.up * thrust, ForceMode2D.Impulse);
            }
            if (ne == 2)
            {
                jet_left.SetActive(true);
                jet_right.SetActive(true);
            }
            
            if (ne == 0)
            {
                jet_left.SetActive(false);
                jet_right.SetActive(false);  
            }
        
        
    }

    private void Update()
    {
	    //if (Input.GetKeyDown(KeyCode.LeftArrow))
	    //{
		//    gaz1=1;
	    //}
	    //if (Input.GetKeyUp(KeyCode.LeftArrow))
	    //{
		//    gaz1=0;
	    //}
	    //if (Input.GetKeyDown(KeyCode.RightArrow))
	    //{
		//    gaz2=1;
	    //}
	    //if (Input.GetKeyUp(KeyCode.RightArrow))
	    //{
		//    gaz2=0;
	    //}
    }
}
