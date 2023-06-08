import subprocess


def verify(dafny_binary_path: str, file: str):
  flags = ["verify",
           "--cores=2",
           "--use-basename-for-filename",
           "--verification-time-limit=300"]
  cmd = [dafny_binary_path] + flags + [file]
  proc = subprocess.run(cmd, capture_output=True)
  return proc
