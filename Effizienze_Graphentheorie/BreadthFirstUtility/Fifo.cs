using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.BreadthFirstUtility
{
    class Fifo<T>
    {
        T[] items;
        int start;
        int end;

        public Fifo(int capacity){
            items = new T[capacity + 1];
            start = 0;
            end = 0;
        }

        public void AddItem(T item){
            if ((end + 1) % items.Length == start)
                throw new Exception("Fifo is full!");
            items[end] = item;
            end = (end + 1) % items.Length;
        }

        public T GetItem()
        {
            if (start == end)
                throw new Exception("Fifo is empty!");
            T ret = items[start];
            start = (start + 1) % items.Length;
            return ret;
        }

        public T Peek(){
            if(start == end)
                throw new Exception("Fifo is empty!");
            return items[start];
        }

        public bool IsEmpty()
        {
            return start == end;
        }

        public T[] ToArray()
        {
            T[] ret;
            if(start > end)
                ret =  new T[items.Length - (start - end)];
            else
                ret =  new T[end - start];

            for (int i = start, count = 0; i != end; i = (i + 1) % items.Length)
            {
                ret[count] = items[i];
                count++;
            }
            return ret;
        }
    }
}
