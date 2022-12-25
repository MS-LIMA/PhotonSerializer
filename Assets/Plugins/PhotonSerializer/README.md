# I. Abstract
When developing multiplayer game with PUN2, there will be cases where sending custom types such as user defined structure or class through RPC and Event, etc. However, sending theses custom types directly to network is not allowed in PUN2. Instead, PUN2 provides custom type registering method as a solution.

For using PUN2’s custom type registering method, a custom type must be serialized into byte array. In many cases, byte serialization is performed using Binary Formatter. However, Binary Formatter creates a significant amount of additional bytes, which will hurt traffic for network intensive games and give poor experience for mobile users who cannot use free WIFI connection. In addition, Binary Formatter cannot serialize Unity's Vector2, Vector3, Quaternion and etc.

This archive provides custom type registering method with script and compares the difference with other serialization methods.

# II. Serialize with Binary Formatter
### 1. Serialize and Measure the Size
Given structure will be serialized with Binary Formatter and the size will be measured by Marshal. Note that Unity objects such as Vector3, Quaternion and etc are not included since those cannot be serialized by Binary Formatter.

```csharp
public struct TestStruct
{
    public int int_1;
    public int int_2;

    public string string_1;

    public float float_1;
    public float float_2;
}

TestStruct testStruct = new TestStruct
{
    int_1 = 30,
    int_2 = 71,
    string_1 = "ABC가나다",
    float_1 = 0.162f,
    float_2 = 62f,
};

```
Serializing the structure and measuring size of it are as follows:

```csharp
public void BinaryFormatterSerialize(TestStruct testStruct)
{
    byte[] bytes;

    MemoryStream memoryStream = new MemoryStream();
    BinaryFormatter binaryFormatter = new BinaryFormatter();

    binaryFormatter.Serialize(memoryStream, testStruct);

    memoryStream.Close();
    bytes = memoryStream.ToArray();

    Debug.Log(string.Format("Bianary Formatter Serialized Size : {0} bytes", bytes.Length));
}
```
```csharp
public void CheckSize(TestStruct testStruct)
{
    Debug.Log(string.Format("Original Size : {0} bytes", Marshal.SizeOf(testStruct)));
    Debug.Log(JsonUtility.ToJson(testStruct, true));
}
```

### 2. Result
The result is as follows:

|   |  size(bytes) |
| ------------ | ------------ |
| Original | 24 bytes  |
|  Binary Formatter | 199 bytes  |

The theoretical size of given structure is 24 bytes. When the given structure is serialized with Binary Formatter, the size is 199 bytes which is 8 times larger than the theoretical size. This may lead to traffic overhead when serializing and sending it to the network.

# III. Serialize with JsonUtility
### 1. Serialize 
Given structure above will be serialized with JsonUtility and coverted to bytes. Serializing the structure is as follows:

```csharp
public void JsonSerialize(TestStruct testStruct)
{
    byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(testStruct));
    Debug.Log(string.Format("JsonUtility Serialized Size : {0} bytes", bytes.Length));
}
```

### 2. Result
The result is as follows:

|   |  size(bytes) |
| ------------ | ------------ |
| Original | 24 bytes  |
|  JsonUtility & byte conversion | 94 bytes  |

When the given structure is serialized with JsonUtility and converted to bytes, the size is 94 bytes which is about 4 times larger than the theoretical size. This size can be reduced by shortening the names of the variables. For instance, when the names of variables are changed as shown below, the result is as follows.

```csharp
public struct TestStruct
{
    public int a;
    public int b;

    public string c;

    public float d;
    public float e;
}

TestStruct testStruct = new TestStruct
{
	a = 30,
	b = 71,
	c = "ABC가나다",
	d = 0.162f,
	e = 62f,
};
```
|   |  size(bytes) |
| ------------ | ------------ |
| Original | 24 bytes  |
|  JsonUtility & byte conversion | 67 bytes  |

The size of bytes are reduced from 94 bytes to 67 bytes. However, it is still larger than the theoritical size of 24 bytes.

# IV. Serialize with Custom Serializer
### 1. Introduce
This archive introduces custom serializer which can serialize custom type such as user defined structure or class. This serializer can provide a size that is close to the theoretical size. Serializable types and sizes are as follows:

|  Type | Size(bytes)  |
| ------------ | ------------ |
|  byte | 1 byte  |
|  byte[] | 4 + (1 * Length) bytes  |
|  bool | 1 byte  |
|  bool[] | 4 + (1 * Length) bytes  |
|  int | 4 bytes  |
|  int[] | 4 + (4 * Length) bytes  |
|  float | 4 bytes  |
|  float[] | 4 + (4 * Length) bytes  |
|  Vector2 | 8 bytes  |
|  Vector2[] | 4 + (8 * Length) bytes  |
|  Vector3 | 12 bytes  |
|  Vector3[] | 4 + (12 * Length) bytes  |
|  Quaternion | 16 bytes  |
|  Quaternion[] | 4 + (16 * Length) bytes  |
|  string | 4 + α (UTF8 Encoding) bytes |
|  string[] | 4 + ((4 + α) * Length) bytes  |

