from datetime import datetime
import Env
import os
import subprocess


def runMutantGenerator(mutant_generator_path: str, seed_file: str,
                       output_dir: str, num_mutants: int = 1,
                       max_order: int = 20):
  cmd = [mutant_generator_path,
         f"--seed={seed_file}",
         f"--output-dir={output_dir}",
         f"--num-mutants={num_mutants}",
         f"--max-order={max_order}"]
  proc = subprocess.run(cmd, capture_output=True)
  return proc


def generateMutants(seed_files: list[str],
                    mutants_per_seed: int = 1,
                    max_order: int = 20):
  files_total = 0
  files_successfully_mutated = []
  files_with_parse_resolve_errors = []
  files_without_mutations = []
  files_with_internal_errors = []
  for seed in seed_files:
    files_total += 1
    p = runMutantGenerator(mutant_generator_path=Env.MUTANT_GEN_PATH,
                           seed_file=seed,
                           output_dir=Env.MUTANTS_DIR,
                           num_mutants=mutants_per_seed,
                           max_order=max_order)
    code = p.returncode
    if code == 0:
      files_successfully_mutated.append(seed)
    elif code == 20:
      files_with_parse_resolve_errors.append(seed)
    elif code == 50:
      files_without_mutations.append(seed)
    else:
      files_with_internal_errors.append(seed)
  SUMMARY_LOG = os.path.join(Env.MUTANTS_DIR, 'summary.log')
  SUMMARY_TEMPLATE = '''==================================
{timestamp}
Total files: {files_total}
Successfully mutated: {files_successfully_mutated}
Files without mutations: {files_without_mutations}
Files with parse/resolve errors: {files_with_parse_resolve_errors}
Files with internal errors: {files_with_internal_errors}
'''
  with open(SUMMARY_LOG, 'a') as log:
    log.write(SUMMARY_TEMPLATE.format(
        timestamp=datetime.now(),
        files_total=files_total,
        files_successfully_mutated=f'{len(files_successfully_mutated)}\n' +
        '\n'.join(files_successfully_mutated),
        files_without_mutations=f'{len(files_without_mutations)}\n' +
        '\n'.join(files_without_mutations),
        files_with_parse_resolve_errors=f'{len(files_with_parse_resolve_errors)}\n' +
        '\n'.join(files_with_parse_resolve_errors),
        files_with_internal_errors=f'{len(files_with_internal_errors)}\n' +
        '\n'.join(files_with_internal_errors)
    ))


def generateMutantsFromDir(seed_dir: str,
                           mutants_per_seed: int = 1,
                           max_order: int = 20):
  # TODO: This does not recursively find files.
  files = [os.path.join(seed_dir, f)
           for f in os.listdir(seed_dir) if f.endswith(".dfy")]
  generateMutants(files, mutants_per_seed, max_order)

generateMutants(['../old_work_dir/fuzzd/files/reconditioned/fuzzd_27.dfy'])