using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace MyLib
{
    public static class Exception
    {
        #region[배열 범위초과 검사]
        public static bool IndexOutRange<T>(int x, int y, T[,] array)
        {
            if (x >= array.GetLength(0) || x < 0 || y >= array.GetLength(1) || y < 0)
                return false;
            return true;
        }

        public static bool IndexOutRange<T>(Vector2Int v, T[,] array)
        {
            return IndexOutRange<T>(v.x, v.y, array);
        }

        public static bool IndexOutRange<T>(int a, List<T> array)
        {
            if (array == null || a >= array.Count || a < 0)
                return false;
            return true;
        }

        public static bool IndexOutRange<T>(int a, T[] array)
        {
            if (a >= array.GetLength(0) || a < 0)
                return false;
            return true;
        }
        #endregion
    }

    public static class AreaCheck
    {
        #region[범위 내부인지 검사]
        public static bool RectIn(Vector2 pos,Rect rect)
        {
            if (rect.x > pos.x || rect.x + rect.width < pos.x || rect.y < pos.y || rect.y - rect.height > pos.y)
                return false;
            return true;
        }

        public static bool RectIn(Vector2 pos, RectInt rect)
        {
            return RectIn(pos, new Rect(rect.x, rect.y, rect.width, rect.height));
        }
        #endregion
    }

    public static class Algorithm
    {
        #region[Next_Permutation]
        public static bool Next_Permutation<T>(List<T> list) where T : IComparable
        {
            Action<int, int> Swap = (int idx1, int idx2) => { T temp = list[idx1]; list[idx1] = list[idx2]; list[idx2] = temp; };
            int a = 0, b = 0, p = 0; //p : pivot
            for (int i = list.Count - 2; i >= 0; --i)
                if (list[i].CompareTo(list[i + 1]) < 0)
                {
                    a = i;
                    p = i + 1;

                    for (int j = list.Count - 1; j >= 0; --j)
                        if (list[a].CompareTo(list[j]) < 0)
                        {
                            b = j;
                            break;
                        }

                    Swap(a, b);

                    for (int j = 0; j < (list.Count - p) / 2; j++)
                        Swap(j + p, list.Count - j - 1);

                    return true;
                }
            // 이미 순서대로 정렬되어 있음 => 역순으로 뒤집
            for (int i = 0; i < list.Count / 2; i++)
                Swap(i, list.Count - i - 1);
            return false;
        }
        #endregion

        #region[Swap]
        public static void Swap<T>(ref T a,ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        #endregion

        #region[Shuffle]
        public static void Shuffle<T>(ref List<T> list)
        {
            //list에 있는 데이터를 섞는다.
            //랜덤하게 인데스(a,b) 두개를 정하고
            //해당 인덱스에 해당하는 값을 교환한다.
            //해당 과정을 list길이*10번 만큼 반복한다.
            for (int i = 0; i < list.Count * 10; i++)
            {
                int a = UnityEngine.Random.Range(0, list.Count);
                int b = UnityEngine.Random.Range(0, list.Count);

                T temp = list[a];
                list[a] = list[b];
                list[b] = temp;
            }
        }
        #endregion

        #region[CreateRandomList]
        //1~N중에서 중복하지 않는 m개를 뽑는다.
        public static List<int> CreateRandomList(int n,int m)
        {
            int[] tree = new int[n+1];
            List<int> temp = new List<int>();

            int Sum(int i)
            {
                int ans = 0;
                while (i > 0)
                {
                    ans += tree[i];
                    i -= (i & -i);
                }
                return ans;
            }

            void Update(int i, int num)
            {
                while (i <= n)
                {
                    tree[i] += num;
                    i += (i & -i);
                }
            }

            for (int i = 1; i <= n; i++)
                Update(i, 1);

            for (int i = n; i > n - m; i--)
            {
                int rand = UnityEngine.Random.Range(0, i);

                int left = 1;
                int right = i;
                while (left < right)
                {
                    int mid = (left + right) / 2;
                    if (Sum(mid) >= rand)
                        right = mid;
                    else
                        left = mid + 1;
                }
                temp.Add(right);
                Update(right, -1);
            }

            return temp;
        }
        #endregion
    }

    public static class Calculator
    {
        #region[CreateRandomList]

        //블록의 크기를 고려해서 x,y에 해당하는 블록의 위치를 구한다.
        public static Vector2 CalculateHexagonPos(float blockWidth, float blockHeight, int x, int y)
        {
            float resultY = y * blockHeight * 0.5f;
            float resultX = x * blockWidth;
            if (y % 2 != 1)
                resultX += blockWidth * 0.5f;

            return new Vector2(resultX, resultY);
        }
        #endregion

    }

    public static class Json
    {
        #region[JSON 유틸]
        public static string ObjectToJson(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public static T JsonToOject<T>(string jsonData)
        {
            return JsonUtility.FromJson<T>(jsonData);
        }

        public static void CreateJsonFile(string createPath, string fileName, string jsonData)
        {
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
        }

        public static T LoadJsonFile<T>(string loadPath, string fileName)
        {
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            string jsonData = Encoding.UTF8.GetString(data);
            return JsonUtility.FromJson<T>(jsonData);
        }

        [System.Serializable]
        public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
        {
            [SerializeField]
            List<TKey> keys;
            [SerializeField]
            List<TValue> values;

            Dictionary<TKey, TValue> target;
            public Dictionary<TKey, TValue> ToDictionary() { return target; }

            public Serialization(Dictionary<TKey, TValue> target)
            {
                this.target = target;
            }

            public void OnBeforeSerialize()
            {
                keys = new List<TKey>(target.Keys);
                values = new List<TValue>(target.Values);
            }

            public void OnAfterDeserialize()
            {
                var count = Mathf.Min(keys.Count, values.Count);
                target = new Dictionary<TKey, TValue>(count);
                for (var i = 0; i < count; ++i)
                {
                    target.Add(keys[i], values[i]);
                }
            }
        }

        [System.Serializable]
        public class Serialization<T>
        {
            [SerializeField]
            List<T> target;
            public List<T> ToList() { return target; }

            public Serialization(List<T> target)
            {
                this.target = target;
            }
        }
        #endregion
    }

    public static class Action2D
    {
        #region[MoveTo]
        public static IEnumerator MoveTo(Transform target, Vector3 to, float duration)
        {
            Vector2 from = target.position;

            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                elapsed += Time.smoothDeltaTime;
                target.position =
                    Vector2.Lerp(from, to, elapsed / duration);

                yield return null;
            }

            target.position = to;

            yield break;
        }
        #endregion
    }
}