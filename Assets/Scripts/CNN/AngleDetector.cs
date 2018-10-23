using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

using TensorFlow;

namespace TFClassify
{
    public class AngleDetector
    {
        private static float IMAGE_STD = 255f;
        private static float[] MEAN_PIXEL_RGB = { 123.7f, 116.8f, 103.9f };
        private static float[] MEAN_PIXEL_BGR = { 103.9f, 116.8f, 123.7f };
        
        private static int NUM_CLASSES = 36;

        private static string INPUT_NODE_NAME = "input_image";
        private static string OUTPUT_NODE_NAME = "angle_y_output/Softmax";

        private TFGraph graph;
        private int inputSize;

        public AngleDetector(byte[] model, int inputSize)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            TensorFlowSharp.Android.NativeBinding.Init();
#endif
            this.graph = new TFGraph();
            this.graph.Import(new TFBuffer(model)); // load graph model

            this.inputSize = inputSize;
        }

        public Task<float> DetectAsync(Color32[] data)
        {
            return Task.Run(() =>
            {
                using (var session = new TFSession(this.graph))
                {
                    using (var tensor_input_image = TransformInput(data, inputSize, inputSize))
                    {
                        var runner = session.GetRunner();
                        runner.AddInput(this.graph[INPUT_NODE_NAME][0], tensor_input_image)
                              .Fetch(graph[OUTPUT_NODE_NAME][0]);
                        UnityEngine.Debug.Log("runner.Run()");
                        var output = runner.Run();

                        // change tensor output to float array
                        // netout shape is [1, NUM_CLASSES]
                        var tmp = (float[,])output[0].GetValue(jagged: false);

                        List<float> netout = new List<float>() { };

                        for(int i = 0; i < NUM_CLASSES; i++)
                        {
                            netout.Add(tmp[0, i]);
                        }
                        var sorted = netout.Select((x, i) => new KeyValuePair<float, int>(x, i))
                                           .OrderBy(x => -x.Key)
                                           .ToList();
                        List<int> sorted_idx = sorted.Select(x => x.Value).ToList();

                        var first_idx = sorted_idx[0];
                        var second_idx = sorted_idx[1];
                        var third_idx = sorted_idx[2];
                        return (second_idx * 10 * netout[second_idx]) + (third_idx * 10 * netout[third_idx]) +
                               (first_idx * 10 * (1 - netout[second_idx] - netout[third_idx]));
                    }
                }
            });
        }

        public float Detect(Color32[] data)
        {
            using (var session = new TFSession(this.graph))
            {
                using (var tensor_input_image = TransformInput(data, inputSize, inputSize))
                {
                    var runner = session.GetRunner();
                    runner.AddInput(this.graph[INPUT_NODE_NAME][0], tensor_input_image)
                          .Fetch(graph[OUTPUT_NODE_NAME][0]);
                    UnityEngine.Debug.Log("runner.Run()");
                    var output = runner.Run();

                    // change tensor output to float array
                    // netout shape is [1, NUM_CLASSES]
                    var tmp = (float[,])output[0].GetValue(jagged: false);

                    List<float> netout = new List<float>() { };

                    for (int i = 0; i < NUM_CLASSES; i++)
                    {
                        netout.Add(tmp[0, i]);
                    }
                    var sorted = netout.Select((x, i) => new KeyValuePair<float, int>(x, i))
                                       .OrderBy(x => -x.Key)
                                       .ToList();
                    List<int> sorted_idx = sorted.Select(x => x.Value).ToList();

                    var first_idx = sorted_idx[0];
                    var second_idx = sorted_idx[1];
                    var third_idx = sorted_idx[2];
                    return (second_idx * 10 * netout[second_idx]) + (third_idx * 10 * netout[third_idx]) +
                           (first_idx * 10 * (1 - netout[second_idx] - netout[third_idx]));
                }
            }
        }

        /*
         * transform input image to tensor with shape: [1, widht, height, 3]
         */
        public static TFTensor TransformInput(Color32[] pic, int width, int height)
        {
            float[] floatValues = new float[width * height * 3];

            // order of image file which read by python cv.imread is BGR
            // and left -> right and top -> bottom
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var color = pic[y * width + x];
                    int i = (height - y - 1) * width + x; // for change bottom and top


                    // normalize pixel value ( pixel_value = ((pixel_value - mean_pixel_value) / 255))
                    floatValues[i * 3 + 0] = (float)((color.b - MEAN_PIXEL_BGR[0]) / IMAGE_STD);
                    floatValues[i * 3 + 1] = (float)((color.g - MEAN_PIXEL_BGR[1]) / IMAGE_STD);
                    floatValues[i * 3 + 2] = (float)((color.r - MEAN_PIXEL_BGR[2]) / IMAGE_STD);
                }
            }

            TFShape shape = new TFShape(1, width, height, 3);
            return TFTensor.FromBuffer(shape, floatValues, 0, floatValues.Length);
        }
    }
}