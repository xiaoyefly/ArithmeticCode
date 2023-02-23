using System;
using Arithmetic.Code.Common;

public class Day1
{
    public  Day1()
    {
        Console.Write("Day1:\n");
        var node = new ValueNode(6);
        node.Value = 6;

        var nextNode = new ValueNode(4);
        node.NextNode = nextNode;

        var curNode = node;
        while (curNode!=null)
        {
            Console.Write(node.Value+"\n");
            curNode = curNode.NextNode;

        }

        // Console.Write(A);
    }
}