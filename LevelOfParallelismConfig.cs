using Nunit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly : LevelOfParallelism(2)]