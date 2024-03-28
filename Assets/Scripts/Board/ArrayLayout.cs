[System.Serializable]
public class ArrayLayout 
{
    [System.Serializable]

    public struct RowData
    {
        public bool[] Rows;
    }

    public RowData[] RowDatas = new RowData[14];
}
