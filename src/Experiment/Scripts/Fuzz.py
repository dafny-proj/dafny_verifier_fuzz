import Dafny
import Env
import MutantGenerator
import os
import os.path as path
import pathlib
import shutil
import subprocess

''' Notes:
Dafny Exit Codes.
- 0: Success
- 1: Preprocessing Error.
- 2: Dafny Error.
- 3: Compile Error.
- 4: Verification Error.
- 5: Format Error.

Assume for a `file.dfy`, its mutants will be placed in the directory 
`work_dir/file` under the name format `file_mutantseed_order.dfy`, 
with a corresponding log `file_mutantseed_order.dfy.log`.
'''


def writeOutcomeToFile(result: subprocess.CompletedProcess, file: str):
  with open(file, 'a') as f:
    f.write(f'Return code: {result.returncode}\n')
    f.write(f'Output:\n{result.stdout.decode()}')
    f.write(f'Error:\n{result.stderr.decode()}')


def getDafnyFilesInDir(dir: str):
  return [p.as_posix() for p in list(pathlib.Path(dir).glob('*.dfy'))]


def logPotentialBug(file: str):
  with open(Env.POTENTIAL_BUGS_FILE, 'a') as bug_file:
    bug_file.write(f'{path.basename(file)}\n')


def fuzzVerbose(seed_file: str):
  ''' Generates mutants from a seed, and cross-checks the verification outcome.

  - The verification outcome of the seed and mutants are recorded in 
    `filename.dfy.out`.
  - If the seed has other errors (e.g. parse/resolution errors), no mutants will
    be generated. The seed will be recorded in the potential bugs file.
  - If a mutant has a different outcome to the seed, it will be recorded in the 
    potential bugs file.
  '''
  seed_name_ext = path.basename(seed_file)
  seed_name = path.splitext(seed_name_ext)[0]
  out_dir = path.join(Env.FUZZ_DIR, seed_name)
  os.makedirs(out_dir, exist_ok=True)

  # Save a copy of the seed and its verification result.
  seed_out = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=seed_file)
  seed_file_copy = path.join(out_dir, seed_name_ext)
  seed_out_log = seed_file_copy + '.out'
  if not path.exists(seed_file_copy):
    shutil.copyfile(seed_file, seed_file_copy)
  if not path.exists(seed_out_log):
    writeOutcomeToFile(seed_out, seed_out_log)
  # Don't try to process seeds with preprocessing/parse/resolve errors.
  if seed_out.returncode != 0 and seed_out.returncode != 4:
    logPotentialBug(seed_file)
    return
  # Generate mutants from seed.
  MutantGenerator.runGenMulti(seed_file=seed_file, num_mutants=5, max_order=20,
                              work_dir=Env.FUZZ_DIR)
  # Try to verify mutants.
  dafny_files = getDafnyFilesInDir(out_dir)
  for file in dafny_files:
    out_log = file + '.out'
    if path.exists(out_log):
      # We already ran verification on this file previously, skip.
      continue
    out = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=file)
    writeOutcomeToFile(out, out_log)
    # Record mutants which produced an unexpected outcome.
    if out.returncode != seed_out.returncode:
      logPotentialBug(file)


def fuzz(seed_file: str):
  ''' Generates mutants from a seed, and cross-checks the verification outcome.

  - Only files (and the seed) behaving unexpectedly will be saved.
    Files will be saved in Env.BUGS_DIR/seed_name and in the potential bugs file.
    - `filename.dfy`: Either the seed or a mutant.
    - `filename.dfy.log`: Records how the mutant was generated.
    - `filename.dfy.out`: Records the verification outcome of the file.
  '''
  seed_name_ext = path.basename(seed_file)
  seed_name = path.splitext(seed_name_ext)[0]
  bug_dir = path.join(Env.BUGS_DIR, seed_name)

  seed_out = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=seed_file)
  seed_file_copy = path.join(bug_dir, seed_name_ext)
  seed_out_log = seed_file_copy + '.out'
  # Don't try to process seeds with preprocessing/parse/resolve errors.
  if seed_out.returncode != 0 and seed_out.returncode != 4:
    if not path.exists(bug_dir):
      os.makedirs(bug_dir)
      shutil.copyfile(seed_file, seed_file_copy)
      writeOutcomeToFile(seed_out, seed_out_log)
    logPotentialBug(seed_file)
    return
  # Generate mutants from seed.
  MutantGenerator.runGenMulti(seed_file=seed_file, num_mutants=5, max_order=20,
                              work_dir=Env.TMP_DIR)
  # Try to verify mutants.
  dafny_files = getDafnyFilesInDir(path.join(Env.TMP_DIR, seed_name))
  for file in dafny_files:
    out = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=file)
    # Record mutants which produced an unexpected outcome.
    if out.returncode != seed_out.returncode:
      if not path.exists(bug_dir):
        os.makedirs(bug_dir)
        shutil.copyfile(seed_file, seed_file_copy)
        writeOutcomeToFile(seed_out, seed_out_log)
      file_copy = path.join(Env.BUGS_DIR, path.basename(file))
      if not path.exists(file_copy):
        shutil.move(file, file_copy)
        shutil.move(file + '.log', file_copy + '.log')
        writeOutcomeToFile(out, file_copy + '.out')
        logPotentialBug(file)
  # Remove unused files.
  shutil.rmtree(Env.TMP_DIR)
