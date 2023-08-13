using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    public static BulletSpawner instance { get; private set; }

    private IObjectPool<BulletController> bulletPool;
    [SerializeField] private BulletController bulletPrefab;
   
    // Start is called before the first frame update


    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            bulletPool = new ObjectPool<BulletController>(() =>
            {
                BulletController bullet = Instantiate(bulletPrefab);
                bullet.InIt(KillObjectInPool);

                return bullet;
            }
                , note => note.gameObject.SetActive(true)
                , note => note.gameObject.SetActive(false)
                , note => Destroy(note.gameObject)
                , false, 100);

        }



    }

    public BulletController GetBullet()
    {
        return bulletPool.Get(); 
    }

    private void KillObjectInPool(BulletController bullet)
    {
        bulletPool.Release(bullet);
    }
}
