# IMinimalDAG 

An incrementally constructed (ie no trie involved) minimal directed acyclic graph. Aimed at minimum-node storage of collections of collections of values for fast, memory-friendly searching. Supports both ref and value types. Happily serializes down to a gzipped json (and back) for persistence of calculated MinimalDAGs (note this is not a significant speed increase until collections are very* large). *hundreds of thousands of entries.

Largely written as a test of both production-style complexity in code, and of the performance of moving through a large number of `yield` commands.


I have not verified this is in a working state; it's purely another example of old code. 
