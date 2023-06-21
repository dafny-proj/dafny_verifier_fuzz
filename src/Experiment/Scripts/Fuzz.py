import Dafny
import Env
import MutantGenerator

import os
import os.path as path
import pathlib
import random
import shutil
import subprocess


def getDafnyFilesInDir(dir: str):
  return [p.as_posix() for p in list(pathlib.Path(dir).glob('*.dfy'))]


def logPotentialBug(file: str):
  with open(Env.POTENTIAL_BUGS_FILE, 'a') as bug_file:
    bug_file.write(f'{path.basename(file)}\n')


def logOutcome(result: subprocess.CompletedProcess, log_file: str):
  with open(log_file, 'a') as log:
    log.write(f'Return code: {result.returncode}\n')
    log.write(f'Output:\n{result.stdout.decode()}')
    log.write(f'Error:\n{result.stderr.decode()}')


def fuzz(seeds: list[str]):
  ''' Generates mutants from seeds, and cross-checks the verification outcome.

  - Only files (and the seed) behaving unexpectedly will be saved.
    Files will be saved in Env.BUGS_DIR/seed_name and in the potential bugs file.
    - `filename.dfy`: Either the seed or a mutant.
    - `filename.dfy.log`: Records how the mutant was generated.
    - `filename.dfy.out`: Records the verification outcome of the file.
  '''
  found_bug = False
  tries = 0
  while not found_bug:
    seed = random.choice(seeds)
    seed_name_ext = path.basename(seed)
    seed_name = path.splitext(seed_name_ext)[0]
    bug_dir = path.join(Env.BUGS_DIR, seed_name)

    seed_out = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=seed)
    seed_file_copy = path.join(bug_dir, seed_name_ext)
    seed_out_log = seed_file_copy + '.out'
    # Don't try to process seeds with preprocessing/parse/resolve errors.
    if seed_out.returncode != 0 and seed_out.returncode != 4:
      if not path.exists(bug_dir):
        os.makedirs(bug_dir)
        shutil.copyfile(seed, seed_file_copy)
        logOutcome(seed_out, seed_out_log)
      logPotentialBug(seed)
      return
    # Generate mutants from seed.
    MutantGenerator.runGenMulti(seed_file=seed, num_mutants=5, max_order=1,
                                work_dir=Env.TMP_DIR)
    # Try to verify mutants.
    dafny_files = getDafnyFilesInDir(path.join(Env.TMP_DIR, seed_name))
    for file in dafny_files:
      tries += 1
      out = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=file)
      # Record mutants which produced an unexpected outcome.
      if out.returncode != seed_out.returncode:
        if not path.exists(bug_dir):
          os.makedirs(bug_dir)
          shutil.copyfile(seed, seed_file_copy)
          logOutcome(seed_out, seed_out_log)
        file_copy = path.join(bug_dir, path.basename(file))
        if not path.exists(file_copy):
          shutil.move(file, file_copy)
          shutil.move(file + '.log', file_copy + '.log')
          logOutcome(out, file_copy + '.out')
          logPotentialBug(file)
        found_bug = True
        break
    # Remove unused files.
    shutil.rmtree(Env.TMP_DIR)
  os.makedirs(Env.FUZZ_DIR, exist_ok=True)
  fuzz_log_path = path.join(Env.FUZZ_DIR, 'fuzz.log')
  with open(fuzz_log_path, 'a') as fuzz_log:
    fuzz_log.write(f'Fuzz experiment of {tries} tries.')
