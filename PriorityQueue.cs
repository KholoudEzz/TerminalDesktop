using System.Collections;

public class PriorityQueue<T> : IEnumerable<T>
{
    private List<T> items = new List<T>();
    private IComparer<T> comparer;

    public PriorityQueue(IComparer<T> comparer)
    {
        this.comparer = comparer;
    }

    public void Enqueue(T item)
    {
        items.Add(item);
        items.Sort(comparer);
    }

    public T Dequeue()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T item = items[0];
        items.RemoveAt(0);
        return item;
    }

    public int Count
    {
        get { return items.Count; }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
