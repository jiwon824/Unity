using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        // -1�� ������ ������ ������ �����ص� �Ǳ� ����.
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length); // �ּҰ�, �ִ밪

            // i��° ���Ҹ� ���� ���ҿ� ��ü
            T tempItem = array[randomIndex]; // tempItem�� �������� ������ ���� �������� ����
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }

}
