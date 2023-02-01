using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        // -1한 이유는 마지막 루프를 생략해도 되기 때문.
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length); // 최소값, 최대값

            // i번째 원소를 랜덤 원소와 교체
            T tempItem = array[randomIndex]; // tempItem에 무작위로 선택한 랜덤 아이템을 저장
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }

}
