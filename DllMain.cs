using System;
using System.Runtime.InteropServices;

namespace LibProfiler;

public class HResult {
    public const int S_OK = 0x0;
}

public class DllMain {
    private static ClassFactory? Instance;

    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(Guid* rclsid, Guid* riid, IntPtr* ppv) {

        Instance = new ClassFactory();
        Console.WriteLine("Hello from the profiling API");

        *ppv = Instance.Object;

        return HResult.S_OK;
    }
}