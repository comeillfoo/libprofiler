using System.Runtime.InteropServices;

namespace LibProfiler;

public unsafe class ClassFactory {

    public IntPtr Object { get; }

    public ClassFactory() {
        // Allocate the chunk of memory for the vtable pointer + the pointers to the 5 methods
        var chunk = (IntPtr*)NativeMemory.Alloc(2 + 5, (nuint)IntPtr.Size);

        // Pointer to the vtable
        *chunk = (IntPtr)(chunk + 2);

        // var classFactory = this;
        // *(chunk + 1) = (nint)(nint*)&classFactory; // good idea but bad practice
        // a managed object can be moved at any time by the garbage collector,
        // so the pointer could become invalid at the next garbage collection

        var handle = GCHandle.Alloc(this); // protects object from garbage collection
        *(chunk + 1) = GCHandle.ToIntPtr(handle);


        // Pointers to each method of the interface
        *(chunk + 2) = (IntPtr)(delegate* unmanaged<IntPtr*, Guid*, IntPtr*, int>)&Native.QueryInterface;
        *(chunk + 3) = (IntPtr)(delegate* unmanaged<IntPtr*, int>)&Native.AddRef;
        *(chunk + 4) = (IntPtr)(delegate* unmanaged<IntPtr*, int>)&Native.Release;
        *(chunk + 5) = (IntPtr)(delegate* unmanaged<IntPtr*, IntPtr, Guid*, IntPtr*, int>)&Native.CreateInstance;
        *(chunk + 6) = (IntPtr)(delegate* unmanaged<IntPtr*, bool, int>)&Native.LockServer;

        Object = (IntPtr) chunk;

        // <0xchunk + 0>: <0xchunk + 2>
        // <0xchunk + 1>: &this
        // <0xchunk + 2>: &QueryInterfaceNative
        // <0xchunk + 3>: &AddRefNative
        // <0xchunk + 4>: &ReleaseNative
        // <0xchunk + 5>: &CreateInstanceNative
        // <0xchunk + 6>: &LockServerNative
    }

    public int QueryInterface(Guid* guid, IntPtr* ptr) {
        Console.WriteLine("QueryInterface");
        *ptr = IntPtr.Zero;
        return 0;
    }

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

    private class Native {
        [UnmanagedCallersOnly] // this attribute can only be applied to static methods
        public static int QueryInterface(IntPtr* self, Guid* guid, IntPtr* ptr) {
            var handleAddress = *(self + 1);
            var handle = GCHandle.FromIntPtr(handleAddress);
            var instance = (ClassFactory)handle.Target!;

            return instance!.QueryInterface(guid, ptr);
        }

        [UnmanagedCallersOnly] // this attribute can only be applied to static methods
        public static int AddRef(IntPtr* self) {
            var handleAddress = *(self + 1);
            var handle = GCHandle.FromIntPtr(handleAddress);
            var instance = (ClassFactory)handle.Target!;

            return instance.AddRef();
        }

        [UnmanagedCallersOnly] // this attribute can only be applied to static methods
        public static int Release(IntPtr* self) {
            var handleAddress = *(self + 1);
            var handle = GCHandle.FromIntPtr(handleAddress);
            var instance = (ClassFactory)handle.Target!;

            return instance.Release();
        }

        [UnmanagedCallersOnly] // this attribute can only be applied to static methods
        public static unsafe int CreateInstance(IntPtr* self, IntPtr outer, Guid* guid, IntPtr* instance) {
            var handleAddress = *(self + 1);
            var handle = GCHandle.FromIntPtr(handleAddress);
            var _instance = (ClassFactory)handle.Target!;

            return _instance.CreateInstance(outer, guid, instance);
        }

        [UnmanagedCallersOnly] // this attribute can only be applied to static methods
        public static int LockServer(IntPtr* self, bool @lock) {
            var handleAddress = *(self + 1);
            var handle = GCHandle.FromIntPtr(handleAddress);
            var instance = (ClassFactory)handle.Target!;

            return instance.LockServer(@lock);
        }
    }

}