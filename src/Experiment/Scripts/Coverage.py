import subprocess
import os
import shlex

# dotnet tool install --global coverlet.console


def collect_coverage(dafnyDir, testFile):
  dafnyCoreAssembly = os.path.join(dafnyDir, 'Binaries', 'Dafny.dll')
  dafny = os.path.join(dafnyDir, 'Scripts', 'Dafny')
  cmd = f'coverlet {dafnyCoreAssembly} --target "{dafny}" --targetargs "verify --cores=2 {testFile}" --output "dcoverage/" -f json -f cobertura'
  args = shlex.split(cmd)
  print(args)
  subprocess.run(args)


# --output "/custom/directory/" -f json -f lcov
collect_coverage("/Users/wyt/Desktop/dafny_proj/dafny_sut",
                 "/Users/wyt/Desktop/dafny_proj/dafny_verifier_fuzz/examples/VerificationCornerL3.dfy")
