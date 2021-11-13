//unity headers
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

public class enemyscript : MonoBehaviour
{
    //creatingvariables.
    [Header("Enemy")]
    [SerializeField] float health = 100f;
    [SerializeField] float counter;
    [SerializeField] float maxCount = 3f;
    [SerializeField] float minCount = 0.2f;
    [SerializeField] int points = 150;
    
    [Header("Laser")]
    [SerializeField] GameObject laser;
    [SerializeField] float laserspeed = 20f;
    
    [Header("vfx")]
    [SerializeField] GameObject explosion;
    [Range(0, 1)] [SerializeField] float VolumeOfDestruction = 1f;
    [SerializeField] AudioClip deathnote;
    [Range(0, 1)] [SerializeField] float VolumeOfLaser = 0.02f;
    [SerializeField] AudioClip lasernote;
    SceneLoaderScript sceneload;
    
    private void Start()
    {
        //variable for the time delay between the shots of enemy.
        //random for better gameplay.
        counter = Random.Range(minCount, maxCount);
    }
    
    private void Update()
    {
        // function for counter attack by enemy
        shootmethod();
    }
    
    
    private void shootmethod()
    {
        // a simple timer
        counter -= Time.deltaTime;
        //if timer completes
        if (counter <= 0)
        {
            //shoot and reset timer.
            //function for shooting
            Fire();
            counter = Random.Range(minCount, maxCount);
        }
    }
    
    //funciton for spawning objects,vfx etc.
    private void Fire()
    {
        GameObject lasers = Instantiate(laser, transform.position, Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(lasernote, Camera.main.transform.position, VolumeOfLaser);
        lasers.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -laserspeed);
    }
    
    //function for collision detection and taking damages
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //objects have a damagescript attached to them
        //caching collion object to a variable, for easy use.
        damagescript damager = collision.gameObject.GetComponent<damagescript>();
        //if the collided object doesn't have damagescript then do nothing and return
        if (!damager) { return; }
        //else decrease health and destroy the collided object
        health -= damager.Getdamageint();
        damager.destruct();
        //function responsible actions realted to death of enemy.
        deathcode();
    }
    
    //function for spawning death vfx, sfx etc.
    private void deathcode()
    {
        // if health reaches to 0
        if (health <= 0)
        {
            // Increase the score as this was a enemy who got defeated,
            FindObjectOfType<ScoreScript>().addscore(points);
            
            Destroy(gameObject);
            GameObject paticlevfx = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(deathnote, Camera.main.transform.position, VolumeOfDestruction);
            Destroy(paticlevfx, 1f);
        }
    }
   
}
