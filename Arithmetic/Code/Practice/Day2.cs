using System;
using System.Collections.Generic;
using System.Linq;
using Arithmetic.Code.Common;

public class Day2
{
    public  Day2()
    {

        int[] nums = new[]
        {
            3, 55, 32, 567, 32, 24, 6, 23, 64, 234, 234, 67, 43, 24, 124, 4

        };
        // HeapSort(nums);//堆排序
        nums=BucketSort(nums);//桶排序
        foreach (var VARIABLE in nums)
        {
            Console.Write(VARIABLE+"\n");
        }
    }
    
    public int[] BucketSort(int[] nums)
    {
        int maxNum = nums.Max();
        int bucketSize = maxNum / nums.Length + 1;
        List<int>[] buckets = new List<int>[bucketSize];
        for (int i = 0; i < bucketSize; i++)
        {
            buckets[i] = new List<int>();
        }
        foreach (int num in nums)
        {
            int index = num / bucketSize;
            buckets[index].Add(num);
        }
        List<int> result = new List<int>();
        foreach (List<int> bucket in buckets)
        {
            result.AddRange(bucket.OrderBy(x => x));
        }
        return result.ToArray();
    }

    
    void Heapify(int[] nums, int n, int i)
    {
        int largest = i;
        int left = 2 * i + 1;
        int right = 2 * i + 2;
        if (left < n && nums[left] > nums[largest])
        {
            largest = left;
        }
        if (right < n && nums[right] > nums[largest])
        {
            largest = right;
        }
        if (largest != i)
        {
            int temp = nums[i];
            nums[i] = nums[largest];
            nums[largest] = temp;
            Heapify(nums, n, largest);
        }
    }
    public int[] HeapSort(int[] nums)
    {

        int n = nums.Length;
        for (int i = n / 2 - 1; i >= 0; i--)
        {
            Heapify(nums, n, i);
        }
        for (int i = n - 1; i > 0; i--)
        {
            int temp = nums[0];
            nums[0] = nums[i];
            nums[i] = temp;
            Heapify(nums, i, 0);
        }
        return nums;
    }

}