# Abstract
When developing multiplayer game with PUN2, there will be cases where sending custom types such as user defined structure or class through RPC and Event, etc. However, sending theses custom types directly to network is not allowed in PUN2. Instead, PUN2 provides custom type registering method as a solution.

For using PUN2’s custom type registering method, a custom type must be serialized to byte array. In many cases, byte serialization is performed using Binary Formatter. However, Binary Formatter creates a significant amount of additional bytes, which will hurt traffic for network intensive games and give poor experience for mobile users who cannot use free WIFI connection. In addition, Binary Formatter cannot serialize Unity's Vector2, Vector3, Quaternion and etc.

This content provides custom type registering method with script and compares the difference with other serialization methods.
