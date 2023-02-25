# libprofiler

## For build on Linux

```bash
dotnet publish -p:SelfContained=true -r linux-x64 -c Release
export CORECLR_ENABLE_PROFILING=1
export CORECLR_PROFILER=AAAA # so far, anything fits
export CORECLR_PROFILER_PATH=<path to .so>
```