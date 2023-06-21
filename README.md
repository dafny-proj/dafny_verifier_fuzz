# dafny_verifier_fuzz

*(Experimental, Use with caution)*

An investigation into mutation-based fuzzing targeted at the Dafny verifier. We develop a standalone mutant generator and a fuzzing workflow for this purpose.

## Mutant Generator
For a seed `<seed_name>.dfy`, the mutant generator creates equivalent mutants from the seed. A log for each mutant which records the mutations applied to the seed is also generated. These files are placed under the `<work_dir>/<seed_name>` directory. `work_dir` is provided as a user argument. 

Each mutant and its log will be named as `<seed_name>_<mutant_seed>_<mutant_order>.dfy(.log)`. `mutant_seed` and `mutant_order` define a specific combination of mutations on the seed. `mutant_seed` sets the random seed for the mutation decisions, whilst `mutant_order` sets the amount of mutations applied on the seed. 

The mutant generator can be invoked as a library or as an executable. The steps for using the mutant generator as an executable are provided below.

### Running the executable.
Two approaches are available for running the executable.
#### Approach 1: 
```
cd src/MutantGeneratorMain
dotnet run [cmd] [arguments] [options]
```
#### Approach 2: 
```
cd src/MutantGeneratorMain
dotnet build
bin/Debug/net7.0/MutantGeneratorMain [cmd] [arguments] [options]
```
The mutant generator can be used to generate a single mutant or multiple mutants from one run. We describe both usages next.

### Generating a single mutant.
A single mutant is generated for a specific seed program, mutant seed and mutant order. Useful for reproducing files during debugging or development.
```
Usage:
  MutantGeneratorMain gen-single <seed> <work-dir> [options]

Arguments:
  <seed>      The seed file from which mutants are generated.
  <work-dir>  The directory to output mutants and logs to.

Options:
  --mutant-seed <mutant-seed>    Random seed for determining the mutations applied.
  --mutant-order <mutant-order>  Number of mutations to apply.
```

### Generating multiple mutants.
Multiple mutants are generated from a specific seed program using random mutant seeds and mutant order.
```
Usage:
  MutantGeneratorMain gen-multi <seed> <work-dir> [options]

Arguments:
  <seed>      The seed file from which mutants are generated.
  <work-dir>  The directory to output mutants and logs to.

Options:
  --num-mutants <num-mutants>  The number of mutants to generate.
  --max-order <max-order>      The maximum number of mutations applied to generate a mutant.
```

### Extending the mutant generator.
The existing equivalence mutations can be found in the [MutantGenerator\Mutators directory](src/MutantGenerator/Mutators/). The mutant generator can be extended by adding additional mutators that implement the `IMutator` interface in this section of the repository.

## Fuzzing
Example scripts demonstrating how the mutant generator can be invoked for fuzzing and coverage collection are provided in the [Experiment directory](src/Experiment/) of this repository.
