using System;

namespace Arithmetic.Code.Practice.Queue
{
    //简单的链表实现
    public class PracticeQueue1
    {
        public PracticeQueue1()
        {
            Queue<int> queue = new Queue<int>();
            for (int i = 0; i < 100; i++)
            {
                queue.Eneqeue(i);
            }

            while (queue.size>0)
            {
                Console.Write(queue.Deeqeue()+"\n");
            }
        }

        private class Queue<T>
        {
            private Node<T> firstData;
            private Node<T> endData;

            public int size;
            public void Eneqeue(T data)
            {
                Node<T> node = new Node<T>(data);
                if (IsEmpty())
                {
                    firstData = node;
                    endData = node;
                }
                else
                {
                    node.NextData = firstData;
                    firstData = node;
                }

                size++;
            }

            public T Deeqeue()
            {
                if (IsEmpty())
                {
                    return default(T);
                }

                Node<T> temp = firstData;
                T result = temp.Data;

                firstData = temp.NextData;
                size--;
                
                return result;
            }
            bool IsEmpty()
            {
                return size==0;
            }
            
        }
        
        private class Node<T>
        {
            public T Data;

            public Node<T> NextData=null;
            public Node(T info)
            {
                Data = info;
                NextData = null;
            }
        }
        
        
    }
}