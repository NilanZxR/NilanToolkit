namespace NilanToolkit {
public static class ArrayUtil {

    public static T[] Transit<T>(T[] arr, int delta) {
        delta = delta % arr.Length;
        if (delta < 0) delta = arr.Length + delta;
        var ret = new T[arr.Length];
        for (int i = 0; i < ret.Length; i++) {
            ret[i] = arr[(i + delta) % arr.Length];
        }

        return ret;
    }

}
}