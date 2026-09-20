using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GPUAcceleratedNormalGenerator
{
    public class Misc
    {
        public static string ReadFromManifest(string name)
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)!;
            using StreamReader sr = new StreamReader(stream!);
            return sr.ReadToEnd();
        }

    }
}
