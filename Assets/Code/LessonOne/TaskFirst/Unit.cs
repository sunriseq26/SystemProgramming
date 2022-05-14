using System;
using System.Collections;
using UnityEngine;

namespace LessonOne
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int _health;

        private bool _isFinish = true;
        private bool _isMax = false;
        private bool _isTime = false;

        private void Update()
        {
            ReceiveHealing();
        }

        private void ReceiveHealing()
        {
            if (_isFinish && !_isTime && _health < 100)
            {
                _isFinish = false;
                StartCoroutine(HealingCoroutine(0.5f, 5));
            }
            else if (_health > 100 && !_isMax)
            {
                _health = 100;
                _isMax = true;
            }
            
            Debug.Log(_health);
        }

        private IEnumerator HealingCoroutine(float seconds, int factor)
        {
            if (_isMax)
            {
                yield break;
            }

            StartCoroutine(Timer(3.0f));
            yield return new WaitForSeconds(seconds);
            _health += factor;
            _isFinish = true;
        }

        private IEnumerator Timer(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _isTime = true;
        }
    }
}