using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movment Settings")]
    [SerializeField] private float limitValue;

    private bool winState = false;

    private Animator pAnim;
    private SplineFollower playerFollower;

    private void Awake()
    {
        playerFollower = GetComponent<SplineFollower>();
        pAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacles")
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void MovePlayer()
    {
        if (Input.GetMouseButton(0) && winState == false)
        {
            float halfScreen = Screen.width / 2;
            float xPos = (Input.mousePosition.x - halfScreen) / halfScreen;
            float finalXpos = Mathf.Clamp(xPos * limitValue, -limitValue, limitValue);
            playerFollower.motion.offset = new Vector3(finalXpos, 0, 0); 
        }

    }

    public void Win()
    {
        winState = true;
        pAnim.SetTrigger("Win");
        playerFollower.followSpeed = 0f;
        Invoke("ReloadScene", 6f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
  
    public void Jump()
    {
        pAnim.SetTrigger("JumpStart");
    }

    public void Land()
    {
        pAnim.SetTrigger("JumpEnd");
        Debug.Log("Works");
    }
}
