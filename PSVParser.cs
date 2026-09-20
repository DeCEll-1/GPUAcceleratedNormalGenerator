using System;
using System.Collections.Generic;
using System.Text;

namespace GPUAcceleratedNormalGenerator
{
    public class BatchTask
    {
        public string SourcePath { get; set; } = string.Empty;
        public string? HeightmapPath { get; set; }
        public string? BlurPath { get; set; }
        public string NormalPath { get; set; } = string.Empty;
    }
    public class PSVParser
    {


        public static List<BatchTask> LoadAndValidateBatchFile(string batchFilePath)
        {
            if (string.IsNullOrWhiteSpace(batchFilePath) || !File.Exists(batchFilePath))
            {
                throw new FileNotFoundException($"Batch file not found: {batchFilePath}");
            }

            var tasks = new List<BatchTask>();
            var lines = File.ReadAllLines(batchFilePath);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue; // Skip comments/empty lines

                var parts = line.Split('|');
                if (parts.Length != 4)
                {
                    throw new FormatException($"Invalid format on line {i + 1}: expected 4 pipe-separated values.");
                }

                string sourcePath = parts[0].Trim();
                string heightmapPath = parts[1].Trim();
                string blurPath = parts[2].Trim();
                string normalPath = parts[3].Trim();

                if (!File.Exists(sourcePath))
                    throw new FileNotFoundException($"Source image does not exist on line {i + 1}: {sourcePath}");

                if (string.IsNullOrEmpty(normalPath))
                    throw new ArgumentException($"Normal output path cannot be empty on line {i + 1}.");

                var normalDir = Path.GetDirectoryName(normalPath);
                if (!string.IsNullOrEmpty(normalDir) && !Directory.Exists(normalDir))
                    Directory.CreateDirectory(normalDir);

                if (heightmapPath != "_")
                {
                    var heightmapDir = Path.GetDirectoryName(heightmapPath);
                    if (!string.IsNullOrEmpty(heightmapDir) && !Directory.Exists(heightmapDir))
                        Directory.CreateDirectory(heightmapDir);
                }

                if (blurPath != "_")
                {
                    var blurDir = Path.GetDirectoryName(blurPath);
                    if (!string.IsNullOrEmpty(blurDir) && !Directory.Exists(blurDir))
                        Directory.CreateDirectory(blurDir);
                }

                tasks.Add(new BatchTask
                {
                    SourcePath = sourcePath,
                    HeightmapPath = heightmapPath == "_" ? null : heightmapPath,
                    BlurPath = blurPath == "_" ? null : blurPath,
                    NormalPath = normalPath
                });
            }

            return tasks;
        }
    }
}
