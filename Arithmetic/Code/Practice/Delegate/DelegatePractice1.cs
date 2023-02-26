using System;

namespace Arithmetic.Code.Practice.Delegate
{
    public class DelegatePractice1
    {
        public delegate int walk(int x,int y);

        public event walk walkFun;

        public DelegatePractice1()
        {
            // walkFun += invokeWalk;
            findTarget();
            TestDelegataFun(delegate(int x, int y)
            {
                return x;
            });
        }

        public void TestDelegataFun(walk functionTest)
        {
            
        }
        public void invokeWalk()
        {
            Console.Write("invokle Walk");
        }

        public void findTarget()
        {
            walkFun?.Invoke();
        }
    }
    
    
}