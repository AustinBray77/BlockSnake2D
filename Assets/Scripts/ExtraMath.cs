//Static class to store extra math functions
public static class ExtraMath
{
    //Binds and int to between 1 and -1
    public static int NormalizeInt(int num)
    {
        return num > 0 ?
            1 : num < 0 ?
                -1 : 0;
    }
}
