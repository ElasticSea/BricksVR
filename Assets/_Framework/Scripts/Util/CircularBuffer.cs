using System;
using System.Collections;
using System.Collections.Generic;

namespace _Framework.Scripts.Util
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] data;
        private int head;
        private int nextInsert;

        public CircularBuffer(int size)
        {
            if (size < 1)
                throw new Exception();
            data = new T[size];
            head = -size;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (head < 0)
            {
                for (var i = 0; i < nextInsert; i++)
                    yield return data[i];
            }
            else
            {
                for (var i = 0; i < data.Length; i++)
                    yield return data[(nextInsert + i) % data.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T t)
        {
            data[nextInsert] = t;
            nextInsert = (nextInsert + 1) % data.Length;
            if (head < 0)
                head++;
        }
    }
}