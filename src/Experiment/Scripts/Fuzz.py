import Dafny
import Env
import MutantGenerator
import random
import os
import os.path as path
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
`Env.MUTANTS_DIR/file` under the name format `file_mutantseed_order.dfy`, 
with a corresponding log `file_mutantseed_order.dfy.log`.
'''


def findDafnyFilesInDir(dir: str):
  # TODO: This does not recursively find files.
  return [os.path.join(dir, f)
          for f in os.listdir(dir) if f.endswith(".dfy")]


def getLogForMutant(mutant: str):
  return mutant + '.log'


def writeOutcomeToFile(result: subprocess.CompletedProcess, file: str):
  with open(file, 'a') as f:
    f.write(f'Return code: {result.returncode}\n')
    f.write(f'Output:\n{result.stdout.decode()}')
    f.write(f'Error:\n{result.stderr.decode()}')


def fuzzSaveErrorOnly(seeds: list[str]):
  '''Save Dafny programs with unexpected errors into Env.BUGS_DIR.'''
  seed = random.choice(seeds)
  seed_name_ext = path.basename(seed)
  seed_name = path.splitext(seed_name_ext)
  seed_dir = path.join(Env.MUTANTS_DIR, seed_name)
  bugdir_seed = path.join(Env.BUGS_DIR, seed_name_ext)
  bugdir_seed_log = getLogForMutant(bugdir_seed)

  seed_result = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=seed)
  if seed_result.returncode is not (0 or 4):
    os.makedirs(Env.BUGS_DIR, exist_ok=True)
    if not path.exists(bugdir_seed_log):
      shutil.copyfile(seed, bugdir_seed)
      writeOutcomeToFile(seed_result, bugdir_seed_log)
    with open(Env.POTENTIAL_BUGS_FILE, 'a') as file:
      file.write(f'{seed_name_ext}\n')
    # Don't try to process seeds with preprocessing/parse/resolve errors.
    return

  # Mutant generation may fail but we don't care.
  MutantGenerator.runMutantGenerator(mutant_generator_path=Env.MUTANT_GEN_PATH,
                                     seed_file=seed, output_dir=Env.MUTANTS_DIR,
                                     num_mutants=5, max_order=20)
  # Try to get mutants generated, if any.
  expected_code = seed_result.returncode
  mutants = findDafnyFilesInDir(seed_dir)
  for mutant in mutants:
    result = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=mutant)
    if result.returncode == expected_code:
      continue
    # Potential bug, save files for inspection.
    mutant_name_ext = path.basename(mutant)
    mutant_log = getLogForMutant(mutant)
    bugdir_mutant = path.join(Env.BUGS_DIR, mutant_name_ext)
    bugdir_mutant_log = getLogForMutant(bugdir_mutant)
    os.makedirs(Env.BUGS_DIR, exist_ok=True)
    if not path.exists(bugdir_seed_log):
      shutil.copyfile(seed, bugdir_seed)
      writeOutcomeToFile(seed_result, bugdir_seed_log)
    if not path.exists(bugdir_mutant_log):
      shutil.move(mutant, bugdir_mutant)
      shutil.move(mutant_log, bugdir_mutant_log)
      writeOutcomeToFile(result, bugdir_mutant_log)
      with open(Env.POTENTIAL_BUGS_FILE, 'a') as file:
        file.write(f'{mutant_name_ext}\n')
  # Remove uninteresting files.
  shutil.rmtree(seed_dir)


def fuzzSaveAll(seeds: list[str]):
  '''Save all generated Dafny programs.

  The verification outcome is saved in the corresponding log file.
  Programs causing unexpected errors are noted in Env.POTENTIAL_BUGS_FILE.
  '''
  seed = random.choice(seeds)
  seed_name_ext = path.basename(seed)
  seed_name = path.splitext(seed_name_ext)
  seed_dir = path.join(Env.MUTANTS_DIR, seed_name)
  seed_log = path.join(seed_dir, f'{seed_name_ext}.log')

  seed_result = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=seed)
  # Create bug directory, copy the seed and save the expected result for
  # this seed if it doesn't exist yet.
  os.makedirs(seed_dir, exist_ok=True)
  if not path.exists(seed_log):
    shutil.copyfile(seed, path.join(seed_dir, seed_name_ext))
    writeOutcomeToFile(seed_result, seed_log)
  # Don't try to process seeds with preprocessing/parse/resolve errors.
  if seed_result.returncode is not (0 or 4):
    return

  # Mutant generation may fail but we don't care.
  MutantGenerator.runMutantGenerator(mutant_generator_path=Env.MUTANT_GEN_PATH,
                                     seed_file=seed, output_dir=seed_dir,
                                     num_mutants=5, max_order=20)
  # Try to get mutants generated, if any.
  expected_code = seed_result.returncode
  mutants = findDafnyFilesInDir(seed_dir)
  for mutant in mutants:
    result = Dafny.verify(dafny_binary_path=Env.DAFNY_BINARY, file=mutant)
    mutant_log = getLogForMutant(mutant)
    writeOutcomeToFile(result, mutant_log)
    mutant_name_ext = path.basename(mutant)
    with open(seed_log, 'a') as log:
      log.write(f'{mutant_name_ext}: {result.returncode}\n')
    # Note down mutant as potential bug if outcomes don't match.
    if result.returncode != expected_code:
      with open(Env.POTENTIAL_BUGS_FILE, 'a') as file:
        file.write(f'{mutant_name_ext}\n')
