using System;
using System.Runtime.InteropServices;

namespace LibProfiler;

public class HResult {
    public const int S_OK = 0x0;
}

public class DllMain {
    [UnmanagedCallersOnly]
    public static unsafe int QueryInterface(IntPtr self, Guid* guid, IntPtr* ptr) {
        Console.WriteLine("QueryInterface");
        *ptr = IntPtr.Zero;
        return 0;
    }

    [UnmanagedCallersOnly]
    public static int AddRef(IntPtr self) {
        Console.WriteLine("AddRef");
        return 1;
    }

    [UnmanagedCallersOnly]
    public static int Release(IntPtr self) {
        Console.WriteLine("Release");
        return 1;
    }

    [UnmanagedCallersOnly]
    public static unsafe int CreateInstance(IntPtr self, IntPtr outer, Guid* guid, IntPtr* instance) {
        Console.WriteLine("CreateInstance");
        *instance = IntPtr.Zero;
        return 0;
    }

    [UnmanagedCallersOnly]
    public static int LockServer(IntPtr self, bool @lock) {
        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(Guid* rclsid, Guid* riid, IntPtr* ppv) {
        Console.WriteLine("Hello from the profiling API");

        // Allocate the chunk of memory for the vtable pointer + the pointers to the 5 methods
        var chunk = (IntPtr*)NativeMemory.Alloc(1 + 5, (nuint)IntPtr.Size);

        // Pointer to the vtable
        *chunk = (IntPtr)(chunk + 1);

        // Pointers to each method of the interface
        *(chunk + 1) = (IntPtr)(delegate* unmanaged<IntPtr, Guid*, IntPtr*, int>)&QueryInterface;
        *(chunk + 2) = (IntPtr)(delegate* unmanaged<IntPtr, int>)&AddRef;
        *(chunk + 3) = (IntPtr)(delegate* unmanaged<IntPtr, int>)&Release;
        *(chunk + 4) = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, Guid*, IntPtr*, int>)&CreateInstance;
        *(chunk + 5) = (IntPtr)(delegate* unmanaged<IntPtr, bool, int>)&LockServer;

        *ppv = (IntPtr)chunk;

        return HResult.S_OK;
    }
}