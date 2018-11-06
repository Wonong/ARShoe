using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

using TensorFlow;

namespace TFClassify
{
    public class BoxOutline
    {
        public int left { get; set; } = 0;
        public int right { get; set; } = 0;
        public int top { get; set; } = 0;
        public int bottom { get; set; } = 0;
        public float Confidence { get; set; } = 0;
        public float Prob { get; set; } = 0;
    }

    public class FootDetector
    {
        private static float IMAGE_STD = 255f;

        private static string INPUT_NODE_NAME = "input";
        private static string OUTPUT_NODE_NAME = "output";

        private TFGraph graph;
        private int inputCNNSize;
        private int inputImageWidth;
        private int inputImageHeight;
        private float[] anchors = { 0.57273f, 0.677385f, 1.87446f, 2.06253f, 3.33843f, 5.47434f, 7.88282f, 3.52778f, 9.77052f, 9.16828f };

        public FootDetector(byte[] model, int inputCNNSize)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            TensorFlowSharp.Android.NativeBinding.Init();
#endif
            this.graph = new TFGraph();
            this.graph.Import(new TFBuffer(model)); // load graph model
            this.inputCNNSize = inputCNNSize;
        }

        public void SetInputImageSize(int inputImageWidth, int inputImageHeight)
        {
            this.inputImageWidth = inputImageWidth;
            this.inputImageHeight = inputImageHeight;
        }

        public Task<List<BoxOutline>> DetectAsync(Color32[] data)
        {
            return Task.Run(() =>
            {
                using (var session = new TFSession(this.graph))
                {
                    using (var tensor_input_image = TransformInput(data, inputCNNSize, inputCNNSize))
                    {
                        var runner = session.GetRunner();
                        runner.AddInput(this.graph[INPUT_NODE_NAME][0], tensor_input_image)
                              .Fetch(graph[OUTPUT_NODE_NAME][0]);
                        UnityEngine.Debug.Log("runner.Run()");

                        var output = runner.Run();

                        // change tensor output to float array
                        // netout shape is [1, grid_h, grid_w, 30]
                        var netout = (float[,,,])output[0].GetValue(jagged: false);

                        return FindBoxes(netout);
                    }
                }
            });
        }

        public List<BoxOutline> Detect(Color32[] data)
        {
            TimeSpan time;
            DateTime start;

            start = DateTime.Now;
            using (var session = new TFSession(this.graph))
            {
                using (var tensor_input_image = TransformInput(data, inputCNNSize, inputCNNSize))
                {
                    var runner = session.GetRunner();
                    runner.AddInput(this.graph[INPUT_NODE_NAME][0], tensor_input_image)
                          .Fetch(graph[OUTPUT_NODE_NAME][0]);
                    UnityEngine.Debug.Log("runner.Run()");

                    var output = runner.Run();

                    // change tensor output to float array
                    // netout shape is [1, grid_h, grid_w, 30]
                    var netout = (float[,,,])output[0].GetValue(jagged: false);

                    return FindBoxes(netout);
                }
            }
        }

        /*
         * transform input image to tensor with shape: [1, widht, height, 3]
         */
        private static TFTensor TransformInput(Color32[] pic, int width, int height)
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
                    //int i = y * width + x;

