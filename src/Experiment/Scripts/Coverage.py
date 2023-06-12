import Env
import json
import os
import os.path as path
import shlex
import subprocess

# dotnet tool install --global coverlet.console


def collect_coverage(dafny_dir, files: list[str], name: str):
  dafny_asm = os.path.join(dafny_dir, 'Binaries', 'Dafny.dll')
  dafny = os.path.join(dafny_dir, 'Scripts', 'Dafny')
  coverage_json = path.join(Env.COVERAGE_DIR, 'coverage.json')
  coverage_xml = path.join(Env.COVERAGE_DIR, 'coverage.cobertura.xml')
  summary_json = path.join(Env.COVERAGE_DIR, 'Summary.json')
  line_cov, branch_cov = [], []
  for file in files:
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
  }
  cov_progression_json = json.dumps(cov_progression_dict, indent=4)
  with open(path.join(Env.COVERAGE_DIR, 'cov_progression.json'), 'a') as file:
    file.write(cov_progression_json)
