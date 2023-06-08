import os
import shutil
import subprocess


def genFromXDSmith(xdSmithDir, numFiles, outputDir):
  cwd = os.path.join(xdSmithDir, "xdsmith")
  baseCmd = ["racket", "fuzzer.rkt", "--dafny-syntax", "true"]
  for i in range(numFiles):
    outputFile = os.path.join(outputDir, f"xdsmith_{i}.dfy")
    cmd = baseCmd + ["--seed", str(i), "--output-file", outputFile]
    subprocess.run(cmd, cwd=cwd)


def genFromFuzzd(fuzzdDir, numFiles, outputDir):
  tmpDir = os.path.join(outputDir, "tmp")
  generatedDir = os.path.join(outputDir, "generated")
  reconditionedDir = os.path.join(outputDir, "reconditioned")
  os.makedirs(tmpDir, exist_ok=True)
  os.makedirs(generatedDir, exist_ok=True)
  os.makedirs(reconditionedDir, exist_ok=True)
  baseCmd = ["java", "-jar", os.path.join(fuzzdDir, "app/build/libs/app.jar"),
             "fuzz", "--noRun", "--verifier", f"--output", tmpDir]
  for i in range(numFiles):
    subprocess.run(baseCmd + ["--seed", str(i)], capture_output=True)
    fileName = f"fuzzd_{i}.dfy"
    generatedFile = os.path.join(tmpDir, "generated.dfy")
    reconditionedFile = os.path.join(tmpDir, "main.dfy")
    subprocess.run(["mv", generatedFile, os.path.join(generatedDir, fileName)])
    if (os.path.exists(reconditionedFile)):
      subprocess.run(
          ["mv", reconditionedFile, os.path.join(reconditionedDir, fileName)])
  shutil.rmtree(tmpDir)
