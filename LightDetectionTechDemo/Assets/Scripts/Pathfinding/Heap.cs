using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{

    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        // Grid size x grid size y 
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        // Every item should keep their own index in the heap
        // Also which item has the higher index in the heap
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        // Take the item at the end of the heap and put it at the first slot
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T _item)
    {
        return Equals(items[_item.HeapIndex], _item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }


    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // Check if the item has at least one child (one on the left)
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                // Check if the item also has a child on the right
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) // if it has a higher priority
                    {
                        swapIndex = childIndexRight;
                    }
                }

                // Check if the parent has a lower priority
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    // Swap them
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    // Sort the heap
    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int temp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = temp;

    }
}


public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}