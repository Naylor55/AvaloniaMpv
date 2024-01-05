# AvaloniaMpv
一个跨平台的rtsp播放器，使用Avalonia框架完成跨平台，使用Mpv作为播放器。


# 开发环境

* windows11 
* VisualStudio 2022 
* .net 8.0
* Vavlonia 11.0.6
* mpv播放器使用的是 libMpv，libmpv是对mpv的封装，将mpv播放器打包到libmpv中，并提供了对外的C-api 供其他语言调用


# 模块说明

## AvaloniaApplication1.Desktop

* 主启动程序
* mpv文件夹中包含了各个系统的libmpv动态链接库
* 在program.cs 中通过 InitMpv 方法加载 libmpv ，各个操作系统平台加载对应的动态链接库或文件如下：
    * windows：AvaloniaMpv\AvaloniaApplication1.Desktop\mpv\windows\libmpv-2.dll
    * linux：AvaloniaMpv\AvaloniaApplication1.Desktop\mpv\liunx\libmpv.so.1
    * macos：AvaloniaMpv\AvaloniaApplication1.Desktop\mpv\macos\arm64\libmpv.dylib 或者 AvaloniaMpv\AvaloniaApplication1.Desktop\mpv\macos\amd64\libmpv.dylib
    * 路径可以随意配置，只要能找到对应的动态链接库就行。以linux举例，若已经在机器上面安装了mpv（通过官网下载安装包https://mpv.io/installation/  或者通过linux包管理器下载），也可以将path指向/usr/lib/x86_64-linux-gnu 或者 /usr/local/lib
* 主启动程序起来后将加载 AvaloniaApplication1.views.MainView 


## AvaloniaApplication1

* MainView 中定义了16个 mpv:VideoView 控件，用来播放视频
* MainViewModel 实例化了16个VideoViewModel，分别和16个播放控件绑定，在构造函数中调用VideoViewModel的play方法并传递一个视频播放地址作为参数，本例中为摄像机rtsp播放地址


## LibMpv Wrapper

libmpv包装器参考了另外一个项目（https://github.com/homov/LibMpv）

### LibMpv.Client
The LibMpv.Client project contains a complete libmpv API wrapper automatically generated using a modified version of FFmpeg.AutoGen (LibMpv.Generator)

### LibMpv.MVVM
* MpvContext as ViewModel for easier use in MVVM projects
* BaseMpvContextViewModel中的loadFile方法中通过 配置log-file = /tmp/mpv.log 开启mpv的log功能，代码如下： 

~~~

    public void LoadFile(string fileName, string mode = "replace")
    {
        Command("loadfile", fileName, mode);
        SetOptionString("keepaspect", "no");
        SetOptionString("hwdec", "auto-copy");
        SetOptionString("video-sync", "display-resample");
        SetOptionString("interpolation", "yes");
        SetOptionString("tscale", "oversample");
        SetOptionString("log-file", "/tmp/mpv.log");
    
    }

~~~

### LibMpv.Avalonia
VideoView (NativeVideoView, OpenGlVideoView, SoftwareVideoView) control for AvaloniaUI

### What works

Linux (renderers - OpenGl, Software)
Windows (renderers - OpenGl, Software, Native window)
Android (renderers - OpenGl). Works on Android Phone emulator but fails on Android TV emulator


# 流程

* Program：程序启动的时候  LibMpv.Client 将 libpmv 装载到系统中
* MainView.axaml：通过实例化 VideoView 控件初始化mpv播放器
* VideoViewModel.Play：通过调用 VideoViewModel.Play 方法，开启播放

# 问题

## linux

### 闪退问题

当前在UOS 20（专业版）中发现如果同时开启16个播放器窗格，偶发性会闪退，程序崩溃

#### 运行环境

* OS：UOS 20 专业版
* 内存：32G
* 显卡：Geforce RTX 2080 SUPER   6g显存
* cpu：/proc/cpuinfo

~~~

processor	: 0
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.995
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 0
cpu cores	: 8
apicid		: 0
initial apicid	: 0
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 1
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.000
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 1
cpu cores	: 8
apicid		: 2
initial apicid	: 2
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 2
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.999
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 2
cpu cores	: 8
apicid		: 4
initial apicid	: 4
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 3
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.999
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 3
cpu cores	: 8
apicid		: 6
initial apicid	: 6
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 4
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.977
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 4
cpu cores	: 8
apicid		: 8
initial apicid	: 8
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 5
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.999
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 5
cpu cores	: 8
apicid		: 10
initial apicid	: 10
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 6
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.006
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 6
cpu cores	: 8
apicid		: 12
initial apicid	: 12
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 7
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.966
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 7
cpu cores	: 8
apicid		: 14
initial apicid	: 14
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 8
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.009
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 0
cpu cores	: 8
apicid		: 1
initial apicid	: 1
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 9
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.055
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 1
cpu cores	: 8
apicid		: 3
initial apicid	: 3
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 10
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.003
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 2
cpu cores	: 8
apicid		: 5
initial apicid	: 5
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 11
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.026
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 3
cpu cores	: 8
apicid		: 7
initial apicid	: 7
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 12
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.999
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 4
cpu cores	: 8
apicid		: 9
initial apicid	: 9
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 13
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.047
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 5
cpu cores	: 8
apicid		: 11
initial apicid	: 11
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 14
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4900.057
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 6
cpu cores	: 8
apicid		: 13
initial apicid	: 13
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:

processor	: 15
vendor_id	: GenuineIntel
cpu family	: 6
model		: 165
model name	: Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz
stepping	: 5
microcode	: 0xf8
cpu MHz		: 4899.962
cache size	: 16384 KB
physical id	: 0
siblings	: 16
core id		: 7
cpu cores	: 8
apicid		: 15
initial apicid	: 15
fpu		: yes
fpu_exception	: yes
cpuid level	: 22
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 clflush dts acpi mmx fxsr sse sse2 ss ht tm pbe syscall nx pdpe1gb rdtscp lm constant_tsc art arch_perfmon pebs bts rep_good nopl xtopology nonstop_tsc cpuid aperfmperf pni pclmulqdq dtes64 monitor ds_cpl vmx smx est tm2 ssse3 sdbg fma cx16 xtpr pdcm pcid sse4_1 sse4_2 x2apic movbe popcnt tsc_deadline_timer aes xsave avx f16c rdrand lahf_lm abm 3dnowprefetch cpuid_fault epb invpcid_single ssbd ibrs ibpb stibp ibrs_enhanced tpr_shadow vnmi flexpriority ept vpid ept_ad fsgsbase tsc_adjust bmi1 avx2 smep bmi2 erms invpcid mpx rdseed adx smap clflushopt intel_pt xsaveopt xsavec xgetbv1 xsaves dtherm ida arat pln pts hwp hwp_notify hwp_act_window hwp_epp pku ospke md_clear flush_l1d arch_capabilities
bugs		: spectre_v1 spectre_v2 spec_store_bypass swapgs itlb_multihit srbds mmio_stale_data
bogomips	: 7600.00
clflush size	: 64
cache_alignment	: 64
address sizes	: 39 bits physical, 48 bits virtual
power management:



~~~

#### mpvlog

~~~

