import os
import os.path as path
import pathlib
import shutil
import subprocess


def getDafnyFilesInDir(dir: str):
  return [p.as_posix() for p in list(pathlib.Path(dir).glob('*.dfy'))]


def genFromXDSmith(xdsmith_dir, num_files, output_dir):
  cwd = os.path.join(xdsmith_dir, 'xdsmith')
  base_cmd = ['racket', 'fuzzer.rkt', '--dafny-syntax', 'true']
  for i in range(num_files):
    output_file = os.path.join(path.abspath(output_dir), f'xdsmith_{i}.dfy')
    cmd = base_cmd + ['--seed', str(i), '--output-file', output_file]
    subprocess.run(cmd, cwd=cwd)


def genFromFuzzd(fuzzd_dir, num_files, output_dir):
  tmp_dir = os.path.join(output_dir, 'tmp')
  os.makedirs(tmp_dir, exist_ok=True)
  base_cmd = ['java', '-jar', os.path.join(fuzzd_dir, 'app/build/libs/app.jar'),
              'fuzz', '--noRun', '--verifier', f'--output', tmp_dir]
  for i in range(num_files):
    subprocess.run(base_cmd + ['--seed', str(i)], capture_output=True)
    file = path.join(output_dir, f'fuzzd_{i}.dfy')
    generated_file = os.path.join(tmp_dir, 'generated.dfy')
    reconditioned_file = os.path.join(tmp_dir, 'main.dfy')
    if path.exists(reconditioned_file):
      shutil.move(reconditioned_file, file)
    elif path.exists(generated_file):
      shutil.move(generated_file, file)
  shutil.rmtree(tmp_dir)


def genFromDafnyVerifier(dafnyverifier_dir, num_files, output_dir):
  base_cmd = ['java', '-cp', 'target/classes',
              'Main.VerificationProgramGeneration']
  output_dir = path.abspath(output_dir)
  correct_dir = path.join(dafnyverifier_dir, 'tests-minimized')
  incorrect_dir = path.join(dafnyverifier_dir, 'tests-incorrect')
  os.makedirs(output_dir, exist_ok=True)
  os.makedirs(correct_dir, exist_ok=True)
  os.makedirs(incorrect_dir, exist_ok=True)
  for i in range(num_files):
    seed = str(i)
    subprocess.run(base_cmd + [seed], cwd=dafnyverifier_dir)
    count = 0
    for file in getDafnyFilesInDir(correct_dir):
      shutil.move(file, path.join(
          output_dir, f'dafnyverifier_{seed}_s{count}.dfy'))
      count += 1
    count = 0
    for file in getDafnyFilesInDir(incorrect_dir):
      shutil.move(file, path.join(
          output_dir, f'dafnyverifier_{seed}_f{count}.dfy'))
      count += 1
