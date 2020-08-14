using System.Runtime.InteropServices;

namespace Hotels.Basic
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PriceRelation
    {
        public int R1, M1, R2, M2;
    }
}
