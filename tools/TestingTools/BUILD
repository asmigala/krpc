load('/tools/build/csharp', 'csharp_library', 'csharp_assembly_info')
load('/config', 'version', 'author')

csharp_library(
    name = 'TestingTools',
    srcs = glob(['**/*.cs']) + [':AssemblyInfo'],
    deps = [
        '//server:KRPC',
        '//service/SpaceCenter:KRPC.SpaceCenter',
        '//tools/build/ksp:Google.Protobuf',
        '//tools/build/ksp:Assembly-CSharp',
        '//tools/build/ksp:Assembly-CSharp-firstpass',
        '//tools/build/ksp:UnityEngine',
        '//tools/build/ksp:TDx.TDxInput',
        '//tools/build/ksp:mscorlib',
        '//tools/build/ksp:System',
        '//tools/build/ksp:System.Core',
        '//tools/build/ksp:System.Xml',
        '//tools/build/ksp:System.Xml.Linq'
    ],
    visibility = ['//visibility:public']
)

csharp_assembly_info(
    name = 'AssemblyInfo',
    title = 'TestingTools',
    version = version,
    copyright = author
)
