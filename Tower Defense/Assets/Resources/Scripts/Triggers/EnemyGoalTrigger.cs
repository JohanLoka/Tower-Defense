using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Enemy")
            return;

        BaseEntity entity = other.gameObject.GetComponent<BaseEntity>();

        if (!entity)
            return;
            
        entity.OnGoalReached();
    }
}
