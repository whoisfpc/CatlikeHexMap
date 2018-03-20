using System;
using System.Collections.Generic;

namespace HexMap.Util
{
    public class PriorityQueue<T> where T : class
    {
        private const int DefaultInitalCapacity = 11;

        public int Count => count;

        private readonly Comparison<T> comparison;
        private T[] queue;
        private int count;


        public PriorityQueue(Comparison<T> comparison) : this(DefaultInitalCapacity, comparison)
        {
        }

        public PriorityQueue(int initialCapacity, Comparison<T> comparison)
        {
            if (initialCapacity < 1 || comparison == null)
            {
                throw new ArgumentException();
            }
            queue = new T[initialCapacity];
            this.comparison = comparison;
        }

        private const int MaxArraySize = int.MaxValue - 8;

        private void Grow(int minCapacity)
        {
            int oldCapacity = queue.Length;
            // Double size if small; else grow by 50%
            int newCapacity = oldCapacity + (oldCapacity < 64 ? oldCapacity + 2 : oldCapacity >> 1);

            // overflow-conscious code
            if (newCapacity - MaxArraySize > 0)
            {
                newCapacity = HugeCapacity(minCapacity);
            }

            T[] newQueue = new T[newCapacity];
            Array.Copy(queue, newQueue, oldCapacity);
            queue = newQueue;
        }

        private static int HugeCapacity(int minCapacity)
        {
            if (minCapacity < 0)
            {
                throw new OutOfMemoryException();
            }
            return minCapacity > MaxArraySize ? int.MaxValue : MaxArraySize;
        }

        public void Enqueue(T e)
        {
            if (e == null)
            {
                throw new NullReferenceException();
            }
            int i = count;
            if (i >= queue.Length)
            {
                Grow(i + 1);
            }
            count = i + 1;
            if (i == 0)
            {
                queue[0] = e;
            }
            else
            {
                SiftUp(i, e);
            }
        }

        public T Peek()
        {
            return count == 0 ? null : queue[0];
        }

        private void SiftUp(int k, T x)
        {
            while (k > 0)
            {
                int parent = (k - 1) >> 1;
                var e = queue[parent];
                if (comparison(x, e) >= 0)
                {
                    break;
                }
                queue[k] = e;
                k = parent;
            }
            queue[k] = x;
        }

        private void SiftDown(int k, T x)
        {
            int half = count >> 1;
            while (k < half)
            {
                int child = (k << 1) + 1;
                var c = queue[child];
                int right = child + 1;
                if (right < count && comparison(c, queue[right]) > 0)
                {
                    c = queue[child = right];
                }
                if (comparison(x, c) <= 0)
                {
                    break;
                }
                queue[k] = c;
                k = child;
            }
            queue[k] = x;
        }

        public T Dequeue()
        {
            if (count == 0)
            {
                return null;
            }
            int s = --count;
            var result = queue[0];
            var x = queue[s];
            queue[s] = null;
            if (s != 0)
            {
                SiftDown(0, x);
            }
            return result;
        }

        public void Clear()
        {
            for (int i = 0; i < count; i++)
            {
                queue[i] = null;
            }
            count = 0;
        }

        private bool Remove(T e)
        {
            int i = Array.IndexOf(queue, e);
            if (i < 0)
            {
                return false;
            }
            else
            {
                RemoveAt(i);
                return true;
            }
        }

        private T RemoveAt(int i)
        {
            int s = --count;
            if (s == i)
            {
                queue[i] = null;
            }
            else
            {
                var moved = queue[s];
                queue[s] = null;
                SiftDown(i, moved);
                if (queue[i] == moved)
                {
                    SiftUp(i, moved);
                    if (queue[i] != moved)
                    {
                        return moved;
                    }
                }
            }
            return null;
        }

        public void Change(T e)
        {
            if (Remove(e))
            {
                Enqueue(e);
            }
        }
    }
}
