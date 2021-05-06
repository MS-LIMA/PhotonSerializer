# I. Abstract
When developing multiplayer game with PUN2, there will be cases where sending custom types such as user defined structure or class through RPC and Event, etc. However, sending theses custom types directly to network is not allowed in PUN2. Instead, PUN2 provides custom type registering method as a solution.

For using PUN2’s custom type registering method, a custom type must be serialized to byte array. In many cases, byte serialization is performed using Binary Formatter. However, Binary Formatter creates a significant amount of additional bytes, which will hurt traffic for network intensive games and give poor experience for mobile users who cannot use free WIFI connection. In addition, Binary Formatter cannot serialize Unity's Vector2, Vector3, Quaternion and etc.

This paper provides custom type registering method with script and compares the difference with other serialization methods.

# II. Serialize with Binary Formatter
### 1. Serialize and Measure the Size
Given structure will be serialized with Binary Formatter and the size will be measured by Marshal. Note that unity objects such as Vector3, Quaternion and etc are not included since those cannot be serialized by Binary Formatter.

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
    public void BinaryFormatterSerialize(TestStruct2 testStruct2)
    {
        byte[] bytes;

        MemoryStream memoryStream = new MemoryStream();
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        binaryFormatter.Serialize(memoryStream, testStruct2);

        memoryStream.Close();
        bytes = memoryStream.ToArray();

        Debug.Log(string.Format("Bianary Formatter Serialized Size : {0} bytes", bytes.Length));
    }
```
```csharp
    public void CheckSize(TestStruct2 testStruct)
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

# II. Serialize with JsonUtility
### 1. Serialize 
Given structure above will be serialized with JsonUtility and coverted to bytes. Serializing the structure is as follows:

```csharp
    public void JsonSerialize(TestStruct2 testStruct2)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(testStruct2));
        Debug.Log(string.Format("Bianary Formatter Serialized Size : {0} bytes", bytes.Length));
    }
```

### 2. Result
The result is as follows:

|   |  size(bytes) |
| ------------ | ------------ |
| Original | 24 bytes  |
|  Binary Formatter | 94 bytes  |

When the given structure is serialized with JsonUtility and converted to bytes, the size is 94 bytes which is about 4 times larger than the theoretical size. This size can be reduced by shortening the names of the variables. For instance, when the names of variables are changed as shown below, the result is as follows.

```csharp
public struct TestStruct2
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
|  Binary Formatter | 67 bytes  |

The size of bytes are reduced from 94 bytes to 67 bytes. However, it is still larger than the theoritical size of 24 bytes.

# III. Serialize with Custom Serializer
### 1. Introduce
This paper introduces custom serializer. This serializer can provide a size that is close to the theoretical size. Serializable types and sizes are as follows:

|  Type | Size(bytes)  |
| ------------ | ------------ |
|  bool | 1 bytes  |
|  int | 4 bytes  |
|  float | 4 bytes  |
|  Vector2 | 8 bytes  |
|  Vector3 | 12 bytes  |
|  Quaternion | 16 bytes  |
|  string | 4 bytes + α (UTF8 Encoding) |

### 2. How to Use
Suppose that the structure is given as follows:

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

First,  create static methods with the names "Serialize" and "Deserialize". "Serialize" method has one parameter with type of object and return type with type of byte[]. "Deserialize" method has one parameter with type of byte[] and return type of object.

Note that the names of the methods, parameters and return types must be same as described.

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


.