### 2. How to Use
First of all, declare using MSLIMA.Serializer above. 

`using MSLIMA.Serializer; `

Then, Suppose that the structure is given as follows:

```csharp
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
}

TestStruct testStruct = new TestStruct
{
    int_1 = 30,
    int_2 = 71,
    float_1 = 0.162f,
    bool_1 = true,
    string_1 = "ABC가나다",
    vector3_1 = new Vector3(-23f, 62f, 26f),
    vector3_2 = new Vector3(1f, 7f, -15f),
    quaternion_1 = Quaternion.Euler(35f, 0f, 15f)
};
```

First,  create static methods with the names "Serialize" and "Deserialize" **inside of the custom type**. "Serialize" method has one parameter with type of object and return type with type of byte[]. "Deserialize" method has one parameter with type of byte[] and return type of object.

Note that the names of the methods, parameters and return types must be same as described. **In addition, these methods must be static.**

```csharp
public static byte[] Serialize(object customObject)
{
}
```

```csharp
public static object Deserialize(byte[] bytes)
{
}
```
Second, cast customObject to custom type, declare byte array in "Serialize" method.

```csharp
public static byte[] Serialize(object customObject)
{
    TestStruct o = (TestStruct)customObject;
    byte[] bytes = new byte[0];
}
```
Now, use Serializer's method to serialize desired fields and return bytes at last. Note that byte array is passed with ref keyword.

```csharp
public static byte[] Serialize(object customObject)
{
	...
    Serializer.Serialize(o.int_1, ref bytes);
    Serializer.Serialize(o.int_2, ref bytes);
    Serializer.Serialize(o.float_1, ref bytes);
    Serializer.Serialize(o.bool_1, ref bytes);
    Serializer.Serialize(o.string_1, ref bytes);
    Serializer.Serialize(o.vector3_1, ref bytes);
    Serializer.Serialize(o.vector3_2, ref bytes);
    Serializer.Serialize(o.quaternion_1, ref bytes);

    return bytes;
}
```

Third, create new custom type in "Deserialize" method and declare offset variable with type of int and intialize it with 0.

```csharp
public static object Deserialize(byte[] bytes)
{
    TestStruct o = new TestStruct();
    int offset = 0;
}
```

Now, use Serializer's deserialize method to deserialize the fields which are serialized above. Offset is passed with ref keyword and return the custom type created above. 

**Note that the order of deserializing must be same as the order of serializing.**

```csharp
public static object Deserialize(byte[] bytes)
{
	...
    o.int_1 = Serializer.DeserializeInt(bytes, ref offset);
    o.int_2 = Serializer.DeserializeInt(bytes, ref offset);
    o.float_1 = Serializer.DeserializeInt(bytes, ref offset);
    o.bool_1 = Serializer.DeserializeBool(bytes, ref offset);
    o.string_1 = Serializer.DeserializeString(bytes, ref offset);
    o.vector3_1 = Serializer.DeserializeVector3(bytes, ref offset);
    o.vector3_2 = Serializer.DeserializeVector3(bytes, ref offset);
    o.quaternion_1 = Serializer.DeserializeQuaternion(bytes, ref offset);

    return o;
}
```
Last, the custom type should be registered to PUN2. Call the method described below once to register the custom type.  If registering multiple custom types is needed, the byte code must be different. Simply achieved by changing the alphabet of the parameter in the method.

```csharp
Serializer.RegisterCustomType<TestStruct>((byte)'A');
```

### 3. Result
The result of serializtion of given structure is as follows:

|   |  size(bytes) |
| ------------ | ------------ |
| Original | 64 bytes  |
|  Custom Serializer | 69 bytes  |

The theoretical size is 64 bytes where the actual serialized size is 69 bytes. The difference of 5 bytes is caused by string, which can be sized variably by the length. The result is acceptable.

# V. Conclusion
The custom serializer provides a smaller size rather than Binary Formatter or JsonUtility serializing. However, there are limitations that it can be inconvenient to have wirte all serialize methods for every custom types which are supposed to be serialized and does not support nested types. Nevertherless, if serializing simple custom types with primitive types and sending it to the network frequently, this custom serializer would help.

## Install

[![openupm](https://img.shields.io/npm/v/com.ms-lima.photonserializer?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ms-lima.photonserializer/)

Add scoped this registry.
```json
"scopedRegistries": [
    {
      "name": "MS-LIMA",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.ms-lima"
      ]
    }
  ]
```
