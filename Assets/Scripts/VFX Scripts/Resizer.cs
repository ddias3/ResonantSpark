using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resizer : MonoBehaviour
{
    public float resizeTime;
    public float resizeSpeed;
    private float timer;
    private Vector3 resizeVector;
    public GameObject electricity;
    // Start is called before the first frame update
    void Start()
    {
        timer = resizeTime;
        resizeVector = new Vector3(resizeSpeed, 0, resizeSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0) {
            this.gameObject.transform.localScale += resizeVector * Time.deltaTime;
            timer -= Time.deltaTime;
        } else
        {
            Destroy(electricity);
            Destroy(this.gameObject);
        }
        
    }
}
