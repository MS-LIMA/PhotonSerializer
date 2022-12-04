using MSLIMA.Serializer;
using UnityEngine;

namespace Samples.MSLIMA
{
    [System.Serializable]
    public struct TestStruct
    {
        public int int_1;
        public int int_2;

        public float float_1;

        public bool bool_1;
        public string string_1;

        public Vector3 vector3_1;
        public Vector3 vector3_2;

        public Quaternion quaternion_1;

        public Enum enum_1;

        public static byte[] Serialize(object customObject)
        {
            TestStruct o = (TestStruct)customObject;
            byte[] bytes = new byte[0];

            Serializer.Serialize(o.int_1, ref bytes);
            Serializer.Serialize(o.int_2, ref bytes);
            Serializer.Serialize(o.float_1, ref bytes);
            Serializer.Serialize(o.bool_1, ref bytes);
            Serializer.Serialize(o.string_1, ref bytes);
            Serializer.Serialize(o.vector3_1, ref bytes);
            Serializer.Serialize(o.vector3_2, ref bytes);
            Serializer.Serialize(o.quaternion_1, ref bytes);
            Serializer.Serialize((int)o.enum_1, ref bytes);

            return bytes;
        }

        public static object Deserialize(byte[] bytes)
        {
            TestStruct o = new TestStruct();
            int offset = 0;

            o.int_1 = Serializer.DeserializeInt(bytes, ref offset);
            o.int_2 = Serializer.DeserializeInt(bytes, ref offset);
            o.float_1 = Serializer.DeserializeInt(bytes, ref offset);
            o.bool_1 = Serializer.DeserializeBool(bytes, ref offset);
            o.string_1 = Serializer.DeserializeString(bytes, ref offset);
            o.vector3_1 = Serializer.DeserializeVector3(bytes, ref offset);
            o.vector3_2 = Serializer.DeserializeVector3(bytes, ref offset);
            o.quaternion_1 = Serializer.DeserializeQuaternion(bytes, ref offset);
            o.enum_1 = (Enum)Serializer.DeserializeInt(bytes, ref offset);

            return o;
        }
    }
}