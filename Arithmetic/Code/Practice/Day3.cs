using System;
using System.Collections.Generic;
using System.Linq;
using Arithmetic.Code.Common;

public class Day3
{
    public  Day3()
    {

        List<string> list = new List<string>();
        long dt = DateTime.UtcNow.Ticks;
        string str = "";
       
        for (int i = 0; i < 10000000; i++)
        {
            list.Add(i.ToString());
            
        }
        long dt1 = DateTime.UtcNow.Ticks;
        Console.Write(dt1-dt+"/n");
        List<string> list1=new List<string>();
         dt = DateTime.UtcNow.Ticks;
    
         for (int i = 0; i < 10000000; i++)
         {
             str += i.ToString();
             list1.Add(str);
            
         }
       
         dt1 = DateTime.UtcNow.Ticks;
        Console.Write(dt1-dt);
    }
    

}