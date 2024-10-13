public class XTextComparer : IComparer<XText>
{
    public int Compare(XText x1, XText x2)
    {
        return x2.x.CompareTo(x1.x);
    }
}