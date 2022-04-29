using System;
using System.Collections;
using UnityEngine;

namespace LessonOne
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int _health;

        private bool _isFinish = true;
        private bool _isMax;

        private void Update()
        {
            ReceiveHealing();
        }

        private void ReceiveHealing()
        {
            if (_isFinish && _health < 100)
            {
                _isFinish = false;
                StartCoroutine(HealingCoroutine(0.5f, 5));
            }
            else if (_health > 100 && !_isMax)
            {
                _health = 100;
                _isMax = true;
            }
        }

        private IEnumerator HealingCoroutine(float seconds, int factor)
        {
            yield return new WaitForSeconds(seconds);
            _health += factor;
            _isFinish = true;
        }
    }
}