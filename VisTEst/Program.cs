using JwtVisualiser;
using System;

namespace VisTEst
{
    class Program
    {
        static void Main(string[] args)
        {
            String myString = "eyJ0...";
            JwtTokenVisualiser.TestShowVisualizer(myString);
        }
    }
}
