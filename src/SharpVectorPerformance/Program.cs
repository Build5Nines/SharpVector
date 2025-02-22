// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;

namespace SharpVectorPerformance;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<MemoryVectorDatabasePerformance>();
    }
}