using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Stats;

public class Projectile : MonoBehaviour
{
    
    [HideInInspector] public Vector3 velocity;

    AudioSource audioSource;

    private float speed;
    
    void Start()
    {
        Debug.Log("Projectile Start Called!");
        speed = BossStats.Instance.projectileSpeed;

        StartCoroutine(Live());      
    }

    void Update()
    {
        transform.Translate(velocity * speed * Time.deltaTime);       
    }

    IEnumerator Live()
    {
        yield return new WaitForSeconds(BossStats.Instance.projectileLifetime);
        
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Debug.Log("Projectile hit the player");
            
            IKillable killable = other.gameObject.GetComponent<IKillable>();
            Debug.Assert(killable != null, "Projectile could not locate the player's IKillable component");

            if (killable != null)
            {
                killable.OnAttacked(BossStats.Instance.projectileDamage);
            }            
        }

        Destroy(gameObject);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }      
        audioSource.PlayOneShot(clip);
    }
}
