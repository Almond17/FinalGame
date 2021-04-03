using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            //if (controller.health < controller.maxHealth)
            //{
                controller.addcog();
                Destroy(gameObject);

                //controller.PlaySound(collectedClip);
            //}
        }

    }
}