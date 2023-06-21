import Env
import json
import matplotlib.pyplot as plt
import os
import os.path as path
import subprocess
from tqdm import tqdm
import random


# dotnet tool install --global coverlet.console


def collect_coverage(dafny_dir, files: list[str], name: str):
  dafny_asm = path.join(dafny_dir, 'Binaries', 'Dafny.dll')
  dafny = path.join(dafny_dir, 'Scripts', 'Dafny')
  os.makedirs(Env.COVERAGE_DIR, exist_ok=True)
  coverage_json = path.join(Env.COVERAGE_DIR, 'coverage.json')
  coverage_xml = path.join(Env.COVERAGE_DIR, 'coverage.cobertura.xml')
  summary_json = path.join(Env.COVERAGE_DIR, 'Summary.json')
  line_cov, branch_cov = [], []
  for i in tqdm(range(len(files))):
    file = files[i]
    cmd = ['coverlet', dafny_asm, '--target', dafny,
           '--targetargs', f'verify --cores=2 {file}',
           '--merge-with', coverage_json, '-o', Env.COVERAGE_DIR + "/",
           '-f', 'json', '-f', 'cobertura']
    subprocess.run(cmd, capture_output=True)
    cmd = ['reportgenerator', f'-reports:{coverage_xml}',
           '-reporttypes:JsonSummary', f'-targetdir:{Env.COVERAGE_DIR}']
    subprocess.run(cmd, capture_output=True)
    with open(summary_json) as summary_file:
      summary = json.load(summary_file)
      for asm in summary['coverage']['assemblies']:
        if asm['name'] == 'DafnyCore':
          line_cov.append(asm['coverage'])
          branch_cov.append(asm['branchcoverage'])
  cov_progression_dict = {
      "name": name,
      "line_cov": line_cov,
      "branch_cov": branch_cov,
      "files": files,
  }
  cov_progression_json = json.dumps(cov_progression_dict, indent=4)
  with open(path.join(Env.COVERAGE_DIR, 'cov_progression.json'), 'a') as file:
    file.write(cov_progression_json)


def run_collect_coverage(name: str):
  files = []
  with open(path.join(Env.MUTANTS_DIR, f'{name}.txt')) as entry_file:
    files = [entry.strip() for entry in entry_file.readlines()]
  random.shuffle(files)
  collect_coverage(dafny_dir=Env.DAFNY_DIR, files=files, name=name)


def read_coverage(name: str):
  coverage_fp = path.join(
      Env.COVERAGE_DIR, f'{name}_coverage', 'cov_progression.json')
  with open(coverage_fp) as coverage_file:
    coverage = json.load(coverage_file)
  return coverage['line_cov'], coverage['branch_cov']


def plot_coverage():
  seed_lc, seed_bc = read_coverage('seed')
  mut_lc, mut_bc = read_coverage('mutant')
  test_num = range(1, 52)
  plt.plot(test_num, mut_lc, 'r.-', label='Line coverage from mutants')
  plt.plot(test_num, mut_bc, 'r+-', label='Branch coverage from mutants')
  plt.plot(test_num, seed_lc, 'b.-', label='Line coverage from seeds')
  plt.plot(test_num, seed_bc, 'b+-', label='Branch coverage from seeds')
  plt.xlabel('Number of tests')
  plt.ylabel('Coverage (%)')
  plt.legend()
  plt.show()