[   0.100][v][cplayer] mpv 0.34.1 Copyright © 2000-2021 mpv/MPlayer/mplayer2 projects
[   0.100][v][cplayer]  built on UNKNOWN
[   0.100][v][cplayer] FFmpeg library versions:
[   0.100][v][cplayer]    libavutil       56.22.100
[   0.100][v][cplayer]    libavcodec      58.35.100
[   0.100][v][cplayer]    libavformat     58.20.100
[   0.100][v][cplayer]    libswscale      5.3.100
[   0.100][v][cplayer]    libavfilter     7.40.101
[   0.100][v][cplayer]    libswresample   3.3.100
[   0.100][v][cplayer] FFmpeg version: 4.1.9-deepin13
[   0.100][v][cplayer] 
[   0.100][v][cplayer] Configuration: ./waf configure --prefix=/usr --libdir=/usr/lib/x86_64-linux-gnu --confdir=/etc/mpv --zshdir=/usr/share/zsh/vendor-completions --enable-cdda --enable-dvdnav --enable-libmpv-shared --enable-sdl2 --enable-gl-x11 --disable-build-date --enable-dvbin
[   0.102][v][cplayer] List of enabled features: alsa asm caca cdda cplayer cplugins debug-build drm dvbin dvdnav egl egl-drm egl-helpers egl-x11 ffmpeg ffmpeg-aviocontext-bytes-read gbm gbm.h gl gl-wayland gl-x11 glibc-thread-name glob glob-posix gpl iconv jpeg lcms2 libass libavdevice libbluray libdl libm libmpv-shared librt linux-fstatfs linux-input-event-codes lua lua52 memfd_create optimize plain-gl posix posix-or-mingw pthreads pulse rubberband sdl2 sdl2-audio sdl2-gamepad sdl2-video stdatomic uchardet vaapi vaapi-drm vaapi-egl vaapi-wayland vaapi-x-egl vaapi-x11 vdpau vdpau-gl-x11 vector vt.h wayland wayland-protocols x11 xv zlib
[   0.102][v][cplayer] Set property: log-file="/tmp/mpv.log" -> 1
[   0.102][v][cplayer] Set property: pause=false -> 1
[   0.102][v][ffmpeg] Opening rtsp://admin:Admin123@192.168.1.120:554/ch1/main/av_stream
[   0.102][d][ffmpeg] resize stream to 131072 bytes, drop 0 bytes
[   0.102][d][ffmpeg] Stream opened successfully.
[   0.102][v][demux] Trying demuxers for level=request.
[   0.102][d][demux] Trying demuxer: lavf (force-level: request)
[   0.109][v][lavf] Found 'rtsp' at score=100 size=0 (forced).
[   0.109][v][lavf] Broken FFmpeg RTSP API => not setting timeout.
[   0.633][v][cplayer] Set property: wid=0 -> 1
[   0.633][v][libmpv_render] GL_VERSION='4.0.0 NVIDIA 510.85.02'
[   0.633][v][libmpv_render] Detected desktop OpenGL 4.0.
[   0.633][v][libmpv_re[   0.685][v][cplayer] Set property: wid=0 -> 1
v][libmpv_render] GL_RENDERER='NVIDIA GeForce RTX 2080 SUPER/PCIe/SSE2'
[   0.633][v][libmpv_render] GL_SHADING_LANGUAGE_VERSION='4.00 NVIDIA via Cg compiler'
[   0.633][d][libmpv_render] Combined OpenGL extensions string:
[   0.633][d][libmpv_render]  GL_AMD_multi_draw_indirect GL_AMD_seamless_cubemap_per_texture GL_AMD_vertex_shader_viewport_index GL_AMD_vertex_shader_layer GL_ARB_arrays_of_arrays GL_ARB_base_instance GL_ARB_bindless_texture GL_ARB_blend_func_extended GL_ARB_buffer_storage GL_ARB_clear_buffer_object GL_ARB_clear_texture GL_ARB_clip_control GL_ARB_color_buffer_float GL_ARB_compressed_texture_pixel_storage GL_ARB_conservative_depth GL_ARB_compute_shader GL_ARB_compute_variable_group_size GL_ARB_conditional_render_inverted GL_ARB_copy_buffer GL_ARB_copy_image GL_ARB_cull_distance GL_ARB_debug_output GL_ARB_depth_buffer_float GL_ARB_depth_clamp GL_ARB_depth_texture GL_ARB_derivative_control GL_ARB_direct_state_access GL_ARB_draw_buffers GL_ARB_draw_buffers_blend GL_ARB_draw_indirect GL_ARB_draw_elements_base_vertex GL_ARB_draw_instanced GL_ARB_enhanced_layouts GL_ARB_ES2_compatibility GL_ARB_ES3_compatibility GL_ARB_ES3_1_compatibility GL_ARB_ES3_2_compatibility GL_ARB_explicit_attrib_location GL_ARB_explicit_uniform_location GL_ARB_fragment_coord_conventions GL_ARB_fragment_layer_viewport GL_ARB_fragment_program GL_ARB_fragment_program_shadow GL_ARB_fragment_shader GL_ARB_fragment_shader_interlock GL_ARB_framebuffer_no_attachments GL_ARB_framebuffer_object GL_ARB_framebuffer_sRGB GL_ARB_geometry_shader4 GL_ARB_get_program_binary GL_ARB_get_texture_sub_image GL_ARB_gl_spirv GL_ARB_gpu_shader5 GL_ARB_gpu_shader_fp64 GL_ARB_gpu_shader_int64 GL_ARB_half_float_pixel GL_ARB_half_float_vertex GL_ARB_imaging GL_ARB_indirect_parameters GL_ARB_instanced_arrays GL_ARB_internalformat_query GL_ARB_internalformat_query2 GL_ARB_invalidate_subdata GL_ARB_map_buffer_alignment GL_ARB_map_buffer_range GL_ARB_multi_bind GL_ARB_multi_draw_indirect GL_ARB_multisample GL_ARB_multitexture GL_ARB_occlusion_query GL_ARB_occlusion_query2 GL_ARB_parallel_shader_compile GL_ARB_pipeline_statistics_query GL_ARB_pixel_buffer_object GL_ARB_point_parameters GL_ARB_point_sprite GL_ARB_polygon_offset_clamp GL_ARB_post_depth_coverage GL_ARB_program_interface_query GL_ARB_provoking_vertex GL_ARB_query_buffer_object GL_ARB_robust_buffer_access_behavior GL_ARB_robustness GL_ARB_sample_locations GL_ARB_sample_shading GL_ARB_sampler_objects GL_ARB_seamless_cube_map GL_ARB_seamless_cubemap_per_texture GL_ARB_separate_shader_objects GL_ARB_shader_atomic_counter_ops GL_ARB_shader_atomic_counters GL_ARB_shader_ballot GL_ARB_shader_bit_encoding GL_ARB_shader_clock GL_ARB_shader_draw_parameters GL_ARB_shader_group_vote GL_ARB_shader_image_load_store GL_ARB_shader_image_size GL_ARB_shader_objects GL_ARB_shader_precision GL_ARB_shader_storage_buffer_object GL_ARB_shader_subroutine GL_ARB_shader_texture_image_samples GL_ARB_shader_texture_lod GL_ARB_shading_language_100 GL_ARB_shader_viewport_layer_array GL_ARB_shading_language_420pack GL_ARB_shading_language_include GL_ARB_shading_language_packing GL_ARB_shadow GL_ARB_sparse_buffer GL_ARB_sparse_texture GL_ARB_sparse_texture2 GL_ARB_sparse_texture_clamp GL_ARB_spirv_extensions GL_ARB_stencil_texturing GL_ARB_sync GL_ARB_tessellation_shader GL_ARB_texture_barrier GL_ARB_texture_border_clamp GL_ARB_texture_buffer_object GL_ARB_texture_buffer_object_rgb32 GL_ARB_texture_buffer_range GL_ARB_texture_compression GL_ARB_texture_compression_bptc GL_ARB_texture_compression_rgtc GL_ARB_texture_cube_map GL_ARB_texture_cube_map_array GL_ARB_texture_env_add GL_ARB_texture_env_combine GL_ARB_texture_env_crossbar GL_ARB_texture_env_dot3 GL_ARB_texture_filter_anisotropic GL_ARB_texture_filter_minmax GL_ARB_texture_float GL_ARB_texture_gather GL_ARB_texture_mirror_clamp_to_edge GL_ARB_texture_mirrored_repeat GL_ARB_texture_multisample GL_ARB_texture_non_power_of_two GL_ARB_texture_query_levels GL_ARB_texture_query_lod GL_ARB_texture_rectangle GL_ARB_texture_rg GL_ARB_texture_rgb10_a2ui GL_ARB_texture_stencil8 GL_ARB_texture_storage GL_ARB_texture_storage_multisample GL_ARB_texture_swizzle GL_ARB_texture_view GL_ARB_timer_query GL_ARB_transform_feedback2 GL_ARB_transform_feedback3 GL_ARB_transform_feedback_instanced GL_ARB_transform_feedback_overflow_query GL_ARB_transpose_matrix GL_ARB_uniform_buffer_object GL_ARB_vertex_array_bgra GL_ARB_vertex_array_object GL_ARB_vertex_attrib_64bit GL_ARB_vertex_attrib_binding GL_ARB_vertex_buffer_object GL_ARB_vertex_program GL_ARB_vertex_shader GL_ARB_vertex_type_10f_11f_11f_rev GL_ARB_vertex_type_2_10_10_10_rev GL_ARB_viewport_array GL_ARB_window_pos GL_ATI_draw_buffers GL_ATI_texture_float GL_ATI_texture_mirror_once GL_S3_s3tc GL_EXT_texture_env_add GL_EXT_abgr GL_EXT_bgra GL_EXT_bindable_uniform GL_EXT_blend_color GL_EXT_blend_equation_separate GL_EXT_blend_func_separate GL_EXT_blend_minmax GL_EXT_blend_subtract GL_EXT_compiled_vertex_array GL_EXT_Cg_shader GL_EXT_depth_bounds_test GL_EXT_direct_state_access GL_EXT_draw_buffers2 GL_EXT_draw_instanced GL_EXT_draw_range_elements GL_EXT_fog_coord GL_EXT_framebuffer_blit GL_EXT_framebuffer_multisample GL_EXTX_framebuffer_mixed_formats GL_EXT_framebuffer_multisample_blit_scaled GL_EXT_framebuffer_object GL_EXT_framebuffer_sRGB GL_EXT_geometry_shader4 GL_EXT_gpu_program_parameters GL_EXT_gpu_shader4 GL_EXT_multi_draw_arrays GL_EXT_multiview_texture_multisample GL_EXT_multiview_timer_query GL_EXT_packed_depth_stencil GL_EXT_packed_float GL_EXT_packed_pixels GL_EXT_pixel_buffer_object GL_EXT_point_parameters GL_EXT_polygon_offset_clamp GL_EXT_post_depth_coverage GL_EXT_provoking_vertex GL_EXT_raster_multisample GL_EXT_rescale_normal GL_EXT_secondary_color GL_EXT_separate_shader_objects GL_EXT_separate_specular_color GL_EXT_shader_image_load_formatted GL_EXT_shader_image_load_store GL_EXT_shader_integer_mix GL_EXT_shadow_funcs GL_EXT_sparse_texture2 GL_EXT_stencil_two_side GL_EXT_stencil_wrap GL_EXT_texture3D GL_EXT_texture_array GL_EXT_texture_buffer_object GL_EXT_texture_compression_dxt1 GL_EXT_texture_compression_latc GL_EXT_texture_compression_rgtc GL_EXT_texture_compression_s3tc GL_EXT_texture_cube_map GL_EXT_texture_edge_clamp GL_EXT_texture_env_combine GL_EXT_texture_env_dot3 GL_EXT_texture_filter_anisotropic GL_EXT_texture_filter_minmax GL_EXT_texture_integer GL_EXT_texture_lod GL_EXT_texture_lod_bias GL_EXT_texture_mirror_clamp GL_EXT_texture_object GL_EXT_texture_shadow_lod GL_EXT_texture_shared_exponent GL_EXT_texture_sRGB GL_EXT_texture_sRGB_R8 GL_EXT_texture_sRGB_decode GL_EXT_texture_storage GL_EXT_texture_swizzle GL_EXT_timer_query GL_EXT_transform_feedback2 GL_EXT_vertex_array GL_EXT_vertex_array_bgra GL_EXT_vertex_attrib_64bit GL_EXT_window_rectangles GL_EXT_x11_sync_object GL_EXT_import_sync_object GL_NV_robustness_video_memory_purge GL_IBM_rasterpos_clip GL_IBM_texture_mirrored_repeat GL_KHR_context_flush_control GL_KHR_debug GL_EXT_memory_object GL_EXT_memory_object_fd GL_NV_memory_object_sparse GL_KHR_parallel_shader_compile GL_KHR_no_error GL_KHR_robust_buffer_access_behavior GL_KHR_robustness GL_EXT_semaphore GL_EXT_semaphore_fd GL_NV_timeline_semaphore GL_KHR_shader_subgroup GL_KTX_buffer_region GL_NV_alpha_to_coverage_dither_control GL_NV_bindless_multi_draw_indirect GL_NV_bindless_multi_draw_indirect_count GL_NV_bindless_texture GL_NV_blend_equation_advanced GL_NV_blend_equation_advanced_coherent GL_NVX_blend_equation_advanced_multi_draw_buffers GL_NV_blend_minmax_factor GL_NV_blend_square GL_NV_clip_space_w_scaling GL_NV_command_list GL_NV_compute_program5 GL_NV_compute_shader_derivatives GL_NV_conditional_render GL_NV_conservative_raster GL_NV_conservative_raster_dilate GL_NV_conservative_raster_pre_snap GL_NV_conservative_raster_pre_snap_triangles GL_NV_conservative_raster_underestimation GL_NV_copy_depth_to_color GL_NV_copy_image GL_NV_depth_buffer_float GL_NV_depth_clamp GL_NV_draw_texture GL_NV_draw_vulkan_image GL_NV_ES1_1_compatibility GL_NV_ES3_1_compatibility GL_NV_explicit_multisample GL_NV_feature_query GL_NV_fence GL_NV_fill_rectangle GL_NV_float_buffer GL_NV_fog_distance GL_NV_fragment_coverage_to_color GL_NV_fragment_program GL_NV_fragment_program_option GL_NV_fragment_program2 GL_NV_fragment_shader_barycentric GL_NV_fragment_shader_interlock GL_NV_framebuffer_mixed_samples GL_NV_framebuffer_multisample_coverage GL_NV_geometry_shader4 GL_NV_geometry_shader_passthrough GL_NV_gpu_program4 GL_NV_internalformat_sample_query GL_NV_gpu_program4_1 GL_NV_gpu_program5 GL_NV_gpu_program5_mem_extended GL_NV_gpu_program_fp64 GL_NV_gpu_shader5 GL_NV_half_float GL_NV_light_max_exponent GL_NV_memory_attachment GL_NV_mesh_shader GL_NV_multisample_coverage GL_NV_multisample_filter_hint GL_NV_occlusion_query GL_NV_packed_depth_stencil GL_NV_parameter_buffer_object GL_NV_parameter_buffer_object2 GL_NV_path_rendering GL_NV_path_rendering_shared_edge GL_NV_pixel_data_range GL_NV_point_sprite GL_NV_primitive_restart GL_NV_query_resource GL_NV_query_resource_tag GL_NV_register_combiners GL_NV_register_combiners2 GL_NV_representative_fragment_test GL_NV_sample_locations GL_NV_sample_mask_override_coverage GL_NV_scissor_exclusive GL_NV_shader_atomic_counters GL_NV_shader_atomic_float GL_NV_shader_atomic_float64 GL_NV_shader_atomic_fp16_vector GL_NV_shader_atomic_int64 GL_NV_shader_buffer_load GL_NV_shader_storage_buffer_object GL_NV_shader_subgroup_partitioned GL_NV_shader_texture_footprint GL_NV_shading_rate_image GL_NV_stereo_view_rendering GL_NV_texgen_reflection GL_NV_texture_barrier GL_NV_texture_compression_vtc GL_NV_texture_env_combine4 GL_NV_texture_multisample GL_NV_texture_rectangle GL_NV_texture_rectangle_compressed GL_NV_texture_shader GL_NV_texture_shader2 GL_NV_texture_shader3 GL_NV_transform_feedback GL_NV_transform_feedback2 GL_NV_uniform_buffer_unified_memory GL_NV_vdpau_interop GL_NV_vdpau_interop2 GL_NV_vertex_array_range GL_NV_vertex_array_range2 GL_NV_vertex_attrib_integer_64bit GL_NV_vertex_buffer_unified_memory GL_NV_vertex_program GL_NV_vertex_program1_1 GL_NV_vertex_program2 GL_NV_vertex_program2_option GL_NV_vertex_program3 GL_NV_viewport_array2 GL_NV_viewport_swizzle GL_NVX_conditional_render GL_NV_gpu_multicast GL_NVX_progress_fence GL_NVX_gpu_memory_info GL_NVX_nvenc_interop GL_NV_shader_thread_group GL_NV_shader_thread_shuffle GL_KHR_blend_equation_advanced GL_KHR_blend_equation_advanced_coherent GL_OVR_multiview GL_OVR_multiview2 GL_SGIS_generate_mipmap GL_SGIS_texture_lod GL_SGIX_depth_texture GL_SGIX_shadow GL_SUN_slice_accum
[   0.634][v][libmpv_render] Loaded extension GL_ARB_get_program_binary.
[   0.634][v][libmpv_render] Loaded extension GL_ARB_buffer_storage.
[   0.634][v][libmpv_render] Loaded extension GL_ARB_shader_image_load_store.
[   0.634][v][libmpv_render] Loaded extension GL_ARB_shader_storage_buffer_object.
[   0.634][v][libmpv_render] Loaded extension GL_ARB_compute_shader.
[   0.634][v][libmpv_render] Loaded extension GL_ARB_arrays_of_arrays.
[   0.634][v][libmpv_render] Loaded extension GL_NV_vdpau_interop.
[   0.634][v][libmpv_render] Loaded extension GL_ARB_debug_output.
[   0.634][v][libmpv_render] GL_*_swap_control extension missing.
[   0.634][d][libmpv_render] Texture formats:
[   0.634][d][libmpv_render]   NAME       COMP*TYPE SIZE           DEPTH PER COMP.
[   0.634][d][libmpv_render]   r8         1*unorm   1B    LF CR ST {8}
[   0.634][d][libmpv_render]   rg8        2*unorm   2B    LF CR ST {8 8}
[   0.634][d][libmpv_render]   rgb8       3*unorm   3B    LF CR ST {8 8 8}
[   0.634][d][libmpv_render]   rgba8      4*unorm   4B    LF CR ST {8 8 8 8}
[   0.634][d][libmpv_render]   r16        1*unorm   2B    LF CR ST {16}
[   0.634][d][libmpv_render]   rg16       2*unorm   4B    LF CR ST {16 16}
[   0.634][d][libmpv_render]   rgb16      3*unorm   6B    LF CR ST {16 16 16}
[   0.634][d][libmpv_render]   rgba16     4*unorm   8B    LF CR ST {16 16 16 16}
[   0.634][d][libmpv_render]   r8ui       1*uint    1B       CR ST {8}
[   0.634][d][libmpv_render]   rg8ui      2*uint    2B       CR ST {8 8}
[   0.634][d][libmpv_render]   rgb8ui     3*uint    3B          ST {8 8 8}
[   0.634][d][libmpv_render]   rgba8ui    4*uint    4B       CR ST {8 8 8 8}
[   0.634][d][libmpv_render]   r16ui      1*uint    2B       CR ST {16}
[   0.634][d][libmpv_render]   rg16ui     2*uint    4B       CR ST {16 16}
[   0.634][d][libmpv_render]   rgb16ui    3*uint    6B          ST {16 16 16}
[   0.634][d][libmpv_render]   rgba16ui   4*uint    8B       CR ST {16 16 16 16}
[   0.634][d][libmpv_render]   r16f       1*float   4B    LF CR ST {32/16}
[   0.634][d][libmpv_render]   rg16f      2*float   8B    LF CR ST {32/16 32/16}
[   0.634][d][libmpv_render]   rgb16f     3*float  12B    LF CR ST {32/16 32/16 32/16}
[   0.634][d][libmpv_render]   rgba16f    4*float  16B    LF CR ST {32/16 32/16 32/16 32/16}
[   0.634][d][libmpv_render]   r32f       1*float   4B    LF CR ST {32}
[   0.634][d][libmpv_render]   rg32f      2*float   8B    LF CR ST {32 32}
[   0.634][d][libmpv_render]   rgb32f     3*float  12B    LF CR ST {32 32 32}
[   0.634][d][libmpv_render]   rgba32f    4*float  16B    LF CR ST {32 32 32 32}
[   0.634][d][libmpv_render]   rgb10_a2   4*unorm   4B    LF CR ST {0/10 0/10 0/10 0/2}
[   0.634][d][libmpv_render]   rgba12     4*unorm   8B    LF CR ST {16 16 16 16}
[   0.634][d][libmpv_render]   rgb10      3*unorm   6B    LF CR ST {16/10 16/10 16/10}
[   0.634][d][libmpv_render]   rgb565     3*unorm   2B    LF    ST {0/8 0/8 0/8}
[   0.634][d][libmpv_render]  LA = LUMINANCE_ALPHA hack format
[   0.634][d][libmpv_render]  LF = linear filterable
[   0.634][d][libmpv_render]  CR = can be used for render targets
[   0.634][d][libmpv_render]  ST = can be used for storable images
[   0.634][d][libmpv_render] Image formats:
[   0.634][d][libmpv_render]   yuv444p => 3 planes 1x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv420p => 3 planes 2x2 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   gray => 1 planes 1x1 8/0 [r8] (r) [unorm]
[   0.634][d][libmpv_render]   gray16 => 1 planes 1x1 16/0 [r16] (r) [unorm]
[   0.634][d][libmpv_render]   uyvy422
[   0.634][d][libmpv_render]   nv12 => 2 planes 2x2 8/0 [r8/rg8] (r/gb) [unorm]
[   0.634][d][libmpv_render]   p010 => 2 planes 2x2 16/6 [r16/rg16] (r/gb) [unorm]
[   0.634][d][libmpv_render]   argb => 1 planes 1x1 8/0 [rgba8] (argb) [unorm]
[   0.634][d][libmpv_render]   bgra => 1 planes 1x1 8/0 [rgba8] (bgra) [unorm]
[   0.634][d][libmpv_render]   abgr => 1 planes 1x1 8/0 [rgba8] (abgr) [unorm]
[   0.634][d][libmpv_render]   rgba => 1 planes 1x1 8/0 [rgba8] (rgba) [unorm]
[   0.634][d][libmpv_render]   bgr24 => 1 planes 1x1 8/0 [rgb8] (bgr) [unorm]
[   0.634][d][libmpv_render]   rgb24 => 1 planes 1x1 8/0 [rgb8] (rgb) [unorm]
[   0.634][d][libmpv_render]   0rgb => 1 planes 1x1 8/0 [rgba8] (_rgb) [unorm]
[   0.634][d][libmpv_render]   bgr0 => 1 planes 1x1 8/0 [rgba8] (bgr) [unorm]
[   0.634][d][libmpv_render]   0bgr => 1 planes 1x1 8/0 [rgba8] (_bgr) [unorm]
[   0.634][d][libmpv_render]   rgb0 => 1 planes 1x1 8/0 [rgba8] (rgb) [unorm]
[   0.634][d][libmpv_render]   rgba64 => 1 planes 1x1 16/0 [rgba16] (rgba) [unorm]
[   0.634][d][libmpv_render]   rgb565 => 1 planes 1x1 0/0 [rgb565] (rgb) [unknown]
[   0.634][d][libmpv_render]   pal8
[   0.634][d][libmpv_render]   vdpau
[   0.634][d][libmpv_render]   d3d11
[   0.634][d][libmpv_render]   dxva2_vld
[   0.634][d][libmpv_render]   mmal
[   0.634][d][libmpv_render]   mediacodec
[   0.634][d][libmpv_render]   drm_prime
[   0.634][d][libmpv_render]   cuda
[   0.634][d][libmpv_render]   yap8 => 2 planes 1x1 8/0 [r8/r8] (r/a) [unorm]
[   0.634][d][libmpv_render]   yap16 => 2 planes 1x1 16/0 [r16/r16] (r/a) [unorm]
[   0.634][d][libmpv_render]   grayaf32 => 2 planes 1x1 32/0 [r16f/r16f] (r/a) [float]
[   0.634][d][libmpv_render]   yuv444pf => 3 planes 1x1 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.634][d][libmpv_render]   yuva444pf => 4 planes 1x1 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.634][d][libmpv_render]   yuv420pf => 3 planes 2x2 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.634][d][libmpv_render]   yuva420pf => 4 planes 2x2 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.634][d][libmpv_render]   yuv422pf => 3 planes 2x1 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.634][d][libmpv_render]   yuva422pf => 4 planes 2x1 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.634][d][libmpv_render]   yuv440pf => 3 planes 1x2 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.634][d][libmpv_render]   yuva440pf => 4 planes 1x2 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.634][d][libmpv_render]   yuv410pf => 3 planes 4x4 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.634][d][libmpv_render]   yuva410pf => 4 planes 4x4 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.634][d][libmpv_render]   yuv411pf => 3 planes 4x1 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.634][d][libmpv_render]   yuva411pf => 4 planes 4x1 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.634][d][libmpv_render]   rgb30 => 1 planes 1x1 10/0 [rgb10_a2] (bgr) [unknown]
[   0.634][d][libmpv_render]   y1 => 1 planes 1x1 8/-7 [r8] (r) [unorm]
[   0.634][d][libmpv_render]   gbrp1 => 3 planes 1x1 8/-7 [r8/r8/r8] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp2 => 3 planes 1x1 8/-6 [r8/r8/r8] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp3 => 3 planes 1x1 8/-5 [r8/r8/r8] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp4 => 3 planes 1x1 8/-4 [r8/r8/r8] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp5 => 3 planes 1x1 8/-3 [r8/r8/r8] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp6 => 3 planes 1x1 8/-2 [r8/r8/r8] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   vdpau_output
[   0.634][d][libmpv_render]   vaapi
[   0.634][d][libmpv_render]   videotoolbox
[   0.634][d][libmpv_render]   yuyv422
[   0.634][d][libmpv_render]   yuv422p => 3 planes 2x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv410p => 3 planes 4x4 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv411p => 3 planes 4x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   monow
[   0.634][d][libmpv_render]   monob
[   0.634][d][libmpv_render]   yuvj422p => 3 planes 2x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   uyyvyy411
[   0.634][d][libmpv_render]   bgr8
[   0.634][d][libmpv_render]   bgr4
[   0.634][d][libmpv_render]   bgr4_byte
[   0.634][d][libmpv_render]   rgb8
[   0.634][d][libmpv_render]   rgb4
[   0.634][d][libmpv_render]   rgb4_byte
[   0.634][d][libmpv_render]   nv21 => 2 planes 2x2 8/0 [r8/rg8] (r/bg) [unorm]
[   0.634][d][libmpv_render]   gray16be
[   0.634][d][libmpv_render]   yuv440p => 3 planes 1x2 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuvj440p => 3 planes 1x2 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuva420p => 4 planes 2x2 8/0 [r8/r8/r8/r8] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   rgb48be
[   0.634][d][libmpv_render]   rgb48 => 1 planes 1x1 16/0 [rgb16] (rgb) [unorm]
[   0.634][d][libmpv_render]   rgb565be
[   0.634][d][libmpv_render]   rgb555be
[   0.634][d][libmpv_render]   rgb555
[   0.634][d][libmpv_render]   bgr565be
[   0.634][d][libmpv_render]   bgr565
[   0.634][d][libmpv_render]   bgr555be
[   0.634][d][libmpv_render]   bgr555
[   0.634][d][libmpv_render]   vaapi_moco
[   0.634][d][libmpv_render]   vaapi_idct
[   0.634][d][libmpv_render]   yuv420p16 => 3 planes 2x2 16/0 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv420p16be
[   0.634][d][libmpv_render]   yuv422p16 => 3 planes 2x1 16/0 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv422p16be
[   0.634][d][libmpv_render]   yuv444p16 => 3 planes 1x1 16/0 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv444p16be
[   0.634][d][libmpv_render]   rgb444
[   0.634][d][libmpv_render]   rgb444be
[   0.634][d][libmpv_render]   bgr444
[   0.634][d][libmpv_render]   bgr444be
[   0.634][d][libmpv_render]   ya8 => 1 planes 1x1 8/0 [rg8] (ra) [unorm]
[   0.634][d][libmpv_render]   bgr48be
[   0.634][d][libmpv_render]   bgr48 => 1 planes 1x1 16/0 [rgb16] (bgr) [unorm]
[   0.634][d][libmpv_render]   yuv420p9be
[   0.634][d][libmpv_render]   yuv420p9 => 3 planes 2x2 16/-7 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv420p10be
[   0.634][d][libmpv_render]   yuv420p10 => 3 planes 2x2 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv422p10be
[   0.634][d][libmpv_render]   yuv422p10 => 3 planes 2x1 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv444p9be
[   0.634][d][libmpv_render]   yuv444p9 => 3 planes 1x1 16/-7 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv444p10be
[   0.634][d][libmpv_render]   yuv444p10 => 3 planes 1x1 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv422p9be
[   0.634][d][libmpv_render]   yuv422p9 => 3 planes 2x1 16/-7 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   gbrp => 3 planes 1x1 8/0 [r8/r8/r8] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp9be
[   0.634][d][libmpv_render]   gbrp9 => 3 planes 1x1 16/-7 [r16/r16/r16] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp10be
[   0.634][d][libmpv_render]   gbrp10 => 3 planes 1x1 16/-6 [r16/r16/r16] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp16be
[   0.634][d][libmpv_render]   gbrp16 => 3 planes 1x1 16/0 [r16/r16/r16] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   yuva422p => 4 planes 2x1 8/0 [r8/r8/r8/r8] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva444p => 4 planes 1x1 8/0 [r8/r8/r8/r8] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva420p9be
[   0.634][d][libmpv_render]   yuva420p9 => 4 planes 2x2 16/-7 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva422p9be
[   0.634][d][libmpv_render]   yuva422p9 => 4 planes 2x1 16/-7 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva444p9be
[   0.634][d][libmpv_render]   yuva444p9 => 4 planes 1x1 16/-7 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva420p10be
[   0.634][d][libmpv_render]   yuva420p10 => 4 planes 2x2 16/-6 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva422p10be
[   0.634][d][libmpv_render]   yuva422p10 => 4 planes 2x1 16/-6 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva444p10be
[   0.634][d][libmpv_render]   yuva444p10 => 4 planes 1x1 16/-6 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva420p16be
[   0.634][d][libmpv_render]   yuva420p16 => 4 planes 2x2 16/0 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva422p16be
[   0.634][d][libmpv_render]   yuva422p16 => 4 planes 2x1 16/0 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   yuva444p16be
[   0.634][d][libmpv_render]   yuva444p16 => 4 planes 1x1 16/0 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.634][d][libmpv_render]   xyz12 => 1 planes 1x1 16/4 [rgb16] (rgb) [unorm]
[   0.634][d][libmpv_render]   xyz12be
[   0.634][d][libmpv_render]   nv16 => 2 planes 2x1 8/0 [r8/rg8] (r/gb) [unorm]
[   0.634][d][libmpv_render]   nv20 => 2 planes 2x1 16/-6 [r16/rg16] (r/gb) [unorm]
[   0.634][d][libmpv_render]   nv20be
[   0.634][d][libmpv_render]   rgba64be
[   0.634][d][libmpv_render]   bgra64be
[   0.634][d][libmpv_render]   bgra64 => 1 planes 1x1 16/0 [rgba16] (bgra) [unorm]
[   0.634][d][libmpv_render]   yvyu422
[   0.634][d][libmpv_render]   ya16be
[   0.634][d][libmpv_render]   ya16 => 1 planes 1x1 16/0 [rg16] (ra) [unorm]
[   0.634][d][libmpv_render]   gbrap => 4 planes 1x1 8/0 [r8/r8/r8/r8] (g/b/r/a) [unorm]
[   0.634][d][libmpv_render]   gbrap16be
[   0.634][d][libmpv_render]   gbrap16 => 4 planes 1x1 16/0 [r16/r16/r16/r16] (g/b/r/a) [unorm]
[   0.634][d][libmpv_render]   qsv
[   0.634][d][libmpv_render]   d3d11va_vld
[   0.634][d][libmpv_render]   yuv420p12be
[   0.634][d][libmpv_render]   yuv420p12 => 3 planes 2x2 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv420p14be
[   0.634][d][libmpv_render]   yuv420p14 => 3 planes 2x2 16/-2 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv422p12be
[   0.634][d][libmpv_render]   yuv422p12 => 3 planes 2x1 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv422p14be
[   0.634][d][libmpv_render]   yuv422p14 => 3 planes 2x1 16/-2 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv444p12be
[   0.634][d][libmpv_render]   yuv444p12 => 3 planes 1x1 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv444p14be
[   0.634][d][libmpv_render]   yuv444p14 => 3 planes 1x1 16/-2 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   gbrp12be
[   0.634][d][libmpv_render]   gbrp12 => 3 planes 1x1 16/-4 [r16/r16/r16] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   gbrp14be
[   0.634][d][libmpv_render]   gbrp14 => 3 planes 1x1 16/-2 [r16/r16/r16] (g/b/r) [unorm]
[   0.634][d][libmpv_render]   yuvj411p => 3 planes 4x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   bayer_bggr8
[   0.634][d][libmpv_render]   bayer_rggb8
[   0.634][d][libmpv_render]   bayer_gbrg8
[   0.634][d][libmpv_render]   bayer_grbg8
[   0.634][d][libmpv_render]   bayer_bggr16
[   0.634][d][libmpv_render]   bayer_bggr16be
[   0.634][d][libmpv_render]   bayer_rggb16
[   0.634][d][libmpv_render]   bayer_rggb16be
[   0.634][d][libmpv_render]   bayer_gbrg16
[   0.634][d][libmpv_render]   bayer_gbrg16be
[   0.634][d][libmpv_render]   bayer_grbg16
[   0.634][d][libmpv_render]   bayer_grbg16be
[   0.634][d][libmpv_render]   xvmc
[   0.634][d][libmpv_render]   yuv440p10 => 3 planes 1x2 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv440p10be
[   0.634][d][libmpv_render]   yuv440p12 => 3 planes 1x2 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.634][d][libmpv_render]   yuv440p12be
[   0.634][d][libmpv_render]   ayuv64 => 1 planes 1x1 16/0 [rgba16] (argb) [unorm]
[   0.634][d][libmpv_render]   ayuv64be
[   0.634][d][libmpv_render]   p010be
[   0.634][d][libmpv_render]   gbrap12be
[   0.634][d][libmpv_render]   gbrap12 => 4 planes 1x1 16/-4 [r16/r16/r16/r16] (g/b/r/a) [unorm]
[   0.634][d][libmpv_render]   gbrap10be
[   0.634][d][libmpv_render]   gbrap10 => 4 planes 1x1 16/-6 [r16/r16/r16/r16] (g/b/r/a) [unorm]
[   0.634][d][libmpv_render]   gray12be
[   0.634][d][libmpv_render]   gray12 => 1 planes 1x1 16/-4 [r16] (r) [unorm]
[   0.634][d][libmpv_render]   gray10be
[   0.634][d][libmpv_render]   gray10 => 1 planes 1x1 16/-6 [r16] (r) [unorm]
[   0.634][d][libmpv_render]   p016 => 2 planes 2x2 16/0 [r16/rg16] (r/gb) [unorm]
[   0.634][d][libmpv_render]   p016be
[   0.634][d][libmpv_render]   gray9be
[   0.634][d][libmpv_render]   gray9 => 1 planes 1x1 16/-7 [r16] (r) [unorm]
[   0.634][d][libmpv_render]   gbrpf32be
[   0.634][d][libmpv_render]   gbrpf32 => 3 planes 1x1 32/0 [r16f/r16f/r16f] (g/b/r) [float]
[   0.634][d][libmpv_render]   gbrapf32be
[   0.634][d][libmpv_render]   gbrapf32 => 4 planes 1x1 32/0 [r16f/r16f/r16f/r16f] (g/b/r/a) [float]
[   0.634][d][libmpv_render]   opencl
[   0.634][d][libmpv_render]   gray14be
[   0.634][d][libmpv_render]   gray14 => 1 planes 1x1 16/-2 [r16] (r) [unorm]
[   0.634][d][libmpv_render]   grayf32be
[   0.634][d][libmpv_render]   grayf32 => 1 planes 1x1 32/0 [r16f] (r) [float]
[   0.634][v][libmpv_render] Testing FBO format rgba16f
[   0.634][d][libmpv_render] Resizing texture: 16x16
[   0.634][e][libmpv_render] after creating texture: OpenGL error INVALID_ENUM.
[   0.634][v][libmpv_render] Using FBO format rgba16f.
[   0.634][v][libmpv_render] Disabling HDR peak computation (one or more of the following is not supported: compute shaders=0, SSBO=1).
[   0.634][v][libmpv_render] Loading hwdec driver 'vaapi-egl'
[   0.634][v][libmpv_render/vaapi-egl] VAAPI hwdec only works with OpenGL or Vulkan backends.
[   0.634][v][libmpv_render] Loading failed.
[   0.634][v][libmpv_render] Loading hwdec driver 'vdpau-gl'
[   0.642][v][libmpv_render] Loading hwdec driver 'drmprime-drm'
[   0.642][v][libmpv_render/drmprime-drm] Failed to retrieve DRM fd from native display.
[   0.642][v][libmpv_render] Loading failed.
.561][v][libmpv_render] Loading hwdec driver 'drmprime-drm'
[   0.561][v][libmpv_render/drmprime-drm] Failed to retrieve DRM fd from native display.
[   0.561][v][libmpv_render] Loading failed.
istance GL_ARB_debug_output GL_ARB_depth_buffer_float GL_ARB_depth_clamp GL_ARB_depth_texture GL_ARB_derivative_control GL_ARB_direct_state_access GL_ARB_draw_buffers GL_ARB_draw_buffers_blend GL_ARB_draw_indirect GL_ARB_draw_elements_base_vertex GL_ARB_draw_instanced GL_ARB_enhanced_layouts GL_ARB_ES2_compatibility GL_ARB_ES3_compatibility GL_ARB_ES3_1_compatibility GL_ARB_ES3_2_compatibility GL_ARB_explicit_attrib_location GL_ARB_explicit_uniform_location GL_ARB_fragment_coord_conventions GL_ARB_fragment_layer_viewport GL_ARB_fragment_program GL_ARB_fragment_program_shadow GL_ARB_fragment_shader GL_ARB_fragment_shader_interlock GL_ARB_framebuffer_no_attachments GL_ARB_framebuffer_object GL_ARB_framebuffer_sRGB GL_ARB_geometry_shader4 GL_ARB_get_program_binary GL_ARB_get_texture_sub_image GL_ARB_gl_spirv GL_ARB_gpu_shader5 GL_ARB_gpu_shader_fp64 GL_ARB_gpu_shader_int64 GL_ARB_half_float_pixel GL_ARB_half_float_vertex GL_ARB_imaging GL_ARB_indirect_parameters GL_ARB_instanced_arrays GL_ARB_internalformat_query GL_ARB_internalformat_query2 GL_ARB_invalidate_subdata GL_ARB_map_buffer_alignment GL_ARB_map_buffer_range GL_ARB_multi_bind GL_ARB_multi_draw_indirect GL_ARB_multisample GL_ARB_multitexture GL_ARB_occlusion_query GL_ARB_occlusion_query2 GL_ARB_parallel_shader_compile GL_ARB_pipeline_statistics_query GL_ARB_pixel_buffer_object GL_ARB_point_parameters GL_ARB_point_sprite GL_ARB_polygon_offset_clamp GL_ARB_post_depth_coverage GL_ARB_program_interface_query GL_ARB_provoking_vertex GL_ARB_query_buffer_object GL_ARB_robust_buffer_access_behavior GL_ARB_robustness GL_ARB_sample_locations GL_ARB_sample_shading GL_ARB_sampler_objects GL_ARB_seamless_cube_map GL_ARB_seamless_cubemap_per_texture GL_ARB_separate_shader_objects GL_ARB_shader_atomic_counter_ops GL_ARB_shader_atomic_counters GL_ARB_shader_ballot GL_ARB_shader_bit_encoding GL_ARB_shader_clock GL_ARB_shader_draw_parameters GL_ARB_shader_group_vote GL_ARB_shader_image_load_store GL_ARB_shader_image_size GL_ARB_shader_objects GL_ARB_shader_precision GL_ARB_shader_storage_buffer_object GL_ARB_shader_subroutine GL_ARB_shader_texture_image_samples GL_ARB_shader_texture_lod GL_ARB_shading_language_100 GL_ARB_shader_viewport_layer_array GL_ARB_shading_language_420pack GL_ARB_shading_language_include GL_ARB_shading_language_packing GL_ARB_shadow GL_ARB_sparse_buffer GL_ARB_sparse_texture GL_ARB_sparse_texture2 GL_ARB_sparse_texture_clamp GL_ARB_spirv_extensions GL_ARB_stencil_texturing GL_ARB_sync GL_ARB_tessellation_shader GL_ARB_texture_barrier GL_ARB_texture_border_clamp GL_ARB_texture_buffer_object GL_ARB_texture_buffer_object_rgb32 GL_ARB_texture_buffer_range GL_ARB_texture_compression GL_ARB_texture_compression_bptc GL_ARB_texture_compression_rgtc GL_ARB_texture_cube_map GL_ARB_texture_cube_map_array GL_ARB_texture_env_add GL_ARB_texture_env_combine GL_ARB_texture_env_crossbar GL_ARB_texture_env_dot3 GL_ARB_texture_filter_anisotropic GL_ARB_texture_filter_minmax GL_ARB_texture_float GL_ARB_texture_gather GL_ARB_texture_mirror_clamp_to_edge GL_ARB_texture_mirrored_repeat GL_ARB_texture_multisample GL_ARB_texture_non_power_of_two GL_ARB_texture_query_levels GL_ARB_texture_query_lod GL_ARB_texture_rectangle GL_ARB_texture_rg GL_ARB_texture_rgb10_a2ui GL_ARB_texture_stencil8 GL_ARB_texture_storage GL_ARB_texture_storage_multisample GL_ARB_texture_swizzle GL_ARB_texture_view GL_ARB_timer_query GL_ARB_transform_feedback2 GL_ARB_transform_feedback3 GL_ARB_transform_feedback_instanced GL_ARB_transform_feedback_overflow_query GL_ARB_transpose_matrix GL_ARB_uniform_buffer_object GL_ARB_vertex_array_bgra GL_ARB_vertex_array_object GL_ARB_vertex_attrib_64bit GL_ARB_vertex_attrib_binding GL_ARB_vertex_buffer_object GL_ARB_vertex_program GL_ARB_vertex_shader GL_ARB_vertex_type_10f_11f_11f_rev GL_ARB_vertex_type_2_10_10_10_rev GL_ARB_viewport_array GL_ARB_window_pos GL_ATI_draw_buffers GL_ATI_texture_float GL_ATI_texture_mirror_once GL_S3_s3tc GL_EXT_texture_env_add GL_EXT_abgr GL_EXT_bgra GL_EXT_bindable_uniform GL_EXT_blend_color GL_EXT_blend_equation_separate GL_EXT_blend_func_separate GL_EXT_blend_minmax GL_EXT_blend_subtract GL_EXT_compiled_vertex_array GL_EXT_Cg_shader GL_EXT_depth_bounds_test GL_EXT_direct_state_access GL_EXT_draw_buffers2 GL_EXT_draw_instanced GL_EXT_draw_range_elements GL_EXT_fog_coord GL_EXT_framebuffer_blit GL_EXT_framebuffer_multisample GL_EXTX_framebuffer_mixed_formats GL_EXT_framebuffer_multisample_blit_scaled GL_EXT_framebuffer_object GL_EXT_framebuffer_sRGB GL_EXT_geometry_shader4 GL_EXT_gpu_program_parameters GL_EXT_gpu_shader4 GL_EXT_multi_draw_arrays GL_EXT_multiview_texture_multisample GL_EXT_multiview_timer_query GL_EXT_packed_depth_stencil GL_EXT_packed_float GL_EXT_packed_pixels GL_EXT_pixel_buffer_object GL_EXT_point_parameters GL_EXT_polygon_offset_clamp GL_EXT_post_depth_coverage GL_EXT_provoking_vertex GL_EXT_raster_multisample GL_EXT_rescale_normal GL_EXT_secondary_color GL_EXT_separate_shader_objects GL_EXT_separate_specular_color GL_EXT_shader_image_load_formatted GL_EXT_shader_image_load_store GL_EXT_shader_integer_mix GL_EXT_shadow_funcs GL_EXT_sparse_texture2 GL_EXT_stencil_two_side GL_EXT_stencil_wrap GL_EXT_texture3D GL_EXT_texture_array GL_EXT_texture_buffer_object GL_EXT_texture_compression_dxt1 GL_EXT_texture_compression_latc GL_EXT_texture_compression_rgtc GL_EXT_texture_compression_s3tc GL_EXT_texture_cube_map GL_EXT_texture_edge_clamp GL_EXT_texture_env_combine GL_EXT_texture_env_dot3 GL_EXT_texture_filter_anisotropic GL_EXT_texture_filter_minmax GL_EXT_texture_integer GL_EXT_texture_lod GL_EXT_texture_lod_bias GL_EXT_texture_mirror_clamp GL_EXT_texture_object GL_EXT_texture_shadow_lod GL_EXT_texture_shared_exponent GL_EXT_texture_sRGB GL_EXT_texture_sRGB_R8 GL_EXT_texture_sRGB_decode GL_EXT_texture_storage GL_EXT_texture_swizzle GL_EXT_timer_query GL_EXT_transform_feedback2 GL_EXT_vertex_array GL_EXT_vertex_array_bgra GL_EXT_vertex_attrib_64bit GL_EXT_window_rectangles GL_EXT_x11_sync_object GL_EXT_import_sync_object GL_NV_robustness_video_memory_purge GL_IBM_rasterpos_clip GL_IBM_texture_mirrored_repeat GL_KHR_context_flush_control GL_KHR_debug GL_EXT_memory_object GL_EXT_memory_object_fd GL_NV_memory_object_sparse GL_KHR_parallel_shader_compile GL_KHR_no_error GL_KHR_robust_buffer_access_behavior GL_KHR_robustness GL_EXT_semaphore GL_EXT_semaphore_fd GL_NV_timeline_semaphore GL_KHR_shader_subgroup GL_KTX_buffer_region GL_NV_alpha_to_coverage_dither_control GL_NV_bindless_multi_draw_indirect GL_NV_bindless_multi_draw_indirect_count GL_NV_bindless_texture GL_NV_blend_equation_advanced GL_NV_blend_equation_advanced_coherent GL_NVX_blend_equation_advanced_multi_draw_buffers GL_NV_blend_minmax_factor GL_NV_blend_square GL_NV_clip_space_w_scaling GL_NV_command_list GL_NV_compute_program5 GL_NV_compute_shader_derivatives GL_NV_conditional_render GL_NV_conservative_raster GL_NV_conservative_raster_dilate GL_NV_conservative_raster_pre_snap GL_NV_conservative_raster_pre_snap_triangles GL_NV_conservative_raster_underestimation GL_NV_copy_depth_to_color GL_NV_copy_image GL_NV_depth_buffer_float GL_NV_depth_clamp GL_NV_draw_texture GL_NV_draw_vulkan_image GL_NV_ES1_1_compatibility GL_NV_ES3_1_compatibility GL_NV_explicit_multisample GL_NV_feature_query GL_NV_fence GL_NV_fill_rectangle GL_NV_float_buffer GL_NV_fog_distance GL_NV_fragment_coverage_to_color GL_NV_fragment_program GL_NV_fragment_program_option GL_NV_fragment_program2 GL_NV_fragment_shader_barycentric GL_NV_fragment_shader_interlock GL_NV_framebuffer_mixed_samples GL_NV_framebuffer_multisample_coverage GL_NV_geometry_shader4 GL_NV_geometry_shader_passthrough GL_NV_gpu_program4 GL_NV_internalformat_sample_query GL_NV_gpu_program4_1 GL_NV_gpu_program5 GL_NV_gpu_program5_mem_extended GL_NV_gpu_program_fp64 GL_NV_gpu_shader5 GL_NV_half_float GL_NV_light_max_exponent GL_NV_memory_attachment GL_NV_mesh_shader GL_NV_multisample_coverage GL_NV_multisample_filter_hint GL_NV_occlusion_query GL_NV_packed_depth_stencil GL_NV_parameter_buffer_object GL_NV_parameter_buffer_object2 GL_NV_path_rendering GL_NV_path_rendering_shared_edge GL_NV_pixel_data_range GL_NV_point_sprite GL_NV_primitive_restart GL_NV_query_resource GL_NV_query_resource_tag GL_NV_register_combiners GL_NV_register_combiners2 GL_NV_representative_fragment_test GL_NV_sample_locations GL_NV_sample_mask_override_coverage GL_NV_scissor_exclusive GL_NV_shader_atomic_counters GL_NV_shader_atomic_float GL_NV_shader_atomic_float64 GL_NV_shader_atomic_fp16_vector GL_NV_shader_atomic_int64 GL_NV_shader_buffer_load GL_NV_shader_storage_buffer_object GL_NV_shader_subgroup_partitioned GL_NV_shader_texture_footprint GL_NV_shading_rate_image GL_NV_stereo_view_rendering GL_NV_texgen_reflection GL_NV_texture_barrier GL_NV_texture_compression_vtc GL_NV_texture_env_combine4 GL_NV_texture_multisample GL_NV_texture_rectangle GL_NV_texture_rectangle_compressed GL_NV_texture_shader GL_NV_texture_shader2 GL_NV_texture_shader3 GL_NV_transform_feedback GL_NV_transform_feedback2 GL_NV_uniform_buffer_unified_memory GL_NV_vdpau_interop GL_NV_vdpau_interop2 GL_NV_vertex_array_range GL_NV_vertex_array_range2 GL_NV_vertex_attrib_integer_64bit GL_NV_vertex_buffer_unified_memory GL_NV_vertex_program GL_NV_vertex_program1_1 GL_NV_vertex_program2 GL_NV_vertex_program2_option GL_NV_vertex_program3 GL_NV_viewport_array2 GL_NV_viewport_swizzle GL_NVX_conditional_render GL_NV_gpu_multicast GL_NVX_progress_fence GL_NVX_gpu_memory_info GL_NVX_nvenc_interop GL_NV_shader_thread_group GL_NV_shader_thread_shuffle GL_KHR_blend_equation_advanced GL_KHR_blend_equation_advanced_coherent GL_OVR_multiview GL_OVR_multiview2 GL_SGIS_generate_mipmap GL_SGIS_texture_lod GL_SGIX_depth_texture GL_SGIX_shadow GL_SUN_slice_accum
[   0.376][v][libmpv_render] Loaded extension GL_ARB_get_program_binary.
[   0.376][v][libmpv_render] Loaded extension GL_ARB_buffer_storage.
[   0.376][v][libmpv_render] Loaded extension GL_ARB_shader_image_load_store.
[   0.376][v][libmpv_render] Loaded extension GL_ARB_shader_storage_buffer_object.
[   0.376][v][libmpv_render] Loaded extension GL_ARB_compute_shader.
[   0.376][v][libmpv_render] Loaded extension GL_ARB_arrays_of_arrays.
[   0.376][v][libmpv_render] Loaded extension GL_NV_vdpau_interop.
[   0.376][v][libmpv_render] Loaded extension GL_ARB_debug_output.
[   0.376][v][libmpv_render] GL_*_swap_control extension missing.
[   0.376][d][libmpv_render] Texture formats:
[   0.376][d][libmpv_render]   NAME       COMP*TYPE SIZE           DEPTH PER COMP.
[   0.376][d][libmpv_render]   r8         1*unorm   1B    LF CR ST {8}
[   0.376][d][libmpv_render]   rg8        2*unorm   2B    LF CR ST {8 8}
[   0.376][d][libmpv_render]   rgb8       3*unorm   3B    LF CR ST {8 8 8}
[   0.376][d][libmpv_render]   rgba8      4*unorm   4B    LF CR ST {8 8 8 8}
[   0.376][d][libmpv_render]   r16        1*unorm   2B    LF CR ST {16}
[   0.376][d][libmpv_render]   rg16       2*unorm   4B    LF CR ST {16 16}
[   0.376][d][libmpv_render]   rgb16      3*unorm   6B    LF CR ST {16 16 16}
[   0.376][d][libmpv_render]   rgba16     4*unorm   8B    LF CR ST {16 16 16 16}
[   0.376][d][libmpv_render]   r8ui       1*uint    1B       CR ST {8}
[   0.376][d][libmpv_render]   rg8ui      2*uint    2B       CR ST {8 8}
[   0.376][d][libmpv_render]   rgb8ui     3*uint    3B          ST {8 8 8}
[   0.376][d][libmpv_render]   rgba8ui    4*uint    4B       CR ST {8 8 8 8}
[   0.376][d][libmpv_render]   r16ui      1*uint    2B       CR ST {16}
[   0.376][d][libmpv_render]   rg16ui     2*uint    4B       CR ST {16 16}
[   0.376][d][libmpv_render]   rgb16ui    3*uint    6B          ST {16 16 16}
[   0.376][d][libmpv_render]   rgba16ui   4*uint    8B       CR ST {16 16 16 16}
[   0.376][d][libmpv_render]   r16f       1*float   4B    LF CR ST {32/16}
[   0.376][d][libmpv_render]   rg16f      2*float   8B    LF CR ST {32/16 32/16}
[   0.376][d][libmpv_render]   rgb16f     3*float  12B    LF CR ST {32/16 32/16 32/16}
[   0.376][d][libmpv_render]   rgba16f    4*float  16B    LF CR ST {32/16 32/16 32/16 32/16}
[   0.376][d][libmpv_render]   r32f       1*float   4B    LF CR ST {32}
[   0.376][d][libmpv_render]   rg32f      2*float   8B    LF CR ST {32 32}
[   0.376][d][libmpv_render]   rgb32f     3*float  12B    LF CR ST {32 32 32}
[   0.376][d][libmpv_render]   rgba32f    4*float  16B    LF CR ST {32 32 32 32}
[   0.376][d][libmpv_render]   rgb10_a2   4*unorm   4B    LF CR ST {0/10 0/10 0/10 0/2}
[   0.376][d][libmpv_render]   rgba12     4*unorm   8B    LF CR ST {16 16 16 16}
[   0.376][d][libmpv_render]   rgb10      3*unorm   6B    LF CR ST {16/10 16/10 16/10}
[   0.376][d][libmpv_render]   rgb565     3*unorm   2B    LF    ST {0/8 0/8 0/8}
[   0.376][d][libmpv_render]  LA = LUMINANCE_ALPHA hack format
[   0.376][d][libmpv_render]  LF = linear filterable
[   0.376][d][libmpv_render]  CR = can be used for render targets
[   0.376][d][libmpv_render]  ST = can be used for storable images
[   0.376][d][libmpv_render] Image formats:
[   0.376][d][libmpv_render]   yuv444p => 3 planes 1x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv420p => 3 planes 2x2 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   gray => 1 planes 1x1 8/0 [r8] (r) [unorm]
[   0.376][d][libmpv_render]   gray16 => 1 planes 1x1 16/0 [r16] (r) [unorm]
[   0.376][d][libmpv_render]   uyvy422
[   0.376][d][libmpv_render]   nv12 => 2 planes 2x2 8/0 [r8/rg8] (r/gb) [unorm]
[   0.376][d][libmpv_render]   p010 => 2 planes 2x2 16/6 [r16/rg16] (r/gb) [unorm]
[   0.376][d][libmpv_render]   argb => 1 planes 1x1 8/0 [rgba8] (argb) [unorm]
[   0.376][d][libmpv_render]   bgra => 1 planes 1x1 8/0 [rgba8] (bgra) [unorm]
[   0.376][d][libmpv_render]   abgr => 1 planes 1x1 8/0 [rgba8] (abgr) [unorm]
[   0.376][d][libmpv_render]   rgba => 1 planes 1x1 8/0 [rgba8] (rgba) [unorm]
[   0.376][d][libmpv_render]   bgr24 => 1 planes 1x1 8/0 [rgb8] (bgr) [unorm]
[   0.376][d][libmpv_render]   rgb24 => 1 planes 1x1 8/0 [rgb8] (rgb) [unorm]
[   0.376][d][libmpv_render]   0rgb => 1 planes 1x1 8/0 [rgba8] (_rgb) [unorm]
[   0.376][d][libmpv_render]   bgr0 => 1 planes 1x1 8/0 [rgba8] (bgr) [unorm]
[   0.376][d][libmpv_render]   0bgr => 1 planes 1x1 8/0 [rgba8] (_bgr) [unorm]
[   0.376][d][libmpv_render]   rgb0 => 1 planes 1x1 8/0 [rgba8] (rgb) [unorm]
[   0.376][d][libmpv_render]   rgba64 => 1 planes 1x1 16/0 [rgba16] (rgba) [unorm]
[   0.376][d][libmpv_render]   rgb565 => 1 planes 1x1 0/0 [rgb565] (rgb) [unknown]
[   0.376][d][libmpv_render]   pal8
[   0.376][d][libmpv_render]   vdpau
[   0.376][d][libmpv_render]   d3d11
[   0.376][d][libmpv_render]   dxva2_vld
[   0.376][d][libmpv_render]   mmal
[   0.376][d][libmpv_render]   mediacodec
[   0.376][d][libmpv_render]   drm_prime
[   0.376][d][libmpv_render]   cuda
[   0.376][d][libmpv_render]   yap8 => 2 planes 1x1 8/0 [r8/r8] (r/a) [unorm]
[   0.376][d][libmpv_render]   yap16 => 2 planes 1x1 16/0 [r16/r16] (r/a) [unorm]
[   0.376][d][libmpv_render]   grayaf32 => 2 planes 1x1 32/0 [r16f/r16f] (r/a) [float]
[   0.376][d][libmpv_render]   yuv444pf => 3 planes 1x1 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.376][d][libmpv_render]   yuva444pf => 4 planes 1x1 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.376][d][libmpv_render]   yuv420pf => 3 planes 2x2 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.376][d][libmpv_render]   yuva420pf => 4 planes 2x2 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.376][d][libmpv_render]   yuv422pf => 3 planes 2x1 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.376][d][libmpv_render]   yuva422pf => 4 planes 2x1 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.376][d][libmpv_render]   yuv440pf => 3 planes 1x2 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.376][d][libmpv_render]   yuva440pf => 4 planes 1x2 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.376][d][libmpv_render]   yuv410pf => 3 planes 4x4 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.376][d][libmpv_render]   yuva410pf => 4 planes 4x4 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.376][d][libmpv_render]   yuv411pf => 3 planes 4x1 32/0 [r16f/r16f/r16f] (r/g/b) [float]
[   0.376][d][libmpv_render]   yuva411pf => 4 planes 4x1 32/0 [r16f/r16f/r16f/r16f] (r/g/b/a) [float]
[   0.376][d][libmpv_render]   rgb30 => 1 planes 1x1 10/0 [rgb10_a2] (bgr) [unknown]
[   0.376][d][libmpv_render]   y1 => 1 planes 1x1 8/-7 [r8] (r) [unorm]
[   0.376][d][libmpv_render]   gbrp1 => 3 planes 1x1 8/-7 [r8/r8/r8] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp2 => 3 planes 1x1 8/-6 [r8/r8/r8] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp3 => 3 planes 1x1 8/-5 [r8/r8/r8] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp4 => 3 planes 1x1 8/-4 [r8/r8/r8] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp5 => 3 planes 1x1 8/-3 [r8/r8/r8] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp6 => 3 planes 1x1 8/-2 [r8/r8/r8] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   vdpau_output
[   0.376][d][libmpv_render]   vaapi
[   0.376][d][libmpv_render]   videotoolbox
[   0.376][d][libmpv_render]   yuyv422
[   0.376][d][libmpv_render]   yuv422p => 3 planes 2x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv410p => 3 planes 4x4 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv411p => 3 planes 4x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   monow
[   0.376][d][libmpv_render]   monob
[   0.376][d][libmpv_render]   yuvj422p => 3 planes 2x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   uyyvyy411
[   0.376][d][libmpv_render]   bgr8
[   0.376][d][libmpv_render]   bgr4
[   0.376][d][libmpv_render]   bgr4_byte
[   0.376][d][libmpv_render]   rgb8
[   0.376][d][libmpv_render]   rgb4
[   0.376][d][libmpv_render]   rgb4_byte
[   0.376][d][libmpv_render]   nv21 => 2 planes 2x2 8/0 [r8/rg8] (r/bg) [unorm]
[   0.376][d][libmpv_render]   gray16be
[   0.376][d][libmpv_render]   yuv440p => 3 planes 1x2 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuvj440p => 3 planes 1x2 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuva420p => 4 planes 2x2 8/0 [r8/r8/r8/r8] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   rgb48be
[   0.376][d][libmpv_render]   rgb48 => 1 planes 1x1 16/0 [rgb16] (rgb) [unorm]
[   0.376][d][libmpv_render]   rgb565be
[   0.376][d][libmpv_render]   rgb555be
[   0.376][d][libmpv_render]   rgb555
[   0.376][d][libmpv_render]   bgr565be
[   0.376][d][libmpv_render]   bgr565
[   0.376][d][libmpv_render]   bgr555be
[   0.376][d][libmpv_render]   bgr555
[   0.376][d][libmpv_render]   vaapi_moco
[   0.376][d][libmpv_render]   vaapi_idct
[   0.376][d][libmpv_render]   yuv420p16 => 3 planes 2x2 16/0 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv420p16be
[   0.376][d][libmpv_render]   yuv422p16 => 3 planes 2x1 16/0 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv422p16be
[   0.376][d][libmpv_render]   yuv444p16 => 3 planes 1x1 16/0 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv444p16be
[   0.376][d][libmpv_render]   rgb444
[   0.376][d][libmpv_render]   rgb444be
[   0.376][d][libmpv_render]   bgr444
[   0.376][d][libmpv_render]   bgr444be
[   0.376][d][libmpv_render]   ya8 => 1 planes 1x1 8/0 [rg8] (ra) [unorm]
[   0.376][d][libmpv_render]   bgr48be
[   0.376][d][libmpv_render]   bgr48 => 1 planes 1x1 16/0 [rgb16] (bgr) [unorm]
[   0.376][d][libmpv_render]   yuv420p9be
[   0.376][d][libmpv_render]   yuv420p9 => 3 planes 2x2 16/-7 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv420p10be
[   0.376][d][libmpv_render]   yuv420p10 => 3 planes 2x2 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv422p10be
[   0.376][d][libmpv_render]   yuv422p10 => 3 planes 2x1 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv444p9be
[   0.376][d][libmpv_render]   yuv444p9 => 3 planes 1x1 16/-7 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv444p10be
[   0.376][d][libmpv_render]   yuv444p10 => 3 planes 1x1 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv422p9be
[   0.376][d][libmpv_render]   yuv422p9 => 3 planes 2x1 16/-7 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   gbrp => 3 planes 1x1 8/0 [r8/r8/r8] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp9be
[   0.376][d][libmpv_render]   gbrp9 => 3 planes 1x1 16/-7 [r16/r16/r16] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp10be
[   0.376][d][libmpv_render]   gbrp10 => 3 planes 1x1 16/-6 [r16/r16/r16] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp16be
[   0.376][d][libmpv_render]   gbrp16 => 3 planes 1x1 16/0 [r16/r16/r16] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   yuva422p => 4 planes 2x1 8/0 [r8/r8/r8/r8] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva444p => 4 planes 1x1 8/0 [r8/r8/r8/r8] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva420p9be
[   0.376][d][libmpv_render]   yuva420p9 => 4 planes 2x2 16/-7 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva422p9be
[   0.376][d][libmpv_render]   yuva422p9 => 4 planes 2x1 16/-7 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva444p9be
[   0.376][d][libmpv_render]   yuva444p9 => 4 planes 1x1 16/-7 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva420p10be
[   0.376][d][libmpv_render]   yuva420p10 => 4 planes 2x2 16/-6 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva422p10be
[   0.376][d][libmpv_render]   yuva422p10 => 4 planes 2x1 16/-6 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva444p10be
[   0.376][d][libmpv_render]   yuva444p10 => 4 planes 1x1 16/-6 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva420p16be
[   0.376][d][libmpv_render]   yuva420p16 => 4 planes 2x2 16/0 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva422p16be
[   0.376][d][libmpv_render]   yuva422p16 => 4 planes 2x1 16/0 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   yuva444p16be
[   0.376][d][libmpv_render]   yuva444p16 => 4 planes 1x1 16/0 [r16/r16/r16/r16] (r/g/b/a) [unorm]
[   0.376][d][libmpv_render]   xyz12 => 1 planes 1x1 16/4 [rgb16] (rgb) [unorm]
[   0.376][d][libmpv_render]   xyz12be
[   0.376][d][libmpv_render]   nv16 => 2 planes 2x1 8/0 [r8/rg8] (r/gb) [unorm]
[   0.376][d][libmpv_render]   nv20 => 2 planes 2x1 16/-6 [r16/rg16] (r/gb) [unorm]
[   0.376][d][libmpv_render]   nv20be
[   0.376][d][libmpv_render]   rgba64be
[   0.376][d][libmpv_render]   bgra64be
[   0.376][d][libmpv_render]   bgra64 => 1 planes 1x1 16/0 [rgba16] (bgra) [unorm]
[   0.376][d][libmpv_render]   yvyu422
[   0.376][d][libmpv_render]   ya16be
[   0.376][d][libmpv_render]   ya16 => 1 planes 1x1 16/0 [rg16] (ra) [unorm]
[   0.376][d][libmpv_render]   gbrap => 4 planes 1x1 8/0 [r8/r8/r8/r8] (g/b/r/a) [unorm]
[   0.376][d][libmpv_render]   gbrap16be
[   0.376][d][libmpv_render]   gbrap16 => 4 planes 1x1 16/0 [r16/r16/r16/r16] (g/b/r/a) [unorm]
[   0.376][d][libmpv_render]   qsv
[   0.376][d][libmpv_render]   d3d11va_vld
[   0.376][d][libmpv_render]   yuv420p12be
[   0.376][d][libmpv_render]   yuv420p12 => 3 planes 2x2 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv420p14be
[   0.376][d][libmpv_render]   yuv420p14 => 3 planes 2x2 16/-2 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv422p12be
[   0.376][d][libmpv_render]   yuv422p12 => 3 planes 2x1 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv422p14be
[   0.376][d][libmpv_render]   yuv422p14 => 3 planes 2x1 16/-2 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv444p12be
[   0.376][d][libmpv_render]   yuv444p12 => 3 planes 1x1 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv444p14be
[   0.376][d][libmpv_render]   yuv444p14 => 3 planes 1x1 16/-2 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   gbrp12be
[   0.376][d][libmpv_render]   gbrp12 => 3 planes 1x1 16/-4 [r16/r16/r16] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   gbrp14be
[   0.376][d][libmpv_render]   gbrp14 => 3 planes 1x1 16/-2 [r16/r16/r16] (g/b/r) [unorm]
[   0.376][d][libmpv_render]   yuvj411p => 3 planes 4x1 8/0 [r8/r8/r8] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   bayer_bggr8
[   0.376][d][libmpv_render]   bayer_rggb8
[   0.376][d][libmpv_render]   bayer_gbrg8
[   0.376][d][libmpv_render]   bayer_grbg8
[   0.376][d][libmpv_render]   bayer_bggr16
[   0.376][d][libmpv_render]   bayer_bggr16be
[   0.376][d][libmpv_render]   bayer_rggb16
[   0.376][d][libmpv_render]   bayer_rggb16be
[   0.376][d][libmpv_render]   bayer_gbrg16
[   0.376][d][libmpv_render]   bayer_gbrg16be
[   0.376][d][libmpv_render]   bayer_grbg16
[   0.376][d][libmpv_render]   bayer_grbg16be
[   0.376][d][libmpv_render]   xvmc
[   0.376][d][libmpv_render]   yuv440p10 => 3 planes 1x2 16/-6 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv440p10be
[   0.376][d][libmpv_render]   yuv440p12 => 3 planes 1x2 16/-4 [r16/r16/r16] (r/g/b) [unorm]
[   0.376][d][libmpv_render]   yuv440p12be
[   0.376][d][libmpv_render]   ayuv64 => 1 planes 1x1 16/0 [rgba16] (argb) [unorm]
[   0.376][d][libmpv_render]   ayuv64be
[   0.376][d][libmpv_render]   p010be
[   0.376][d][libmpv_render]   gbrap12be
[   0.376][d][libmpv_render]   gbrap12 => 4 planes 1x1 16/-4 [r16/r16/r16/r16] (g/b/r/a) [unorm]
[   0.376][d][libmpv_render]   gbrap10be
[   0.376][d][libmpv_render]   gbrap10 => 4 planes 1x1 16/-6 [r16/r16/r16/r16] (g/b/r/a) [unorm]
[   0.376][d][libmpv_render]   gray12be
[   0.376][d][libmpv_render]   gray12 => 1 planes 1x1 16/-4 [r16] (r) [unorm]
[   0.376][d][libmpv_render]   gray10be
[   0.376][d][libmpv_render]   gray10 => 1 planes 1x1 16/-6 [r16] (r) [unorm]
[   0.376][d][libmpv_render]   p016 => 2 planes 2x2 16/0 [r16/rg16] (r/gb) [unorm]
[   0.376][d][libmpv_render]   p016be
[   0.376][d][libmpv_render]   gray9be
[   0.376][d][libmpv_render]   gray9 => 1 planes 1x1 16/-7 [r16] (r) [unorm]
[   0.376][d][libmpv_render]   gbrpf32be
[   0.376][d][libmpv_render]   gbrpf32 => 3 planes 1x1 32/0 [r16f/r16f/r16f] (g/b/r) [float]
[   0.376][d][libmpv_render]   gbrapf32be
[   0.376][d][libmpv_render]   gbrapf32 => 4 planes 1x1 32/0 [r16f/r16f/r16f/r16f] (g/b/r/a) [float]
[   0.376][d][libmpv_render]   opencl
[   0.376][d][libmpv_render]   gray14be
[   0.376][d][libmpv_render]   gray14 => 1 planes 1x1 16/-2 [r16] (r) [unorm]
[   0.376][d][libmpv_render]   grayf32be
[   0.376][d][libmpv_render]   grayf32 => 1 planes 1x1 32/0 [r16f] (r) [float]
[   0.376][v][libmpv_render] Testing FBO format rgba16f
[   0.376][d][libmpv_render] Resizing texture: 16x16
[   0.376][e][libmpv_render] after creating texture: OpenGL error INVALID_ENUM.
[   0.377][v][libmpv_render] Using FBO format rgba16f.
[   0.377][v][libmpv_render] Disabling HDR peak computation (one or more of the following is not supported: compute shaders=0, SSBO=1).
[   0.377][v][libmpv_render] Loading hwdec driver 'vaapi-egl'
[   0.377][v][libmpv_render/vaapi-egl] VAAPI hwdec only works with OpenGL or Vulkan backends.
[   0.377][v][libmpv_render] Loading failed.
[   0.377][v][libmpv_render] Loading hwdec driver 'vdpau-gl'
[   0.410][v][libmpv_render] Loading hwdec driver 'drmprime-drm'
[   0.410][v][libmpv_render/drmprime-drm] Failed to retrieve DRM fd from native display.
[   0.410][v][libmpv_render] Loading failed.
[   0.487][d][ffmpeg/demuxer] rtsp: setting jitter buffer size to 0
[   0.489][d][ffmpeg/demuxer] rtsp: setting jitter buffer size to 0


