from typing import Optional
import Env
import pathlib
import subprocess


def runMutantGenerator(seed_file: str, flags: list[str],
                       work_dir: Optional[str] = None):
  path_to_binary = Env.MUTANT_GEN_PATH
  seed = seed_file
  work_dir = work_dir if work_dir is not None else Env.MUTANTS_DIR
  cmd = [path_to_binary, seed, work_dir] + flags
  return subprocess.run(cmd, capture_output=True)


def runGenMulti(seed_file: str, num_mutants: int = 1, max_order: int = 20,
                work_dir: Optional[str] = None):
  flags = ["gen-multi",
           f"--num-mutants={num_mutants}",
           f"--max-order={max_order}"]
  return runMutantGenerator(seed_file, flags, work_dir)


def runGenSingle(seed_file: str, mutant_seed: int, mutant_order: int,
                 work_dir: Optional[str] = None):
  flags = ["gen-single"
           f"--mutant-seed={mutant_seed}",
           f"--mutant-order={mutant_order}"]
  return runMutantGenerator(seed_file, flags, work_dir)


def generateMutantsFromSeeds(seed_files: list[str],
                             mutants_per_seed: int = 1,
                             max_order: int = 20):
  for seed in seed_files:
    runGenMulti(seed_file=seed,
                num_mutants=mutants_per_seed,
                max_order=max_order)


def generateMutantsFromDir(seed_dir: str,
                           mutants_per_seed: int = 1,
                           max_order: int = 20):
  generateMutantsFromSeeds(seed_files=list(pathlib.Path(seed_dir).glob('*.dfy')),
                           mutants_per_seed=mutants_per_seed,
                           max_order=max_order)
