using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [Header("Player settings")]
    [Range(5f,15f)] [SerializeField] float Gamespeed = 10f;
    float maxX;
    float minX;
    float maxY;
    float minY;
    [SerializeField] float padding = 0.5f;
    [SerializeField] int Health = 200;

    [Header("Laser options")]
    [SerializeField] GameObject laser;
    [SerializeField] float laserspeed = 20f;
    [SerializeField] float lasertime = 0.5f;
    Coroutine firepower;

    [Header("VFX")]
    [SerializeField] GameObject explosion;
    [Range(0,1)] [SerializeField] float VolumeOfLaser = 0.02f;
    [SerializeField] AudioClip lasernote;
    [Range(0, 1)] [SerializeField] float volumeofDestruction = 1f;
    [SerializeField] AudioClip deathnote;
    // Start is called before the first frame update
    void Start()
    {
        minandmax();
    }
   

    // Update is called once per frame
    void Update()
    {
        moveplayer();
        shootenemy();
    }
    private void shootenemy()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           firepower= StartCoroutine(firecontinously());
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(firepower);
        }
    }
    private void moveplayer()
    {
        var movex = Input.GetAxis("Horizontal") * Time.deltaTime * Gamespeed ;
        var newXpos = transform.position.x + movex;
        var movey = Input.GetAxis("Vertical") * Time.deltaTime * Gamespeed;
        var newYpos = transform.position.y + movey; 
        transform.position = new Vector2(Mathf.Clamp(newXpos,minX,maxX),Mathf.Clamp(newYpos,minY,maxY));
    }
    private void minandmax()
    {
        Camera gamecamera = Camera.main;
        minX = gamecamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        maxX = gamecamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        minY = gamecamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        maxY = gamecamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - (padding * 5);
    }
    IEnumerator firecontinously()
    {
        while (true)
        {
            laserfire();
            yield return new WaitForSeconds(lasertime);
        }
    
    }
   private void laserfire()
    {
        GameObject lasernew = Instantiate(laser, transform.position, Quaternion.identity) as GameObject;
        lasernew.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserspeed);
        AudioSource.PlayClipAtPoint(lasernote, Camera.main.transform.position, VolumeOfLaser);
        // Destroy(lasernew, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damagescript damager = collision.gameObject.GetComponent<damagescript>();
        if (!damager) { return; }
        Health -= damager.Getdamageint();
        damager.destruct();
        if (Health <= 0)
        {
            Destroy(gameObject);
            GameObject paticlevfx = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(deathnote, Camera.main.transform.position, volumeofDestruction);
            Destroy(paticlevfx,1f);
            FindObjectOfType<SceneLoaderScript>().LoadGameOverScreen();
        }
    }
    public int givehealth()
    {
        return Health;
    }
}
