using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MSLIMA.Serializer;

public enum Enum
{
    A,
    B,
    C
}

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

public class Example : MonoBehaviour
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
            string_1 = "ABC°¡³ª´Ù",
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
