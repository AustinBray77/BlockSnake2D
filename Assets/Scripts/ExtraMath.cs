public static class ExtraMath
{
    public static int NormalizeInt(int num)
    {
        return num > 0 ?
            1 : num < 0 ?
                -1 : 0;
    }
}
