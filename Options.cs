using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUAcceleratedNormalGenerator
{
    public class Options
    {
        [Option('i', "input", Required = false, HelpText = "Path to the input image file (RGBA/RGB).")]
        public string InputPath { get; set; } = string.Empty;

        [Option('o', "output", Required = false, HelpText = "Path to save the generated normal map.")]
        public string OutputPath { get; set; } = string.Empty;

        [Option('h', "heightmap-out-path", Required = false, HelpText = "Path to save the generated heightmap.")]
        public string HeightmapOutputPath { get; set; } = string.Empty;

        [Option('s', "strength", Default = 1.0f, HelpText = "Normal map bump strength/height scale multiplier.")]
        public float Strength { get; set; } = 1.0f;

        [Option('f', "batch-file", Required = false, HelpText = "Path to a pipe-separated task file (source|heightmap|blur|normal) for batch processing.")]
        public string BatchFilePath { get; set; } = string.Empty;
        [Option('b', "max-batch-size", Default = 32, HelpText = "Maximum number of images to process in a single texture array batch.")]
        public int MaxBatchSize { get; set; } = 32;

        [Option("save-heightmaps-for-batch", Default = false, HelpText = "Saves the generated intermediate heightmap files to disk during batch processing.")]
        public bool SaveHeightMapsForBatch { get; set; } = false;

        [Option("save-blur-for-batch", Default = false, HelpText = "Saves the intermediate blurred/smoothed heightmap textures generated during batch processing.")]
        public bool SaveBlursForBatch { get; set; } = false;
    }
}