~~~

#### 版本

* mpv版本：0.34.1
* FFmpeg library versions:
    *    libavutil       56.22.100
    *    libavcodec      58.35.100
    *    libavformat     58.20.100
    *    libswscale      5.3.100
    *    libavfilter     7.40.101
    *    libswresample   3.3.100
* FFmpeg version: 4.1.9-deepin13


#### 分析

由于mpv本身输出的log太少且对于定位问题没有帮助，我们拉取了0.34版本的源代码，并添加相关log输出，然后手动编译 libmpv.so 。通过漫长和繁琐的排查，最终发现异常发生在 video/out/opengl/common.c/mpgl_load_functions2 方法中。具体表现为当给 gl.extensions 赋值的时候程序发生异常，主启动程序崩溃，在此行后添加的log都没有输出。我们尝试为 ext2 这个指针做判空处理，更加匪夷所思的是无论此指针是否为空，都会偶发性导致主启动程序崩溃。由于我们不是 c 语言的开发者，不太懂 c 语言中的指针，我们无法做更深入的分析。核心代码如下：

video/out/opengl/common.c

~~~


// Fill the GL struct with function pointers and extensions from the current
// GL context. Called by the backend.
// get_fn: function to resolve function names
// ext2: an extra extension string
// log: used to output messages
void mpgl_load_functions2(GL *gl, void *(*get_fn)(void *ctx, const char *n),
                          void *fn_ctx, const char *ext2, struct mp_log *log)
{
    talloc_free(gl->extensions);
    *gl = (GL) {
        .extensions = talloc_strdup(gl, ext2 ? ext2 : ""),
    };

   ......

}

~~~

#### 猜测和可能的解决方案

* 猜测此问题可能和并发有关，因为如果仅播放一个视频，不会出现此问题
* 我们发现 ext2 无论是否为空，都会偶发性导致主启动程序崩溃，并且源码中有对 ext2 做三元判断，为空会赋值空字符串，所以我们索性在进入此方法的时候将 ext2 赋值为空字符串。然后将源码进行编译，重新进行大量测试，搞笑的是 `崩溃` 问题没有再出现，但是会偶发性发生16个播放器窗格，有一个为黑色（无法播放视频），尚不清楚此问题是否为改动代码后引发的问题。



## windows



## macOS




