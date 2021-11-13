// unity headers
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    //caching variables
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
        //function for resticting player from beyound some point for better gameplay
        minandmax();
    }
   

    // Update is called once per frame
    void Update()
    {
        //function responsible for movement of the script holder.
        moveplayer();
        //function responsible for shooting 
        shootenemy();
    }
    
    private void shootenemy()
    {
        //if the "space" bar is being pressed, then fire continously,
        if (Input.GetKeyDown(KeyCode.Space))
        {
           firepower= StartCoroutine(firecontinously());
        }
        //if the "spacebar" is released, then stop firing
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(firepower);
        }
    }
    //function responsible for movement of player/script holder.
    private void moveplayer()
    {
        //tweaking speed of the movement accoording to the need of gameplay.
        //tweak for horizontal movement
        var movex = Input.GetAxis("Horizontal") * Time.deltaTime * Gamespeed ;
        var newXpos = transform.position.x + movex;
        //twaek for vertical movement
        var movey = Input.GetAxis("Vertical") * Time.deltaTime * Gamespeed;
        var newYpos = transform.position.y + movey; 
        
        transform.position = new Vector2(Mathf.Clamp(newXpos,minX,maxX),Mathf.Clamp(newYpos,minY,maxY));
    }
    
    //restricing player movement beyound some point for better gameplay.
    private void minandmax()
    {
        Camera gamecamera = Camera.main;
        minX = gamecamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        maxX = gamecamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        minY = gamecamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        maxY = gamecamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - (padding * 5);
    }
    
    //coroutine for firing continously
    IEnumerator firecontinously()
    {
        //running for infinte times while coroutine runs.
        while (true)
        {
            //function for spawing object,vfx etc.
            laserfire();
            //time delay between shots.
            yield return new WaitForSeconds(lasertime);
        }
    }
    
    //function for spawning shooting elements
   private void laserfire()
    {
        GameObject lasernew = Instantiate(laser, transform.position, Quaternion.identity) as GameObject;
        lasernew.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserspeed);
        AudioSource.PlayClipAtPoint(lasernote, Camera.main.transform.position, VolumeOfLaser);
        Destroy(lasernew, 10f);
    }
    
    //Collison detection
    //code responsible for taking damages.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //lasers(bullets) are attached with damagescript, and cached here for easy use
        damagescript damager = collision.gameObject.GetComponent<damagescript>();
        //if collided with other items do nothing, else
        if (!damager) { return; }
        //decrease health
        Health -= damager.Getdamageint();
        damager.destruct();
        //and if health becomes 0, then destroy the ship, and play vfx and sound
        if (Health <= 0)
        {
            Destroy(gameObject);
            GameObject paticlevfx = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(deathnote, Camera.main.transform.position, volumeofDestruction);
            Destroy(paticlevfx,1f);
            FindObjectOfType<SceneLoaderScript>().LoadGameOverScreen();
        }
    }
    
    //returns health varibale to another script for other purposes like UI.
    public int givehealth()
    {
        return Health;
    }
}
