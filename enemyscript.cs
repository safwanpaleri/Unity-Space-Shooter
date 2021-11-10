using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
public class enemyscript : MonoBehaviour
{
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
        counter = Random.Range(minCount, maxCount);
    }
    private void Update()
    {
        shootmethod();
    }
    private void shootmethod()
    {
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            Fire();
            counter = Random.Range(minCount, maxCount);
        }
    }

    private void Fire()
    {
        GameObject lasers = Instantiate(laser, transform.position, Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(lasernote, Camera.main.transform.position, VolumeOfLaser);
        lasers.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -laserspeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damagescript damager = collision.gameObject.GetComponent<damagescript>();
        if (!damager) { return; }
        health -= damager.Getdamageint();
        damager.destruct();
        deathcode();
    }
    private void deathcode()
    {
       
        if (health <= 0)
        {
            FindObjectOfType<ScoreScript>().addscore(points);
            Destroy(gameObject);
            GameObject paticlevfx = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(deathnote, Camera.main.transform.position, VolumeOfDestruction);
            Destroy(paticlevfx, 1f);
        }
    }
   
}