                    // normalize pixel value ( pixel_value = ((pixel_value - mean_pixel_value) / 255))
                    //floatValues[i * 3 + 0] = (float)((color.b) / IMAGE_STD);
                    //floatValues[i * 3 + 1] = (float)((color.g) / IMAGE_STD);
                    //floatValues[i * 3 + 2] = (float)((color.r) / IMAGE_STD);
                    floatValues[i * 3 + 0] = (float)((color.r) / IMAGE_STD);
                    floatValues[i * 3 + 1] = (float)((color.g) / IMAGE_STD);
                    floatValues[i * 3 + 2] = (float)((color.b) / IMAGE_STD);
                }
            }

            TFShape shape = new TFShape(1, width, height, 3);
            return TFTensor.FromBuffer(shape, floatValues, 0, floatValues.Length);
        }

        private List<BoxOutline> FindBoxes(float[,,,] netout)
        {
            int origW = inputImageWidth, origH = inputImageHeight;
            var grid_h = netout.GetLength(1);
            var grid_w = netout.GetLength(2);
            var nb_box = netout.GetLength(3);
            int nb_classes = 1;
            int nb_anchors = 5;
            int t = (5 + nb_classes);
            float iou_threshold = 0.5f;
            float prob_threshold = 0.6f;
            float[,,] probs = new float[grid_h, grid_w, nb_anchors];

            for (int row = 0; row < grid_h; row++)
            {
                for(int col = 0; col < grid_w; col++)
                {
                    for(int box_loop = 0; box_loop < nb_anchors; box_loop++)
                    {
                        netout[0, row, col, t * box_loop + 4] = Expit(netout[0, row, col, t * box_loop + 4]);
                        netout[0, row, col, t * box_loop + 0] = (col + Expit(netout[0, row, col, t * box_loop + 0])) / grid_w;
                        netout[0, row, col, t * box_loop + 1] = (row + Expit(netout[0, row, col, t * box_loop + 1])) / grid_h;
                        netout[0, row, col, t * box_loop + 2] =  (float)Math.Exp(netout[0, row, col, t * box_loop + 2]) * anchors[2 * box_loop + 0] / grid_w;
                        netout[0, row, col, t * box_loop + 3] = (float)Math.Exp(netout[0, row, col, t * box_loop + 3]) * anchors[2 * box_loop + 1] / grid_h;

                        if (netout[0, row, col, t * box_loop + 4] > iou_threshold)
                        {
                            //probs[(row* grid_w + col)* nb_anchors + box_loop] = netout[0, row, col, t * box_loop + 4];
                            probs[row, col, box_loop] = netout[0, row, col, t * box_loop + 4];
                        }
                    }
                }
            }

            var boxes = new List<BoxOutline>();

            int pred_length = grid_h * grid_w * nb_anchors;
            for (int index = 0; index < pred_length; index++)
            {
                int row = index / (grid_w * nb_anchors);
                int col = (index % (grid_w * nb_anchors)) / nb_anchors;
                int box_loop = index % nb_anchors;
                if (probs[row, col, box_loop] <= 0) continue;

                for(int index2 = index + 1; index2 < pred_length; index2++)
                {
                    int row2 = index2 / (grid_w * nb_anchors);
                    int col2 = (index2 % (grid_w * nb_anchors)) / nb_anchors;
                    int box_loop2 = index2 % nb_anchors;

                    if (index == index2) continue;
                    if (probs[row2, col2, box_loop2] <= 0) continue;

                    if (BoxIOU(
                        netout[0, row, col, t * box_loop + 0],
                        netout[0, row, col, t * box_loop + 1],
                        netout[0, row, col, t * box_loop + 2],
                        netout[0, row, col, t * box_loop + 3],
                        netout[0, row2, col2, t * box_loop2 + 0],
                        netout[0, row2, col2, t * box_loop2 + 1],
                        netout[0, row2, col2, t * box_loop2 + 2],
                        netout[0, row2, col2, t * box_loop2 + 3] ) >= 0.4)
                    {
                        if (probs[row2, col2, box_loop2] > probs[row, col, box_loop])
                        {
                            probs[row, col, box_loop] = 0;
                            break;
                        }
                        probs[row2, col2, box_loop2] = 0;
                    }
                }

                float X = netout[0, row, col, t * box_loop + 0];
                float Y = netout[0, row, col, t * box_loop + 1];
                float W = netout[0, row, col, t * box_loop + 2];
                float H = netout[0, row, col, t * box_loop + 3];
                float C = netout[0, row, col, t * box_loop + 4];
                float P = probs[row, col, box_loop];

                if (P > prob_threshold)
                {
                    var b = new BoxOutline();
                    int left = (int)((X - W / 2.0f) * origW);
                    int right = (int)((X + W / 2.0f) * origW);
                    int top = origH - (int)((Y - H / 2.0f) * origH);
                    int bottom = origH - (int)((Y + H / 2.0f) * origH);

                    if (left < 0) left = 0;
                    if (right > origW) right = origW - 1;
                    if (top < 0) top = 0;
                    if (bottom > origH - 1) bottom = origH - 1;

                    b.left = left;
                    b.right = right;
                    b.top = top;
                    b.bottom = bottom;
                    b.Confidence = C;
                    b.Prob = P;

                    boxes.Add(b);
                }
            }

            return boxes;
        }

        private float Expit(float x)
        {
            float y = 1 / (1 + (float)Math.Exp(-x));
            return y;
        }

        private float BoxIOU(float ax, float ay, float aw, float ah,
                             float bx, float by, float bw, float bh)
        {
            return BoxIntersection(ax, ay, aw, ah, bx, by, bw, bh) / BoxUnion(ax, ay, aw, ah, bx, by, bw, bh);
        }

        private float BoxIntersection(float ax, float ay, float aw, float ah,
                                      float bx, float by, float bw, float bh)
        {
            float w, h, area;
            w = Overlap(ax, aw, bx, bw);
            h = Overlap(ay, ah, by, bh);
            if (w < 0 || h < 0) {
                return 0;
            }

            area = w * h;
            return area;
        }

        private float BoxUnion(float ax, float ay, float aw, float ah,
                               float bx, float by, float bw, float bh)
        {
            float i, u;
            i = BoxIntersection(ax, ay, aw, ah, bx, by, bw, bh);
            u = aw * ah + bw * bh - i;
            return u;
        }

        private float Overlap(float x1, float w1, float x2, float w2)
        {
            float l1, l2, left, right;
            l1 = x1 - w1 / 2.0f;
            l2 = x2 - w2 / 2.0f;
            left = Math.Max(l1, l2);
            right = Math.Min(x1 + w1 / 2.0f, x2 + w2 / 2.0f);
            return right - left;
        }
    }
}