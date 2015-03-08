using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.Graph
{
    class Stack<T>
    {
        private T[] items;
        private int count;

        public Stack(int capacity)
        {
            items = new T[capacity];
            count = 0;
        }

        public void Push(T item)
        {
            if (items.Length == count)
                throw new Exception("Stack is full!");
            items[count++] = item;
        }

        public T Pop()
        {
            if (count == 0)
                throw new Exception("Stack is empty!");
            return items[--count];
        }

        public T Peek()
        {
            if (count == 0)
                throw new Exception("Stack is empty!");
            return items[count - 1];
        }

        public bool IsEmpty()
        {
            return count == 0;
        }

        public T[] GetAsArray()
        {
            T[] ret = new T[count];
            for (int i = 0; i < count; i++)
                ret[i] = items[i];
            return ret;
        } 

    }
}
