using System.Runtime.InteropServices;
using MSLIMA.Serializer;
using UnityEngine;

namespace Samples.MSLIMA
{
    public class TestExample : MonoBehaviour
    {
        private void Awake()
        {
            Serializer.RegisterCustomType<TestStruct>((byte)'A');
        }

        private void Start()
        {
            TestStruct testStruct = new TestStruct
            {
                int_1 = 30,
                int_2 = 71,
                float_1 = 0.162f,
                bool_1 = true,
                string_1 = "ABCDSFD",
                vector3_1 = new Vector3(-23f, 62f, 26f),
                vector3_2 = new Vector3(1f, 7f, -15f),
                quaternion_1 = Quaternion.Euler(35f, 0f, 15f),
                enum_1 = Enum.C
            };

            CheckSize(testStruct);
            Serialize(testStruct);
        }

        public void Serialize(TestStruct testStruct)
        {
            byte[] bytes = TestStruct.Serialize(testStruct);

            Debug.Log(string.Format("Serialized Size : {0} bytes", bytes.Length));
            Debug.Log(JsonUtility.ToJson((TestStruct)TestStruct.Deserialize(bytes), true));
        }

        public void CheckSize(TestStruct testStruct)
        {
            Debug.Log(string.Format("Original Size : {0} bytes", Marshal.SizeOf(testStruct)));
            Debug.Log(JsonUtility.ToJson(testStruct, true));
        }
    }
}
