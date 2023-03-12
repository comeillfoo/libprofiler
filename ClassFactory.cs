using System.Runtime.InteropServices;

namespace LibProfiler;

public unsafe class ClassFactory {
    private static Dictionary<IntPtr, ClassFactory> _instances = new();

    public IntPtr Object { get; }

    public ClassFactory() {
        // Allocate the chunk of memory for the vtable pointer + the pointers to the 5 methods
        var chunk = (IntPtr*)NativeMemory.Alloc(1 + 5, (nuint)IntPtr.Size);

        // Pointer to the vtable
        *chunk = (IntPtr)(chunk + 1);

        // Pointers to each method of the interface
        *(chunk + 1) = (IntPtr)(delegate* unmanaged<IntPtr, Guid*, IntPtr*, int>)&QueryInterfaceNative;
        *(chunk + 2) = (IntPtr)(delegate* unmanaged<IntPtr, int>)&AddRefNative;
        *(chunk + 3) = (IntPtr)(delegate* unmanaged<IntPtr, int>)&ReleaseNative;
        *(chunk + 4) = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, Guid*, IntPtr*, int>)&CreateInstanceNative;
        *(chunk + 5) = (IntPtr)(delegate* unmanaged<IntPtr, bool, int>)&LockServerNative;

        _instances.Add((IntPtr)chunk, this);
        Object = (IntPtr) chunk;
    }

    public int QueryInterface(Guid* guid, IntPtr* ptr) {
        Console.WriteLine("QueryInterface");
        *ptr = IntPtr.Zero;
        return 0;
    }
    // [...] (same for other instance methods of ClassFactory)

    public int AddRef() {
        Console.WriteLine("AddRef");
        return 1;
    }

    public int Release() {
        Console.WriteLine("Release");
        return 1;
    }

    public unsafe int CreateInstance(IntPtr outer, Guid* guid, IntPtr* instance) {
        Console.WriteLine("CreateInstance");
        *instance = IntPtr.Zero;
        return 0;
    }

    public int LockServer(bool @lock) {
        return 0;
    }

    [UnmanagedCallersOnly] // this attribute can only be applied to static methods
    public static int QueryInterfaceNative(IntPtr self, Guid* guid, IntPtr* ptr) {
        var instance = _instances[self];

        return instance.QueryInterface(guid, ptr);
    }

    // [...] (same for other static methods of ClassFactory)
    [UnmanagedCallersOnly] // this attribute can only be applied to static methods
    public static int AddRefNative(IntPtr self) {
        var instance = _instances[self];

        return instance.AddRef();
    }

    [UnmanagedCallersOnly] // this attribute can only be applied to static methods
    public static int ReleaseNative(IntPtr self) {
        var instance = _instances[self];

        return instance.Release();
    }

    [UnmanagedCallersOnly] // this attribute can only be applied to static methods
    public static unsafe int CreateInstanceNative(IntPtr self, IntPtr outer, Guid* guid, IntPtr* instance) {
        var _instance = _instances[self];

        return _instance.CreateInstance(outer, guid, instance);
    }

    [UnmanagedCallersOnly] // this attribute can only be applied to static methods
    public static int LockServerNative(IntPtr self, bool @lock) {
        var instance = _instances[self];

        return instance.LockServer(@lock);
    }
}