using System;
using UnityEngine;
using _Framework.Scripts.Extensions;

namespace _Framework.Scripts.Util
{
    public class MoveOnPath : MonoBehaviour
    {
        [SerializeField] private Transform[] path;
        [SerializeField] private float speed;
        [SerializeField] private float acceleration;

        private int index;
        private bool finished;

        public Transform[] Path
        {
            get { return path; }
            set { path = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        
        public float Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public event Action OnPathFinished = () => { };

        private void Update()
        {
            if (finished) return;
            
            Speed = Speed * (1 + Acceleration * Time.deltaTime);
            var moveBudget = Time.deltaTime * Speed;

            while (moveBudget > 0)
            {
                var currentPointPos = path[index];
                var currentDist = currentPointPos.position.Distance(transform.position);
                if (moveBudget > currentDist)
                {
                    moveBudget -= currentDist;
                    transform.position = currentPointPos.position;
                    index++;

                    if (index == path.Length)
                    {
                        finished = true;
                        OnPathFinished();
                        return;
                    }
                }
                else
                {
                    Vector3 toGo;
                    if (index > 1)
                    {
                        toGo = transform.position + (path[index].position - path[index - 1].position).normalized * moveBudget;
                    }
                    else
                    {
                        toGo = transform.position + (path[index].position - transform.position).normalized * moveBudget;
                    }

                    moveBudget = 0;
                    transform.position = toGo;
                }
            }
        }
    }
}