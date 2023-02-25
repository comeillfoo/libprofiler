using System;
using System.Runtime.InteropServices;

namespace LibProfiler;

public class DllMain {
    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(Guid* rclsid, Guid* riid, IntPtr* ppv) {
        Console.WriteLine("Hello from the profiling API");

        return 0;
    }
}